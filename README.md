# Fraud Detection System

A full-stack fraud detection platform that combines rule-based analysis and machine learning to identify potentially fraudulent financial transactions in real time. The system provides automated risk scoring, fraud alerts, audit logging, and administrative monitoring tools.

## Live Demo

Frontend: https://fraud-detection-system-phi.vercel.app

## Source Code

GitHub: https://github.com/armaan9991/Fraud-detection-system

---

## Problem Statement

Financial fraud detection systems must identify suspicious transactions quickly while minimizing false positives. This project combines rule-based analysis and machine learning predictions to provide explainable fraud scoring, automated alerts, and administrative oversight for transaction monitoring.

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
* Email Notifications for High-Risk Transactions

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
5. High-risk transactions automatically generate fraud alerts and email notifications.
6. Audit logs and reports are created for monitoring and analysis.

---

## Deployment

* Frontend deployed on Vercel
* Backend API deployed on Render
* ML Microservice deployed on Render
* PostgreSQL hosted on Neon
* Redis caching enabled
* Dockerized backend deployment

---

## Monitoring & Operations

* Structured application logging
* Centralized error handling middleware
* Hangfire Dashboard for background job monitoring
* Health monitoring through Render deployments
* PostgreSQL and infrastructure monitoring through Neon and Render dashboards

---

## Project Status

Fully deployed across Vercel, Render, and Neon

End-to-end transaction processing workflow

JWT authentication and refresh token support

Rule-based and machine learning fraud detection

Automated fraud alert generation and email notifications

Background job processing with Hangfire

Redis caching integration

PostgreSQL persistence with Entity Framework Core

Production deployment and monitoring

---

## Future Improvements

* Improve the fraud detection model with larger and more representative datasets
* Implement automated CI/CD testing pipelines with GitHub Actions
* Expand integration and end-to-end test coverage
* Explore advanced fraud detection techniques and feature engineering

---

## Learning Outcomes

This project provided hands-on experience with:

* Full-stack application development using React and ASP.NET Core
* Designing and building secure REST APIs
* JWT authentication, refresh tokens, and role-based authorization
* Cloud deployment across Vercel, Render, and Neon
* Database migrations and PostgreSQL administration
* Microservice communication between .NET and Python services
* Redis caching and Hangfire background job processing
* Integrating machine learning models into production-style applications
* Debugging, monitoring, and troubleshooting distributed systems
* Managing environment variables, deployments, and infrastructure configuration

---

## Author

**Armaan Gill**

Computer Science Student at the University of Calgary

Interested in Backend Engineering, Full-Stack Development, Cloud Infrastructure, and Machine Learning.

LinkedIn: https://www.linkedin.com/in/armaan-gill-776aa43b4/
