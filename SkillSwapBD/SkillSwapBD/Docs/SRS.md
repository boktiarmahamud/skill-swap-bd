# Software Requirements Specification — Skill Swap BD

## 1. Introduction
**Purpose:** Defines requirements for Skill Swap BD, a web platform where users trade skills with each other instead of paying money.
**Scope:** User registration/login, admin approval workflow, skill posting, browsing/search, swap requests, reviews, and user profiles.
**Tech Stack:** ASP.NET Core MVC, Entity Framework Core, SQL Server 2022, Bootstrap 5, ASP.NET Core Identity (cookie-based auth, custom AccountController).

## 2. Overall Description
Actors: **Guest** (browse only), **User** (registered, approved to post/swap), **Admin** (approves/rejects users, manages platform).
New users register and are held in a **Pending** state until an Admin approves them. Pending users can log in and view/edit their own profile but cannot post skills, browse others' full profiles, or request swaps until approved.

## 3. Functional Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| FR-1 | User can register, log in, log out (Identity cookie auth) | High | Done |
| FR-2 | New users assigned "User" role automatically; roles via ASP.NET Identity | High | Done |
| FR-3 | New users start unapproved (IsApproved=false); pending banner shown | High | Done |
| FR-4 | Admin can view pending users, Approve or Reject each | High | Done |
| FR-5 | Approved user gets one-time success message; rejected user is signed out with message | High | Done |
| FR-6 | User can view their own profile (name, bio, location, avatar, cover photo) | High | Done |
| FR-7 | User can edit their own profile incl. profile photo and cover photo upload | High | Done |
| FR-8 | Unapproved users see a limited/pending profile view; approved users see full profile w/ skills & reviews | High | Done |
| FR-9 | User can create, edit, delete their own skill posts (Offer/Want, category) | High | Pending |
| FR-10 | Approval gate enforced on skill posting (unapproved users blocked) | High | Pending |
| FR-11 | Anyone (guest incl.) can browse/search skills by keyword, category, type | High | Pending |
| FR-12 | User can request a swap, offering one of their skills for another's | High | Pending |
| FR-13 | Receiver can Accept/Reject; requester can Cancel; either can mark Completed | High | Pending |
| FR-14 | After a completed swap, either party leaves a 1–5 star rating + comment | High | Pending |
| FR-15 | Average rating computed and shown on profile | Medium | Pending |

## 4. Non-Functional Requirements
- **Security:** Passwords hashed via Identity; anti-forgery tokens on all forms; role-based `[Authorize]` guards; image uploads validated by type/size.
- **Performance:** Search/browse results under 2 seconds at demo scale.
- **Usability:** Core actions reachable within 3 clicks; consistent Bootstrap-based UI; inline validation.
- **Reliability:** FK constraints preserve data integrity; friendly error handling.
- **Maintainability:** MVC separation of concerns; EF Core migrations only (no manual SQL edits).

## 5. Data Model (Summary)
`ApplicationUser` (Identity + FullName, Bio, Location, ProfileImageUrl, CoverImageUrl, IsApproved, IsRejected) · `Category` · `Skill` (Offer/Want, linked to User + Category) · `SwapRequest` (links 2 users + 2 skills, status enum) · `Review` (1–5 rating, tied to a completed swap). Roles (`Admin`, `User`) via `AspNetRoles`/`AspNetUserRoles`.

## 6. Out of Scope (Future Work)
In-app messaging, notifications, admin content moderation of skills, pagination, location-based matching.