import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import classification_report
from sklearn.metrics import accuracy_score

import joblib

# load dataset
df = pd.read_json("../data/trainingdata.json")

if 'hour' not in df.columns:
    rng = np.random.default_rng(42)
    df['hour'] = rng.integers(0,24,len(df))

if 'isNonCadCurrency' not in df.columns:
    df['isNonCadCurrency'] = df['isForeignTransaction']

fraud_sample = pd.DataFrame([{
    'amount' : a, 'isForeignTransaction' :1,
    'isNightTransaction': 1, 'isFraud': 1
} for a in [8000, 12000, 15000, 9500, 11000, 7500, 18000, 6500]
for h in [1,2,3,4]])
print(df)

legit_samples = pd.DataFrame([{
    'amount': a, 'isForeignTransaction': 0,
    'isNightTransaction': 0, 'hour': h,
    'isNonCadCurrency': 0, 'isFraud': 0
} for a in [500, 1000, 1500, 200, 800, 300, 1200, 2000]
  for h in [9, 10, 14, 15]])

df = pd.concat([df, fraud_sample, legit_samples], ignore_index=True)

#  Features
X = df [[
    "amount",
    "isForeignTransaction",
    "isNightTransaction"    ]]

#   Label
y = df["isFraud"]


X_train,X_test, y_train, y_test =(train_test_split(X,y,test_size=0.2,random_state=42))

model = RandomForestClassifier(class_weight='balanced', n_estimators=100, random_state=42)

model.fit(X_train,y_train)

predictions = model.predict(X_test)

accuracy = accuracy_score(y_test,predictions)

print(f"Accuracy: {accuracy}")

print( classification_report ( y_test , predictions))

# save model
joblib.dump(model , "../models/fraud_model.pkl")


print("model saved")
