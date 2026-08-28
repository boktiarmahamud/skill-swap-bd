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
    }
}
