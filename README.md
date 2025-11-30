# University Payment System
This project developed for Yasar University - SE4458 Midterm.  
With this system, students can check the status of their tuition fees via mobile apps, banks, and online banking. APIs have been created for the university.

## Description
This UTPS API allows
* Query Tuition from the mobile app
* Query Tuition from the banking app
* Pay Tuition from the banking app
* University web site admin adds a tuition amount for given student term
* University web site admin adds a tuition amount from a .csv of student data
* University web site admin lists students with unpaid tuition amounts 

## Tech Stack
Tech stacks I use when developing
* **Framework:** ASP.NET Core Web API (.NET 10)
* **Language:** C#
* **Database:** Azure PostgreSQL Flexiable Service
* **API Documentation:** Swagger UI
* **Deployment:** Azure App Service
* **API Gateway:** Azure API Management
* **Authentication:** JWT
  

## Design
```
UniversityPaymentSystem/
│
├── UniversityPaymentSystem.Api/
│   └── Configuration/
│       └── GatewayUrlDocumentFilter.cs
│   └── Controllers/
│       └── AuthController.cs
│       └── TuitionController.cs
│
├── UniversityPaymentSystem.Application/
│   └── Interfaces/
│       └── IAuthService.cs
│       └── ITuitionService.cs
│
│   └── Services/
│       └── AuthService.cs
│       └── TuitionService.cs
│
├── UniversityPaymentSystem.Domain/
│   └── DTOs/
│       └── AddStudentRequestDto.cs
│       └── AddTuitionBatchRequest.cs
│       └── AddTuitionRequestDto.cs
│       └── PayTuitionRequestDto.cs
│       └── TuitionQueryResponseDto.cs
│       └── UserLoginDto.cs
│
│   └── Entities/
│       └── Student.cs
│       └── TuitionPayment.cs
│
├── UniversityPaymentSystem.Infrastructure/
│   └── Data/
│       └── ApplicationDbContext.cs
│
│   └── Migrations/
│       └── InitialCreate.cs
│       └── ApplicationDbContextModelSnapshot.cs
│
│   └── Repositories/
│       └── IStudentRepository.cs
│       └── StudentRepository.cs
│
```

## API Endpoints 
| **Method** |           **Endpoint**            |          **Parameters**          |    **API Response**    | **Authentication**  | **Paging**  |                **Description**                    | 
|------------|-----------------------------------|----------------------------------|------------------------|---------------------|-------------|---------------------------------------------------|
|    POST    |       /api/v1/Auth/login          |       Username,password          |         Token          |         No          |     No      |     To get tokens to implement authentication     |
|    POST    | /api/v1/Tuition/admin/student/add | Student No, Full Name, TC number |   Transaction Status   |         Yes         |     No      |          To add a student to the database         |
|    GET     |  /api/v1/Tuition/mobileApp/query  |            Student No            | Tuition Total, Balance |         No          |     No      |    Returns tuition amount and current balance     |
|    GET     |  /api/v1/Tuition/bankingApp/query |            Student No            | Tuition Total, Balance |         Yes         |     No      |    Returns tuition amount and current balance     |
|    POST    |  /api/v1/Tuition/bankingApp/pay   |     Student No, Term, Amount     |     Payment Status     |         No          |     No      |          Records payment for given term           |
|    POST    |    /api/v1/Tuition/admin/add      | Student No, Term, Tuition Amount |   Transaction Status   |         Yes         |     No      |   Adds a tuition amount for given student term    |
|    POST    |  /api/v1/Tuition/admin/add/batch  |  csv files of Student No, Term   |   Transaction Status   |         Yes         |     No      | Adds a tuition amount from a .csv of student data |
|    GET     |   /api/v1/Tuition/admin/unpaid    |   Term, Page Number, Page Size   |          List          |         Yes         |     Yes     |   List of students with unpaid tuition amounts    |


## Data Model
<img width="406" height="288" alt="image" src="https://github.com/user-attachments/assets/022382ee-bc06-454a-bcf9-1e30f4cd54a2" />

## Presentation Video
[Drive](https://drive.google.com/file/d/1JcUGM0_0CrpekZefZyfRR77YzZZI5Yvo/view?usp=sharing)

## Deployment
[Swagger](https://universitypaymentsystem-hka6d7b5gvf4c7c4.francecentral-01.azurewebsites.net/index.html)

## Source Code
[Github](https://github.com/eecegunguney/UniversityPaymentSystem)

## Assumptions
* BankingApp/pay and Admin/add endpoints must take amount as parameter because system needs to know amount to give total tuition total and balance in mobilApp/query and bankingApp/query.
* Paging and rate limits will be on Api Gateway (Azure Api Management).
* There are two role for authentication: Admin for /admin/ endpoints, Banking for /bankingApp/query endpoint.
* MobileApp/query limit calls are limited to 3 students per day.
  

## Issues Encountered
* There were package conflicts. I used version fixes, dotnet clean, dotnet restore.
* I had a problem sending API management during the deploy, so I changed the deployment mode and deployed the application separately and gave the APIs to API management separately.
* When I used the gateway, I received Access denied, so I removed the key request from the gateway.
