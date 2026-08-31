using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwapBD.Data;
using SkillSwapBD.Models;
using System.Diagnostics;

namespace SkillSwapBD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var latestSkills = await _context.Skills
                .Include(s => s.Category)
                .Include(s => s.User)
                .Where(s => s.IsApproved)
                .OrderByDescending(s => s.CreatedAt)
                .Take(6)
                .ToListAsync();

            ViewBag.SkillCount = await _context.Skills.CountAsync(s => s.IsApproved);
            ViewBag.UserCount = await _context.Users.CountAsync(u => u.IsApproved);

            return View(latestSkills);
        }

        public IActionResult Privacy() => View();
    }
}
