# AccessTrackAPI

## 📌  Description
A secure access control system API for managing user authentication, authorization, and entry logging in buildings or institutions.

## 🚀 Features

### User Management
- User registration and authentication
- Role-based access control (Admin, User, Visitor)
- Password hashing for secure storage
- JWT token-based authentication

### Access Control
- Entry validation for users and visitors
- Exit recording with timestamps
- Comprehensive logging of all access events

### Admin Functionality
- Admin user creation and management
- User/visitor management (CRUD operations)
- Access log viewing and reporting

---
## API Endpoints

### 🔑 Authentication & Security
This API uses JWT authentication. To access protected routes, obtain a token via the login endpoint and include it in the request headers:
```bash
 Authorization: Bearer YOUR_ACCESS_TOKEN
```

### Authentication
- `POST /v1/Users/CreateUser` - Register a new regular user
- `POST /v1/Users/login` - Authenticate a user and get JWT token
- `POST /v1/Admin/LoginAdmin` - Authenticate an admin and get JWT token

### Admin Operations
- `POST /v1/Admin/CreateAdmin` - Create a new admin (requires admin role)
- `POST /v1/Admin/CreateFirstAdmin` - Initialize the first root admin
- `DELETE /v1/Admin/DeleteAdmin/{id}` - Delete an admin (root admin only)
- `DELETE /v1/Admin/DeleteUser/{id}` - Delete a user
- `PUT /v1/Admin/UpdateUser/{id}` - Update user information
- `GET /v1/Admin/UserLogs/{userId?}` - Get access logs (all or specific user)
- `GET /v1/Admin/VisitorLogs/{visitorId?}` - Get visitor access logs

### Visitor Management
- `POST /v1/visitors/CreateVisitor` - Create a new visitor (admin only)
- `DELETE /v1/visitors/DeleteVisitor/{id}` - Delete a visitor (admin only)

### Access Control
- `POST /v1/Entry/validate-entry` - Validate user/visitor entry

### User Operations
- `GET /v1/Users/infos` - Get user information and access logs
---
##  📦 Installation & Setup
### Prerequisites

1. Clone this repository
```shell
    git clone https://github.com/LeonardoEnnes/AccessTrackAPI.git
    cd AccessTrackAPI
```

## 🛠️ Technologies Used
- .NET (C#)
- Entity Framework
- SQL Server 
- Docker

## 📜 License
This project is licensed under the MIT License.

## 📧 Contact
- **Author**: Leonardo Ennes
- **Email**: leonardoennes@protonmail.com 













---
## To ensure the API functions correctly, please follow these steps:

### Initialize User Secrets
```sh
dotnet user-secrets init
```

### Set User Secrets
Use the `dotnet user-secrets set` command to add your secrets. For example:

```sh
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionStringHere"
dotnet user-secrets set "JwtKey" "YourJwtKeyHere"
```

### List User Secrets
To view all the stored User Secrets, use the following command:

```sh
dotnet user-secrets list
```

### Remove User Secrets
To remove a specific secret, use the following command:

```sh
dotnet user-secrets remove "SecretName"
```

## Notes
