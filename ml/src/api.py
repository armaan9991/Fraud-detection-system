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
    hour : int
    isNonCadCurrency : int

# prediction endpoint
@app.post("/predict")


def predict_fraud(data : Transactiondata):
    print(data)
    print(data.model_dump())
    features = pd.DataFrame([{
        'amount' : data.amount,
        'isForeignTransaction' : data.isForeignTransaction,
        'isNightTransaction' : data.isNightTransaction,
        'hour' : data.hour,
        'isNonCadCurrency' : data.isNonCadCurrency,
    }])

    probabilty = Model.predict_proba(features)[0][1]
    prediction = 1 if probabilty >= 0.35 else 0

    print(prediction ,"  ", probabilty)
    return {
        "prediction" : int(prediction),
        "fraudProbability" : float(probabilty)
    }

