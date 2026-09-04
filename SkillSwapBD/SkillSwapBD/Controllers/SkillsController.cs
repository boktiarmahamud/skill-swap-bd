using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwapBD.Data;
using SkillSwapBD.Models;
using SkillSwapBD.ViewModels;
using static SkillSwapBD.Models.Enums;

namespace SkillSwapBD.Controllers
{
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public SkillsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // Public browse — only approved skills
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, SkillType? type)
        {
            var query = _context.Skills
                .Include(s => s.Category).Include(s => s.User).Include(s => s.Likes)
                .Where(s => s.IsApproved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(s => s.Title.Contains(searchTerm) || s.Description.Contains(searchTerm));
            if (categoryId.HasValue) query = query.Where(s => s.CategoryId == categoryId.Value);
            if (type.HasValue) query = query.Where(s => s.Type == type.Value);

            var vm = new SkillListViewModel
            {
                Skills = await query.OrderByDescending(s => s.CreatedAt).ToListAsync(),
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Type = type
            };
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var skill = await _context.Skills
                .Include(s => s.Category).Include(s => s.User)
                .Include(s => s.Likes)
                .Include(s => s.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .Include(s => s.Comments)
                    .ThenInclude(c => c.Likes)               
                .Include(s => s.Comments)
                    .ThenInclude(c => c.Replies)
                        .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            ViewBag.HasLiked = currentUserId != null && skill.Likes.Any(l => l.UserId == currentUserId);
            ViewBag.LikeCount = skill.Likes.Count;

            return View(skill);
        }

        [Authorize]
        public async Task<IActionResult> MySkills()
        {
            var userId = _userManager.GetUserId(User);
            var skills = await _context.Skills
                .Include(s => s.Category).Include(s => s.Likes)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(skills);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval. You can't post skills yet.";
                return RedirectToAction("Index", "Home");
            }

            var vm = new SkillFormViewModel { Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync() };
            return View(vm);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SkillFormViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval. You can't post skills yet.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                return View(vm);
            }

            var skill = new Skill
            {
                Title = vm.Title,
                Description = vm.Description,
                Type = vm.Type,
                CategoryId = vm.CategoryId,
                UserId = user.Id,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            // Attachment is optional
            if (vm.Attachment != null && vm.Attachment.Length > 0)
            {
                var (url, type, error) = await SaveAttachment(vm.Attachment, user.Id);
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error);
                    vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                    return View(vm);
                }
                skill.AttachmentUrl = url;
                skill.AttachmentType = type;
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Skill submitted! It will be visible once an admin approves it.";
            return RedirectToAction(nameof(MySkills));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            if (skill.UserId != _userManager.GetUserId(User)) return Forbid();

            var vm = new SkillFormViewModel
            {
                Id = skill.Id,
                Title = skill.Title,
                Description = skill.Description,
                Type = skill.Type,
                CategoryId = skill.CategoryId,
                ExistingAttachmentUrl = skill.AttachmentUrl,
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SkillFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            if (skill.UserId != _userManager.GetUserId(User)) return Forbid();

            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                return View(vm);
            }

            skill.Title = vm.Title;
            skill.Description = vm.Description;
            skill.Type = vm.Type;
            skill.CategoryId = vm.CategoryId;
            skill.IsApproved = false;

            if (vm.Attachment != null && vm.Attachment.Length > 0)
            {
                var (url, type, error) = await SaveAttachment(vm.Attachment, skill.UserId);
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error);
                    vm.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                    return View(vm);
                }
                skill.AttachmentUrl = url;
                skill.AttachmentType = type;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Skill updated — pending re-approval.";
            return RedirectToAction(nameof(MySkills));
        }

        // Helper: saves image or document, returns (url, type, errorMessage)
        private async Task<(string? url, string? type, string? error)> SaveAttachment(IFormFile file, string userId)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var docExtensions = new[] { ".pdf", ".doc", ".docx" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            bool isImage = imageExtensions.Contains(ext);
            bool isDoc = docExtensions.Contains(ext);

            if (!isImage && !isDoc)
                return (null, null, "Only images (JPG/PNG/WEBP) or documents (PDF/DOC/DOCX) are allowed.");

            if (file.Length > 5 * 1024 * 1024) // 5MB
                return (null, null, "File must be smaller than 5MB.");

            var folder = Path.Combine(_env.WebRootPath, "uploads", "skills");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return ($"/uploads/skills/{fileName}", isImage ? "image" : "document", null);
        }


        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null) return NotFound();
            if (skill.UserId != _userManager.GetUserId(User)) return Forbid();
            return View(skill);
        }

        [HttpPost, ActionName("Delete"), Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            if (skill.UserId != _userManager.GetUserId(User)) return Forbid();

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Skill post deleted.";
            return RedirectToAction(nameof(MySkills));
        }



        // ---------- Comment Likes ----------
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCommentLike(int commentId, int skillId)
        {
            var userId = _userManager.GetUserId(User)!;
            var existing = await _context.CommentLikes
                .FirstOrDefaultAsync(cl => cl.SkillCommentId == commentId && cl.UserId == userId);

            if (existing != null)
                _context.CommentLikes.Remove(existing);
            else
                _context.CommentLikes.Add(new CommentLike { SkillCommentId = commentId, UserId = userId });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = skillId });
        }

        // ---------- Likes ----------
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int skillId)
        {
            var userId = _userManager.GetUserId(User)!;
            var existing = await _context.SkillLikes
                .FirstOrDefaultAsync(l => l.SkillId == skillId && l.UserId == userId);

            if (existing != null)
                _context.SkillLikes.Remove(existing);
            else
                _context.SkillLikes.Add(new SkillLike { SkillId = skillId, UserId = userId });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = skillId });
        }

        // ---------- Comments ----------
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int skillId, string content, int? parentCommentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval. You can't comment yet.";
                return RedirectToAction(nameof(Details), new { id = skillId });
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment can't be empty.";
                return RedirectToAction(nameof(Details), new { id = skillId });
            }

            _context.SkillComments.Add(new SkillComment
            {
                SkillId = skillId,
                UserId = user.Id,
                Content = content,
                ParentCommentId = parentCommentId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = skillId });
        }
    }
}
