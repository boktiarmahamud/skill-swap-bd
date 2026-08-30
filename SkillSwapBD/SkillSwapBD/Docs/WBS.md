# Work Breakdown Structure — Skill Swap BD

## 1. Project Setup
- 1.1 Create solution `SkillSwapBD` in Visual Studio
- 1.2 Install NuGet packages (EF Core, Identity, SQL Server provider)
- 1.3 Configure connection string (SQL Server 2022)
- 1.4 Set up folder structure (Models, ViewModels, Controllers, Views, Data)

## 2. Database Design ✅ Done
- 2.1 Define entities: ApplicationUser, Category, Skill, SwapRequest, Review
- 2.2 Configure relationships & cascade rules in ApplicationDbContext
- 2.3 Run initial migration, verify tables in SSMS
- 2.4 Seed categories

## 3. Authentication & Roles ✅ Done
- 3.1 Build custom AccountController (Register, Login, Logout)
- 3.2 Configure Identity cookie auth
- 3.3 Add Identity Roles (Admin, User); auto-assign "User" on registration
- 3.4 Seed Admin account via DbInitializer

## 4. Admin Approval Workflow ✅ Done
- 4.1 Add IsApproved / IsRejected fields + migration
- 4.2 Build AdminController: PendingUsers, Approve, Reject
- 4.3 Pending-approval banner in layout (hidden for admins)
- 4.4 One-time approved message on login; rejection sign-out flow

## 5. User Profile ✅ Done
- 5.1 Add ProfileImageUrl, CoverImageUrl fields + migration
- 5.2 ProfileController: Index (own/other), Edit
- 5.3 Views: Index (full profile), PendingProfile (own, unapproved), LimitedProfile (others, unapproved), Edit
- 5.4 Profile photo + cover photo upload with validation
- 5.5 Styling (profile.css)

## 6. Skill Posting ⏳ Next
- 6.1 SkillsController: Create, Edit, Delete, MySkills
- 6.2 Enforce approval gate on Create
- 6.3 Views: Create, Edit, Delete, MySkills
- 6.4 Testing: approved vs unapproved posting behavior

## 7. Browse & Search ⏳ Pending
- 7.1 SkillsController.Index with category/type/keyword filters
- 7.2 Views: Index (card grid), Details
- 7.3 Link skill owner to their profile page
- 7.4 Guest access testing (no login required)

## 8. Swap Requests ⏳ Pending
- 8.1 SwapRequestsController: Create, Accept, Reject, Cancel, Complete
- 8.2 Views: Create, Index (incoming/outgoing tabs), Details
- 8.3 Status transition testing (Pending→Accepted/Rejected→Completed/Cancelled)

## 9. Reviews & Ratings ⏳ Pending
- 9.1 AddReview action in SwapRequestsController
- 9.2 Review form in SwapRequests/Details (post-completion only)
- 9.3 Average rating reflected on profile page

## 10. Final Integration & Testing ⏳ Pending
- 10.1 Full walkthrough: Register → Approval → Post → Browse → Swap → Review → Profile
- 10.2 Verify all [Authorize] / [Authorize(Roles="Admin")] placements
- 10.3 Final migration check (no pending model changes)
- 10.4 UI polish pass across all pages