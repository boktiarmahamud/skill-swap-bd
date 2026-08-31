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
            ViewBag.PendingCount = await _context.Users.CountAsync(u => !u.IsApproved);
            ViewBag.PendingSkillCount = await _context.Skills.CountAsync(s => !s.IsApproved); 
            return View();
        }

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
    }
}
