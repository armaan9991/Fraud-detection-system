# Fraud Detection System

A full-stack fraud detection platform that combines rule-based analysis and machine learning to identify potentially fraudulent financial transactions in real time. The system provides automated risk scoring, fraud alerts, audit logging, and administrative monitoring tools.

## Live Demo

Frontend: https://fraud-detection-system-phi.vercel.app

## Source Code

GitHub: https://github.com/armaan9991/Fraud-detection-system

---

## Architecture

* Frontend: React + TypeScript (Vercel)
* Backend API: ASP.NET Core (.NET 8) (Render)
* ML Service: FastAPI + Scikit-learn (Render)
* Database: PostgreSQL (Neon)
* Caching: Redis
* Background Processing: Hangfire

---

## Features

### Authentication & Security

* JWT Authentication
* Refresh Token Rotation
* Role-Based Authorization
* Password Hashing
* Rate Limiting
* Audit Logging

### Fraud Detection

* Rule-Based Fraud Scoring
* Machine Learning Risk Prediction
* Automated Fraud Alerts
* Transaction Monitoring
* Risk-Level Classification

### Administration

* Admin Dashboard
* User Management
* Transaction Review
* Fraud Alert Management
* System Statistics

---

## Tech Stack

### Backend

* ASP.NET Core 8
* Entity Framework Core
* PostgreSQL
* Hangfire
* Redis
* MailKit
* Swagger / OpenAPI

### Frontend

* React 19
* TypeScript
* Vite
* TailwindCSS
* Zustand
* TanStack Query
* Axios
* Recharts

### Machine Learning

* FastAPI
* Scikit-learn
* XGBoost
* Pandas
* NumPy
* Joblib

---

## System Design

1. User submits a transaction.
2. The rule engine evaluates transaction risk factors.
3. The ML service generates a fraud probability score.
4. Results are combined into a final fraud score.
5. High-risk transactions automatically generate fraud alerts.
6. Audit logs and reports are created for monitoring and analysis.

---

## Deployment

* Frontend deployed on Vercel
* Backend API deployed on Render
* ML Microservice deployed on Render
* PostgreSQL hosted on Neon
* Dockerized backend deployment

---

## Future Improvements

* Enhanced ML model trained on larger real-world datasets
* Improved email notification infrastructure
* Monitoring and observability dashboards
* CI/CD automation
* Integration testing coverage
* Advanced fraud detection features
* Performance optimization and scalability improvements

---

## Learning Outcomes

This project provided hands-on experience with:

* Full-stack application development
* ASP.NET Core and Entity Framework Core
* JWT authentication and authorization
* Cloud deployment and infrastructure management
* Database migrations and PostgreSQL administration
* Microservice communication between .NET and Python services
* Redis caching and background job processing
* Machine learning integration in production-style applications
* Debugging, monitoring, and troubleshooting distributed systems

---

## Author

**Armaan Gill**

Computer Science Student at the University of Calgary

Interested in Backend Engineering, Full-Stack Development, Cloud Infrastructure, and Machine Learning.

LinkedIn: https://www.linkedin.com/in/armaan-gill-776aa43b4/
