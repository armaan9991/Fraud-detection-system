Database Design
Entity Relationship Overview

Users
  |
  | 1-to-many
  v
Transactions
  |
  | 1-to-many
  v
FraudAlerts

Database Schema

Users Table

CREATE TABLE Users (
    UserId SERIAL PRIMARY KEY,
    FullName VARCHAR(100),
    Email VARCHAR(100) UNIQUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

Devices Table

CREATE TABLE Devices (
    DeviceId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(UserId),
    DeviceName VARCHAR(100),
    IPAddress VARCHAR(50),
    LastUsed TIMESTAMP
);

Transactions Table

CREATE TABLE Transactions (
    TransactionId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(UserId),
    Amount DECIMAL(12,2),
    Currency VARCHAR(10),
    Country VARCHAR(50),
    DeviceId INT REFERENCES Devices(DeviceId),
    TransactionTime TIMESTAMP,
    FraudProbability DECIMAL(5,2),
    Status VARCHAR(20)
);

FraudAlerts Table

CREATE TABLE FraudAlerts (
    AlertId SERIAL PRIMARY KEY,
    TransactionId INT REFERENCES Transactions(TransactionId),
    RiskLevel VARCHAR(20),
    Reason TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

AuditLogs Table

CREATE TABLE AuditLogs (
    LogId SERIAL PRIMARY KEY,
    Action VARCHAR(100),
    PerformedBy VARCHAR(100),
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);