import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split, cross_val_score
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

fraud_samples = pd.DataFrame([{
    'amount': a, 'isForeignTransaction': 1,
    'isNightTransaction': 1, 'hour': h,
    'isNonCadCurrency': 1, 'isFraud': 1
} for a in [8000, 12000, 15000, 9500, 11000, 7500, 18000, 6500]
  for h in [1, 2, 3, 4]])
print(df)

legit_samples = pd.DataFrame([{
    'amount': a, 'isForeignTransaction': 0,
    'isNightTransaction': 0, 'hour': h,
    'isNonCadCurrency': 0, 'isFraud': 0
} for a in [500, 1000, 1500, 200, 800, 300, 1200, 2000]
  for h in [9, 10, 14, 15]])

df = pd.concat([df, fraud_samples, legit_samples], ignore_index=True)
print(f"Total: {len(df)}, Fraud: {df['isFraud'].sum()}, Legit: {(df['isFraud']==0).sum()}")

#  Features
X = df [[
    "amount",
    "isForeignTransaction",
    "isNightTransaction",
    "hour",
    "isNonCadCurrency"
    ]]

#   Label
y = df["isFraud"]


X_train,X_test, y_train, y_test =(train_test_split(X,y,test_size=0.2,random_state=42,stratify=y))

model = RandomForestClassifier(class_weight='balanced', n_estimators=200, random_state=42,max_depth=8)

model.fit(X_train,y_train)

## Cross Validation
cv_scores = cross_val_score(model,X,y,cv=5,scoring='f1')
print(f"CV F1 scores: {cv_scores.round(3)}, Mean: {cv_scores.mean():.3f}")

predictions = model.predict(X_test)

print(f"Accuracy: {accuracy_score(y_test, predictions):.3f}")

print(classification_report(y_test, predictions))

# save model
joblib.dump(model , "../models/fraud_model.pkl")


print("model saved")
