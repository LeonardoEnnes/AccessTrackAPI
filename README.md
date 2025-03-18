# AccessTrackAPI

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
