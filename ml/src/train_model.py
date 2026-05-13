import pandas as pd

from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import classification_report
from sklearn.metrics import accuracy_score

import joblib

# load dataset
df = pd.read_json("../data/trainingdata.json")

print(df)

#  Features
X = df [[
    "amount",
    "isForeignTransaction",
    "isNightTransaction",
    "fraudScore"
    ]]

#   Label
y = df["isFraud"]


X_train,X_test, y_train, y_test =(train_test_split(X,y,test_size=0.2,random_state=42))

model = RandomForestClassifier()

model.fit(X_train,y_train)

predictions = model.predict(X_test)

accuracy = accuracy_score(y_test,predictions)

print(f"Accuracy: {accuracy}")

print( classification_report ( y_test , predictions))

# save model
joblib.dump(model , "../model/fraud_model.pk1")


print("model saved")
