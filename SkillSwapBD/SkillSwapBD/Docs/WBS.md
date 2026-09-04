# Work Breakdown Structure — Skill Swap BD

## 1. Project Setup ✅ Done
- 1.1 Solution, NuGet packages, connection string, folder structure

## 2. Database Design ✅ Done
- 2.1–2.4 Core entities, relationships, initial migration, category seed

## 3. Authentication & Roles ✅ Done
- 3.1–3.4 Custom AccountController, Identity cookie auth, roles, seeded Admin

## 4. Admin Approval Workflow (Users) ✅ Done
- 4.1–4.4 IsApproved/IsRejected fields, AdminController, pending banner, login messages

## 5. User Profile ✅ Done
- 5.1–5.5 ProfileImageUrl, CoverImageUrl, ProfileController, Index/Pending/Limited/Edit views, styling

## 6. Skill Posting ✅ Done
- 6.1 SkillsController: Create, Edit, Delete, MySkills
- 6.2 Approval gate enforced on Create
- 6.3 Views: Create, Edit, Delete, MySkills, Details
- 6.4 Skill-level admin approval (IsApproved on Skill + AdminController.PendingSkills/ApproveSkill/RejectSkill)
- 6.5 Edits require re-approval before staying live
- 6.6 Optional attachment upload (image or document) via SaveAttachment helper
- 6.7 Admin dashboard updated with pending-skill count + review link
- 6.8 Homepage shows latest approved skills

## 7. Likes & Comments ✅ Done
- 7.1 SkillLike model + ToggleLike action
- 7.2 SkillComment model with self-referencing replies + AddComment action
- 7.3 CommentLike model + ToggleCommentLike action
- 7.4 Facebook-style Details page: like button, comment thread, one-level replies
- 7.5 Comment/reply author names linked to profile pages

## 8. Browse & Search ✅ Done
- 8.1 SkillsController.Index with category/type/keyword filters (approved skills only)
- 8.2 Skills/Index (card grid), Details pages
- 8.3 Guest access verified (no login required)

## 9. Swap Requests ⏳ Next
- 9.1 SwapRequestsController: Create, Accept, Reject, Cancel, Complete
- 9.2 Views: Create, Index (incoming/outgoing tabs), Details
- 9.3 Status transition testing

## 10. Reviews & Ratings ⏳ Pending
- 10.1 AddReview action in SwapRequestsController
- 10.2 Review form in SwapRequests/Details (post-completion only)
- 10.3 Average rating reflected on profile page

## 11. Final Integration & Testing ⏳ Pending
- 11.1 Full walkthrough: Register → Approval → Post → Approve Post → Browse → Like/Comment → Swap → Review → Profile
- 11.2 Verify all [Authorize] / [Authorize(Roles="Admin")] placements
- 11.3 Final migration check (no pending model changes)
- 11.4 UI polish pass, .gitignore check for wwwroot/uploads/