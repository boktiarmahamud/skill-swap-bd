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

            // Browse by Category — top categories by approved skill count
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    SkillCount = c.Skills.Count(s => s.IsApproved)
                })
                .Where(c => c.SkillCount > 0)
                .OrderByDescending(c => c.SkillCount)
                .Take(8)
                .ToListAsync();

            ViewBag.Categories = categories;

            // Top-rated members — approved users with at least 1 review
            var topRated = await _context.Reviews
                .GroupBy(r => r.RevieweeId)
                .Select(g => new
                {
                    UserId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    ReviewCount = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.ReviewCount)
                .Take(4)
                .ToListAsync();

            var topRatedUserIds = topRated.Select(t => t.UserId).ToList();
            var topRatedUsers = await _context.Users
                .Where(u => topRatedUserIds.Contains(u.Id) && u.IsApproved)
                .ToListAsync();

            var topRatedCombined = topRated
                .Join(topRatedUsers, t => t.UserId, u => u.Id, (t, u) => new
                {
                    User = u,
                    t.AverageRating,
                    t.ReviewCount
                })
                .OrderByDescending(x => x.AverageRating)
                .ToList();

            ViewBag.TopRatedMembers = topRatedCombined;

            return View(latestSkills);
        }

        public IActionResult Privacy() => View();
    }
}