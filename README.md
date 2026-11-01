# User Management API

A minimal ASP.NET Core API with in-memory user storage, request validation, bearer-token authentication, error handling, and request logging. User data is lost when the process stops.

## Run locally

In PowerShell, from the project directory:

```powershell
$env:Auth__Token = [guid]::NewGuid().ToString("N")
dotnet run
```

Keep the generated token private. The app requires `Auth__Token` to be set at startup and does not include a shared token in this public repository. For the `.http` examples, copy the same value into the `@token` variable in `UserManagementAPI.http`.

The default HTTP address is `http://localhost:5290`. Send the token on protected requests:

```http
Authorization: Bearer <your-token>
```

## Endpoints

| Method | Path | Description | Success |
|---|---|---|---|
| GET | `/users` | List users | 200 |
| GET | `/users/{id}` | Get a user | 200; 404 if missing |
| POST | `/users` | Create a user | 201 with `Location`; 400 for invalid input |
| PUT | `/users/{id}` | Replace a user's fields | 200; 404 if missing; 400 for invalid input |
| DELETE | `/users/{id}` | Delete a user | 204; 404 if missing |
| GET | `/weatherforecast` | Sample weather forecast | 200 |
| GET | `/openapi/v1.json` | OpenAPI document (Development only) | 200 |

All routes require a valid bearer token. Requests without one receive 401. Create/update bodies require a name (up to 100 characters), valid email address (up to 254 characters), and department (up to 100 characters).

## Example requests

```http
GET http://localhost:5290/users
Authorization: Bearer <your-token>

###

POST http://localhost:5290/users
Authorization: Bearer <your-token>
Content-Type: application/json

{
  "name": "Alex Johnson",
  "email": "alex.johnson@example.com",
  "department": "Engineering"
}
```

See [UserManagementAPI.http](./UserManagementAPI.http) for examples covering every user endpoint, invalid input, and unauthenticated access.

## Verification evidence

The local middleware smoke-check summary is available in [docs/middleware-verification.html](./docs/middleware-verification.html), with a captured [PNG screenshot](./docs/middleware-verification.png).

## Middleware

The pipeline registers error handling, bearer-token authentication, and request logging in that order. Configure the token through an environment variable or another secret provider; never commit production credentials.
