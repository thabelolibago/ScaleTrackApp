# ScaleTrack API Controller Overview

## 1. AuthController – Authentication & Token Management
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/auth/login` | POST | Login with email & password → returns JWT & refresh token | Public |
| `/api/v1/auth/refresh` | POST | Refresh access token using refresh token | Public |
| `/api/v1/auth/logout` | POST | Invalidate refresh token / logout | Public |

## 2. MenuController – Role-Based Menu
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/menu` | GET | Returns filtered menu items based on the logged-in user's role (`Admin` or `User`) | Authenticated |

## 3. IssuesController – Manage Issues
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/issues` | GET | List all issues | Admin/User |
| `/api/v1/issues/{id}` | GET | Get single issue by ID | Admin/User |
| `/api/v1/issues` | POST | Create a new issue | Admin/User |
| `/api/v1/issues/{id}` | PUT | Replace entire issue | Admin |
| `/api/v1/issues/{id}/status` | PATCH | Update only the issue status | Admin |
| `/api/v1/issues/{id}` | DELETE | Delete issue | Admin |

## 4. UsersController – Manage Users
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/users` | GET | List all users | Admin |
| `/api/v1/users/{id}` | GET | Get single user | Admin |
| `/api/v1/users` | POST | Create a new user | Admin |
| `/api/v1/users/{id}` | PUT | Update entire user | Admin |
| `/api/v1/users/{id}` | PATCH | Update single fields of user (optional) | Admin |
| `/api/v1/users/{id}` | DELETE | Delete user | Admin |

## 5. CommentsController – Issue Comments
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/issues/{id}/comments` | GET | List all comments for an issue | Admin/User |
| `/api/v1/issues/{id}/comments` | POST | Add comment to issue | Admin/User |
| `/api/v1/comments/{id}` | DELETE | Delete comment (optional) | Admin |

## 6. TagsController – Manage Tags
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/tags` | GET | List all tags | Admin/User |
| `/api/v1/tags` | POST | Create a new tag | Admin |
| `/api/v1/tags/{id}` | DELETE | Delete tag | Admin |

## 7. IssueTagsController – Assign / Remove Tags
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/issues/{id}/tags` | POST | Assign a tag to an issue | Admin |
| `/api/v1/issues/{id}/tags/{tagId}` | DELETE | Remove a tag from an issue | Admin |

## 8. AuditTrailsController – Track Changes
| Endpoint | HTTP Method | Description | Access |
|----------|------------|-------------|--------|
| `/api/v1/audittrails` | GET | List all audit trail entries | Admin |
| `/api/v1/audittrails/{id}` | GET | Get single audit trail entry | Admin |

### Notes
1. Admin vs User Access:
   - Admin: full access to all endpoints.
   - User: restricted to viewing issues, adding issues, adding comments, and seeing menu.

2. PATCH vs PUT:
   - PATCH: partial updates (single fields)
   - PUT: full replacement of resource

3. Backend-Only Functionality:
   - MenuController dynamically filters menu items by role.
   - AuthController handles authentication entirely on the backend.

