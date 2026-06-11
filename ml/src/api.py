from fastapi import FastAPI
from pydantic import BaseModel

import joblib
import numpy as np
import pandas as pd

Model = joblib.load("../models/fraud_model.pkl")

app = FastAPI()

# Request schema
class Transactiondata(BaseModel):
    amount: float
    isForeignTransaction : int
    isNightTransaction : int

# prediction endpoint
@app.post("/predict")

# def predict_fraud(data: Transactiondata):
#     features = np.array([
#         [
#             data.amount,
#             data.isForeignTransaction,
#             data.isNightTransaction,
#             data.fraudScore
#         ]
#     ])


def predict_fraud(data : Transactiondata):
    print(data)
    print(data.model_dump())
    features = pd.DataFrame([{
        'amount' : data.amount,
        'isForeignTransaction' : data.isForeignTransaction,
        'isNightTransaction' : data.isNightTransaction,
    }])

    prediction = Model.predict(features)[0]

    probabilty = Model.predict_proba(features)[0][1]
    threshold = 0.3
    print(prediction ,"  ", probabilty)
    return {
        "prediction" : int(prediction),
        "fraudProbability" : float(probabilty)
    }

