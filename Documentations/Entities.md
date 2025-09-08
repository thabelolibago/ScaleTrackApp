# ScaleTrack Entities

This document defines the core entities used in **ScaleTrack**, the Issue Tracker app.

---

## Issue

Represents a single tracked issue (bug, feature, gap, etc.).

| Field        | Type        | Description |
|--------------|------------|-------------|
| **Id**       | int (PK)   | Unique identifier for the issue |
| **Title**    | string     | Short summary of the issue |
| **Description** | string | Detailed explanation |
| **Type**     | string     | Category of issue: `Bug`, `Feature Request`, `Improvement`, `MVP Gap`, `Scaling Risk`, `Customer Feedback` |
| **Priority** | string     | Importance: `Low`, `Medium`, `High` |
| **Status**   | string     | Current state: `Open`, `In Progress`, `Resolved`, `Closed` |
| **CreatedAt**| datetime   | Timestamp when created |
| **UpdatedAt**| datetime   | Timestamp when last updated |

---

## User (optional for stretch goals)

Represents a person creating or managing issues.

| Field        | Type        | Description |
|--------------|------------|-------------|
| **Id**       | int (PK)   | Unique identifier for the user |
| **FirstName**| string     | First name |
| **LastName** | string     | Last name |
| **Email**    | string     | Contact email |
| **Role**     | string     | Role in system: `Admin`, `Developer`, `Viewer` |

---

## Comment (optional for stretch goals)

Represents discussion or updates on an issue.

| Field        | Type        | Description |
|--------------|------------|-------------|
| **Id**       | int (PK)   | Unique identifier for the comment |
| **IssueId**  | int (FK)   | Related issue |
| **UserId**   | int (FK)   | Author of the comment |
| **Content**  | string     | Comment text |
| **CreatedAt**| datetime   | Timestamp of comment creation |

---

## Tag (optional for stretch goals)

Used for categorizing or labeling issues.

| Field        | Type        | Description |
|--------------|------------|-------------|
| **Id**       | int (PK)   | Unique identifier for the tag |
| **Name**     | string     | Tag name (e.g., `UI`, `Backend`, `Database`) |

---

## IssueTag (join table for tags, optional)

Many-to-many relationship between Issues and Tags.

| Field        | Type        | Description |
|--------------|------------|-------------|
| **IssueId**  | int (FK)   | Related issue |
| **TagId**    | int (FK)   | Related tag |

---

## AuditTrail (for tracking changes)

Tracks changes across entities for accountability.

| Field         | Type        | Description |
|---------------|------------|-------------|
| **Id**        | int (PK)   | Unique identifier for the audit entry |
| **EntityName**| string     | Name of the entity affected (`Issue`, `User`, `Comment`, `Tag`) |
| **EntityId**  | int        | Id of the affected entity |
| **Action**    | string     | Type of action: `Create`, `Update`, `Delete` |
| **ChangedBy** | int (FK)   | UserId of who made the change |
| **ChangedAt** | datetime   | Timestamp of the change |
| **Changes**   | string     | JSON or text detailing what was changed |
| **ApprovedBy**| int (FK)   |  UserId of who approved the change |
| **ApprovedAt**| datetime   |  Timestamp of approval |
| **ApprovalStatus**| string |  Pending, Approved, Rejected |
