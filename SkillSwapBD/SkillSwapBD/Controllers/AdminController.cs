using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwapBD.Data;
using SkillSwapBD.Models;

namespace SkillSwapBD.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.SkillCount = await _context.Skills.CountAsync();
            ViewBag.PendingCount = await _context.Users.CountAsync(u => !u.IsApproved && !u.IsRejected);
            ViewBag.PendingSkillCount = await _context.Skills.CountAsync(s => !s.IsApproved);

            // Extra stats
            ViewBag.SwapRequestCount = await _context.SwapRequests.CountAsync();
            ViewBag.CompletedSwapCount = await _context.SwapRequests
                .CountAsync(sr => sr.Status == Enums.SwapStatus.Completed);
            ViewBag.ReviewCount = await _context.Reviews.CountAsync();
            ViewBag.AverageRating = await _context.Reviews.AnyAsync()
                ? Math.Round(await _context.Reviews.AverageAsync(r => r.Rating), 1)
                : 0;

            return View();
        }

        // ---------- Pending Users (existing) ----------
        public async Task<IActionResult> PendingUsers()
        {
            var pending = await _context.Users
                .Where(u => !u.IsApproved && !u.IsRejected)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsApproved = true;
            user.IsRejected = false;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"{user.FullName} has been approved.";
            return RedirectToAction(nameof(PendingUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsRejected = true;
            user.IsApproved = false;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"{user.FullName} has been rejected.";
            return RedirectToAction(nameof(PendingUsers));
        }

        // ---------- All Users (NEW) ----------
        public async Task<IActionResult> AllUsers(string? searchTerm)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email!.Contains(searchTerm));

            var users = await query.OrderBy(u => u.FullName).ToListAsync();

            // Skill count per user, loaded separately to avoid N+1
            var skillCounts = await _context.Skills
                .GroupBy(s => s.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            ViewBag.SkillCounts = skillCounts;
            ViewBag.SearchTerm = searchTerm;

            return View(users);
        }

        // GET: confirm delete
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You can't delete your own account from here.";
                return RedirectToAction(nameof(AllUsers));
            }

            ViewBag.SkillCount = await _context.Skills.CountAsync(s => s.UserId == id);
            ViewBag.SwapRequestCount = await _context.SwapRequests
                .CountAsync(sr => sr.RequesterId == id || sr.ReceiverId == id);

            return View(user);
        }

        // POST: actually delete
        [HttpPost, ActionName("DeleteUser"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You can't delete your own account from here.";
                return RedirectToAction(nameof(AllUsers));
            }

            // Manually clean up dependent records first to avoid FK constraint failures.
            // Adjust/remove any block below that doesn't match your actual FK setup.
            var userSkillIds = await _context.Skills
                .Where(s => s.UserId == id)
                .Select(s => s.Id)
                .ToListAsync();

            _context.SkillLikes.RemoveRange(_context.SkillLikes.Where(l => l.UserId == id));
            _context.CommentLikes.RemoveRange(_context.CommentLikes.Where(l => l.UserId == id));
            _context.SkillComments.RemoveRange(_context.SkillComments.Where(c => c.UserId == id || userSkillIds.Contains(c.SkillId)));
            _context.Reviews.RemoveRange(_context.Reviews.Where(r => r.ReviewerId == id || r.RevieweeId == id));
            _context.SwapRequests.RemoveRange(_context.SwapRequests.Where(sr => sr.RequesterId == id || sr.ReceiverId == id));
            _context.Skills.RemoveRange(_context.Skills.Where(s => s.UserId == id));

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Could not delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(AllUsers));
            }

            TempData["Success"] = $"{user.FullName} and all associated data have been deleted.";
            return RedirectToAction(nameof(AllUsers));
        }

        // ---------- Pending Skills (existing) ----------
        public async Task<IActionResult> PendingSkills()
        {
            var pending = await _context.Skills
                .Include(s => s.Category).Include(s => s.User)
                .Where(s => !s.IsApproved)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();
            return View(pending);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            skill.IsApproved = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Skill approved and is now live.";
            return RedirectToAction(nameof(PendingSkills));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Skill rejected and removed.";
            return RedirectToAction(nameof(PendingSkills));
        }

        // ---------- All Skills (NEW) ----------
        public async Task<IActionResult> AllSkills(string? searchTerm)
        {
            var query = _context.Skills.Include(s => s.Category).Include(s => s.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(s => s.Title.Contains(searchTerm));

            var skills = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
            ViewBag.SearchTerm = searchTerm;

            return View(skills);
        }

        // GET: confirm delete (for any skill, approved or not)
        public async Task<IActionResult> DeleteSkillAdmin(int id)
        {
            var skill = await _context.Skills.Include(s => s.Category).Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null) return NotFound();
            return View(skill);
        }

        [HttpPost, ActionName("DeleteSkillAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSkillAdminConfirmed(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();

            _context.SkillLikes.RemoveRange(_context.SkillLikes.Where(l => l.SkillId == id));
            _context.SkillComments.RemoveRange(_context.SkillComments.Where(c => c.SkillId == id));
            _context.Skills.Remove(skill);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Skill post deleted by admin.";
            return RedirectToAction(nameof(AllSkills));
        }
    }
}