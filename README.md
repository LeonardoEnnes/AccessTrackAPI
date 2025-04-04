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

##  📦 Installation & Setup
### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (v20.10+)
- [.NET SDK 9.0](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (optional - included in Docker setup)

1. Clone this repository
```shell
    git clone https://github.com/LeonardoEnnes/AccessTrackAPI.git
    cd AccessTrackAPI
```
2. Configure the enviroment variables:
    - Copy the [.env.example](.env.example) file to `.env`
   ```shell
      cp .env.example .env
    ```
    - Open the .env file and update the following values:
        - `DB_CONNECTION_STRING:` Your SQL Server connection string
        - `JWT_KEY:` A secure secret key for JWT token generation


3. Build and run the application using Docker:
 ```shell
    docker-compose up --build
   ```

4. The API should now be running at:
   [http://localhost:5130](http://localhost:5130)

### For Development (Without Docker): 
- Feel free to improve my code as you see fit. 
- If you have any questions regarding this project, Please feel free to email me.
1. Restore Dependencies & Build
```shell
    dotnet restore
    dotnet build
```

2. Configure User Secrets
```shell
  dotnet user-secrets
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-string-connection"
  dotnet user-secrets set "JwtKey" "your-jwt-key"
```

3. Apply Database Migrations
````shell
  dotnet ef database update
````

4. Run the Application
````shell
  dotnet run
````

## 🛠️ Technologies Used
- .NET (C#)
- Entity Framework
- SQL Server 
- Docker

## 📜 License
This project is licensed under the [MIT License](LICENSE).

## 📧 Contact
- **Author**: Leonardo Ennes
- **Email**: leonardoennes@protonmail.com