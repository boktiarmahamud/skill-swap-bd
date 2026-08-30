using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwapBD.Data;
using SkillSwapBD.Models;
using SkillSwapBD.ViewModels;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;

namespace SkillSwapBD.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Profile/Index/{id} -- id omitted shows current user's own profile
        public async Task<IActionResult> Index(string? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var targetId = id ?? currentUser?.Id;
            if (targetId == null) return Challenge();

            var profileUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetId);
            if (profileUser == null) return NotFound();

            bool isOwnProfile = currentUser != null && currentUser.Id == targetId;
            bool isAdmin = User.IsInRole("Admin");
            bool isPending = !profileUser.IsApproved && !isAdmin;

            ViewBag.IsOwnProfile = isOwnProfile;

            if (isPending)
            {
                // Own profile while pending -> show pending screen WITH edit option
                // Someone else's pending profile -> show pending screen with NO edit option
                return View(isOwnProfile ? "PendingProfile" : "LimitedProfile", profileUser);
            }

            var skills = await _context.Skills
                .Include(s => s.Category)
                .Where(s => s.UserId == targetId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var reviews = await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.RevieweeId == targetId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Skills = skills;
            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;

            return View(profileUser); 
        }

        // GET: /Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var vm = new ProfileEditViewModel
            {
                FullName = user.FullName,
                Bio = user.Bio,
                Location = user.Location,
                ExistingImageUrl = user.ProfileImageUrl,
                ExistingCoverImageUrl = user.CoverImageUrl   
            };
            return View(vm);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.ExistingImageUrl = user.ProfileImageUrl;
                vm.ExistingCoverImageUrl = user.CoverImageUrl;
                return View(vm);
            }

            user.FullName = vm.FullName;
            user.Bio = vm.Bio;
            user.Location = vm.Location;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (vm.ProfileImage != null && vm.ProfileImage.Length > 0)
            {
                var ext = Path.GetExtension(vm.ProfileImage.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext) || vm.ProfileImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(string.Empty, "Profile photo must be JPG/PNG/WEBP under 2MB.");
                    vm.ExistingImageUrl = user.ProfileImageUrl;
                    vm.ExistingCoverImageUrl = user.CoverImageUrl;
                    return View(vm);
                }
                var folder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(folder);
                var fileName = $"{user.Id}{ext}";
                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await vm.ProfileImage.CopyToAsync(stream);
                user.ProfileImageUrl = $"/uploads/profiles/{fileName}";
            }

            // ADD: cover image handling
            if (vm.CoverImage != null && vm.CoverImage.Length > 0)
            {
                var ext = Path.GetExtension(vm.CoverImage.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext) || vm.CoverImage.Length > 4 * 1024 * 1024) 
                {
                    ModelState.AddModelError(string.Empty, "Cover photo must be JPG/PNG/WEBP under 4MB.");
                    vm.ExistingImageUrl = user.ProfileImageUrl;
                    vm.ExistingCoverImageUrl = user.CoverImageUrl;
                    return View(vm);
                }
                var folder = Path.Combine(_env.WebRootPath, "uploads", "covers");
                Directory.CreateDirectory(folder);
                var fileName = $"{user.Id}_cover{ext}";
                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await vm.CoverImage.CopyToAsync(stream);
                user.CoverImageUrl = $"/uploads/covers/{fileName}";
            }

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Profile updated.";
            return RedirectToAction(nameof(Index));
        }


    }
}
