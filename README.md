# Exam System API

A RESTful ASP.NET Core Web API that powers the Exam System platform. This backend provides endpoints for student and admin operations including user authentication, exam creation, question management, and exam submission.

## 🌐 Live Demo

🔗 [Live UI on GitHub Pages](https://ebrahimshabann.github.io/ExamSystemUI)  
🔗 [Backend API (Hosted on MonsterASP)](https://exampro.runasp.net/swagger/index.html)
---

## 🛠️ Tech Stack

- ASP.NET Core 6.0 / 7.0
- Entity Framework Core (Code First)
- SQL Server (Hosted on MonsterASP.NET)
- JWT Authentication
- CORS Configuration for GitHub Pages frontend

---

## 📦 Features

### 🔐 Authentication & Authorization
- User registration & login
- JWT-based token authentication
- Role-based access control (Teacher / Student)

### 📋 Exam Management
- Teacher can:
  - Create/Edit/Delete Exams
  - Add various question types
- Students can:
  - Take assigned exams
  - Submit answers
  - View results

### 📊 Results & Scoring
- Auto-correction for multiple-choice questions

## Angular UI Repo
git clone https://github.com/EbrahimShabann/ExamSystemUI.git

## API Repo
git clone https://github.com/EbrahimShabann/Exam-System-API.git
