from fastapi import FastAPI
from pydantic import BaseModel

import joblib
import numpy as np

Model = joblib.load("../models/fraud_model.pkl")

app = FastAPI()

# Request schema
class Transactiondata(BaseModel):
    amount: float
    isForeignTransaction : int
    isNightTransaction : int
    fraudScore: int

# prediction endpoint
@app.post("/predict")

def predict_fraud(data: Transactiondata):
    features = np.array([
        [
            data.amount,
            data.isForeignTransaction,
            data.isNightTransaction,
            data.fraudScore
        ]
    ])

    prediction = Model.predict(features)[0]

    probabilty = Model.predict_proba(features)[0][1]

    return {
        "prediction" : int(prediction),
        "fraudProbability" : float(probabilty)
    }

