# ScaleTrack API Responses

This document shows **API requests and responses** for ScaleTrack entities.

---

## Authentication & Roles

This section describes login, logout, token refresh, user roles, and API responses for ScaleTrack.

### User Roles
- **Admin** – Full access: create, update, delete issues and tags; manage users.
- **User** – Limited access: create and comment on issues; view tags.

### Login (POST `/api/v1/auth/login`)

**Request Body**
```json
{
  "email": "thabelo@example.com",
  "password": "yourPassword123"
}
```

**Response (200 OK)**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "dGhpc0lzUmVmcmVzaFRva2VuMTIz",
  "user": {
    "id": 1,
    "FirstName": "Thabelo",
    "LastName": "Libago",
    "email": "thabelo@example.com",
    "role": "Admin"
  }
}
```

**Response (401 Unauthorized)**
```json
{
  "message": "Invalid email or password."
}
```

### Register (POST `/api/v1/auth/register`)

**Request Body**
```json
{
  "FirstName": "Thabelo",
  "LastName": "Libago",
  "email": "thabelo@example.com",
  "password": "yourPassword123",
  "role": "Developer"
}
```

**Response (200 OK)**
```json
{
  "id": 1,
  "FirstName": "Thabelo",
  "LastName": "Libago",
  "email": "thabelo@example.com",
  "role": "Developer"
}
```

### Refresh Token (POST `/api/v1/auth/refresh`)

**Request Body**
```json
{
  "refreshToken": "dGhpc0lzUmVmcmVzaFRva2VuMTIz"
}
```

**Response (200 OK)**
```json
{
  "token": "newAccessTokenHere",
  "refreshToken": "newRefreshTokenHere"
}
```

**Response (401 Unauthorized)**
```json
{
  "message": "Invalid or expired refresh token."
}
```

### Logout (POST `/api/v1/auth/logout`)

**Request Body**
```json
{
  "refreshToken": "dGhpc0lzUmVmcmVzaFRva2VuMTIz"
}
```

**Response (200 OK)**
```json
{
  "message": "Successfully logged out."
}
```

**Response (401 Unauthorized)**
```json
{
  "message": "Invalid token."
}
```

---

## Issue

> **Access:**  
> **Admin**: create, update, delete issues  
> **User**: create issues, comment on issues, view issues

### Create Issue (POST `/api/v1/issues/create`)

**Request Body**
```json
{
  "title": "Login button not working",
  "description": "The login button on the homepage does not respond to clicks.",
  "type": "Bug",
  "priority": "High"
}
```

**Response (201 Created)**
```json
{
  "id": 1,
  "title": "Login button not working",
  "description": "The login button on the homepage does not respond to clicks.",
  "type": "Bug",
  "priority": "High",
  "status": "Open",
  "createdAt": "2025-08-26T10:00:00Z",
  "updatedAt": null
}
```

### Get All Issues (GET `/api/v1/issues`)

**Response (200 OK)**
```json
[
  {
    "id": 1,
    "title": "Login button not working",
    "description": "The login button on the homepage does not respond to clicks.",
    "type": "Bug",
    "priority": "High",
    "status": "Open",
    "createdAt": "2025-08-26T10:00:00Z",
    "updatedAt": null
  },
  {
    "id": 2,
    "title": "Add dark mode toggle",
    "description": "Users requested a dark mode toggle in settings.",
    "type": "Feature Request",
    "priority": "Medium",
    "status": "In Progress",
    "createdAt": "2025-08-26T11:00:00Z",
    "updatedAt": "2025-08-26T12:00:00Z"
  }
]
```

### Update Issue Status (PUT `/api/v1/issues/{id}/status`)

> **Access:** Admin only

**Request Body**
```json
{
  "title": "Login button not working",
  "description": "Updated description for the login issue.",
  "type": "Bug",
  "priority": "High"
}

```

**Response (200 OK)**
```json
{
  "id": 1,
  "title": "Login button not working",
  "description": "The login button on the homepage does not respond to clicks.",
  "type": "Bug",
  "priority": "High",
  "status": "Resolved",
  "createdAt": "2025-08-26T10:00:00Z",
  "updatedAt": "2025-08-26T13:00:00Z"
}
```

### Update Issue Status (PATCH `/api/v1/issues/{id}/status`)

> **Access:** Admin only

**Request Body**
```json
{
"Resolved"
}

```

**Response (200 OK)**
```json
{
  "id": 1,
  "title": "Login button not working",
  "description": "The login button on the homepage does not respond to clicks.",
  "type": "Bug",
  "priority": "High",
  "status": "Resolved",
  "createdAt": "2025-08-26T10:00:00Z",
  "updatedAt": "2025-08-26T13:00:00Z"
}
```

---

## User

> **Access:**  
> **Admin**: create and manage users  
> **User**: cannot create or delete users

### Create User (POST `/api/v1/users`)

**Request Body**
```json
{
  "FirstName": "Thabelo",
  "LastName": "Libago",
  "email": "thabelo@example.com",
  "role": "Developer"
}
```

**Response (201 Created)**
```json
{
  "id": 1,
  "FirstName": "Thabelo Libago",
  "LastName": "Libago",
  "email": "thabelo@example.com",
  "role": "Developer"
}
```

---

## Comment

> **Access:**  
> **Admin/User**: add comments to issues

### Add Comment to Issue (POST `/api/v1/issues/{id}/comments`)

**Request Body**
```json
{
  "userId": 1,
  "content": "This issue needs urgent fixing."
}
```

**Response (201 Created)**
```json
{
  "id": 1,
  "issueId": 1,
  "userId": 1,
  "content": "This issue needs urgent fixing.",
  "createdAt": "2025-08-26T14:00:00Z"
}
```

---

## Tag

> **Access:**  
> **Admin**: create tags and assign to issues  
> **User**: view tags

### Create Tag (POST `/api/v1/tags`)

**Request Body**
```json
{
  "name": "UI"
}
```

**Response (201 Created)**
```json
{
  "id": 1,
  "name": "UI"
}
```

### Assign Tag to Issue (POST `/api/v1/issues/{id}/tags`)

> **Access:** Admin only

**Request Body**
```json
{
  "tagId": 1
}
```

**Response (200 OK)**
```json
{
  "issueId": 1,
  "tagId": 1
}
```

