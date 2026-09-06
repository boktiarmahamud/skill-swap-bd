# Software Requirements Specification — Skill Swap BD

## 1. Introduction
**Purpose:** Defines requirements for Skill Swap BD, a web platform where users trade skills with each other instead of paying money.
**Scope:** Registration/login, admin approval workflow, skill posting (with approval + attachments), likes/comments, browsing/search, swap requests, reviews, user profiles.
**Tech Stack:** ASP.NET Core MVC, Entity Framework Core, SQL Server 2022, Bootstrap 5, ASP.NET Core Identity (cookie-based auth, custom AccountController).

## 2. Overall Description
Actors: **Guest** (browse only), **User** (registered, approved to post/swap/comment), **Admin** (approves/rejects users and skill posts).
New users register and are held **Pending** until Admin approval. Pending users can log in and edit their own profile but cannot post skills, comment, or request swaps. Every new/edited skill post also requires separate Admin approval before it appears publicly.

## 3. Functional Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| FR-1 | Register, log in, log out (Identity cookie auth) | High | Done |
| FR-2 | Auto-assign "User" role via ASP.NET Identity roles | High | Done |
| FR-3 | New users start unapproved (IsApproved=false); pending banner shown | High | Done |
| FR-4 | Admin views pending users, Approves or Rejects each | High | Done |
| FR-5 | One-time approval message on login; rejected users signed out with message | High | Done |
| FR-6 | User views/edits own profile: name, bio, location, avatar, cover photo | High | Done |
| FR-7 | Unapproved users see limited/pending profile view; approved users see full profile | High | Done |
| FR-8 | User creates/edits/deletes own skill posts (Offer/Want, category) | High | Done |
| FR-9 | Skill posts require separate Admin approval before appearing on Browse; edits require re-approval | High | Done |
| FR-10 | Approval gate enforced on skill posting for unapproved users | High | Done |
| FR-11 | User can attach an optional image or document (PDF/DOC) to a skill post | Medium | Done |
| FR-12 | Guests and users can browse/search approved skills by keyword, category, type | High | Done |
| FR-13 | Users can like/unlike a skill post; like count shown | Medium | Done |
| FR-14 | Users can comment on a skill post, with one level of threaded replies | Medium | Done |
| FR-15 | Users can like/unlike individual comments | Low | Done |
| FR-16 | Comment and reply author names link to their profile | Low | Done |
| FR-17 | User can request a swap, offering one of their skills for another's | High | Pending |
| FR-18 | Receiver Accepts/Rejects; requester Cancels; either marks Completed | High | Pending |
| FR-19 | After a completed swap, either party leaves a 1–5 star rating + comment | High | Pending |
| FR-20 | Average rating computed and shown on profile | Medium | Pending |

## 4. Non-Functional Requirements
- **Security:** Passwords hashed via Identity; anti-forgery tokens on all forms; role-based `[Authorize]` guards; uploaded files validated by type/size (images ≤2–5MB, docs PDF/DOC/DOCX).
- **Performance:** Search/browse results under 2 seconds at demo scale.
- **Usability:** Core actions reachable within 3 clicks; Facebook-style familiar comment/like UI; consistent Bootstrap-based styling.
- **Reliability:** FK constraints preserve data integrity; friendly error handling; unique constraints prevent duplicate likes.
- **Maintainability:** MVC separation of concerns; EF Core migrations only; reusable helper methods (e.g. `SaveAttachment`).

## 5. Data Model (Summary)
`ApplicationUser` (Identity + FullName, Bio, Location, ProfileImageUrl, CoverImageUrl, IsApproved, IsRejected) · `Category` · `Skill` (Offer/Want, IsApproved, AttachmentUrl/Type, linked to User + Category) · `SkillLike` · `SkillComment` (self-referencing for replies) · `CommentLike` · `SwapRequest` (links 2 users + 2 skills, status enum) · `Review` (1–5 rating, tied to completed swap). Roles (`Admin`, `User`) via `AspNetRoles`/`AspNetUserRoles`.

## 6. Out of Scope (Future Work)
In-app messaging, notifications, pagination, location-based matching, multi-level reply nesting beyond one level.