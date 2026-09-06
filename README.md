# Skill Swap BD
Web app to swap skills with others instead of paying — e.g. trade guitar lessons for web dev help.

## Tech Stack
ASP.NET Core MVC · EF Core · SQL Server · Bootstrap 5 · ASP.NET Identity (cookie auth, role-based)

## Features
- Register / Login / Logout
- Role-based access (Admin, User) via ASP.NET Identity roles
- Admin approval workflow — new accounts are pending until an Admin approves or rejects them
- User profiles — bio, location, profile photo, cover photo (view/edit)
- Post skills (Offer/Want) with an optional image or document attachment
- Skill posts require Admin approval before going live; edits require re-approval
- Browse & search approved skills by keyword, category, and type
- Like skill posts; comment with one level of threaded replies; like comments
- Admin dashboard — review pending users and pending skill posts
- 🚧 Request, accept/reject, complete swaps
- 🚧 Rate & review after a completed swap

## Setup
1. Clone repo
2. Update `appsettings.json` connection string (SQL Server 2022)
3. `dotnet ef database update`
4. `dotnet run`
5. Seeded admin login: `admin@skillswapbd.com` / `Admin@123`

## Status
🚧 In development — course project
Currently implemented: auth, roles, admin approval, profiles, skill posting with attachments, likes/comments.
Next up: swap requests and reviews.
