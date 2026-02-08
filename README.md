# Mobile Identity Verification API

A lightweight **ASP.NET Core Web API** for mobile identity verification.  
The system uses **Basic Authentication**, **Dapper**, and **SQL Server** to securely provide KYC-based customer information and account detail retrieval.

Built with a focus on **performance, security, and clean architecture**.

---

## 🚀 Features
- Basic Authentication for secure access
- KYC-based customer verification
- Retrieve customer profile information
- Fetch account details
- Fast data access using Dapper ORM
- RESTful API design
- Structured error handling & validation

---

## 🛠 Tech Stack
- ASP.NET Core Web API
- C#
- Dapper
- SQL Server
- Swagger / OpenAPI

---

## 📦 API Endpoints (Sample)
- `GET /api/customer/{id}` → Get customer information  
- `GET /api/accounts/{customerId}` → Get customer account details

---

## 🔐 Security
All endpoints are protected using **Basic Authentication**.  
Requests require valid credentials to access resources.

---

## ▶️ Getting Started

### Prerequisites
- .NET SDK
- SQL Server

