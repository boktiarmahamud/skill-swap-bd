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
    [Authorize]
    public class SwapRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SwapRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /SwapRequests  (Incoming + Outgoing lists)
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.DebugUserId = userId; // remove once confirmed fixed

            var incoming = await _context.SwapRequests
                .Include(sr => sr.Requester)
                .Include(sr => sr.RequesterSkill)
                .Include(sr => sr.ReceiverSkill)
                .Where(sr => sr.ReceiverId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();

            var outgoing = await _context.SwapRequests
                .Include(sr => sr.Receiver)
                .Include(sr => sr.RequesterSkill)
                .Include(sr => sr.ReceiverSkill)
                .Where(sr => sr.RequesterId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();

            ViewBag.Incoming = incoming;
            ViewBag.Outgoing = outgoing;

            return View();
        }

        // GET: /SwapRequests/Create?receiverSkillId=5
        public async Task<IActionResult> Create(int receiverSkillId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval. You can't request swaps yet.";
                return RedirectToAction("Index", "Home");
            }

            var receiverSkill = await _context.Skills
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == receiverSkillId && s.IsApproved);

            if (receiverSkill == null) return NotFound();

            if (receiverSkill.UserId == user.Id)
            {
                TempData["Error"] = "You can't request a swap on your own skill post.";
                return RedirectToAction("Details", "Skills", new { id = receiverSkillId });
            }

            var myOfferSkills = await _context.Skills
                .Where(s => s.UserId == user.Id && s.Type == SkillType.Offer && s.IsApproved)
                .ToListAsync();

            if (myOfferSkills.Count == 0)
            {
                TempData["Error"] = "Post at least one approved skill you can offer before requesting a swap.";
                return RedirectToAction(nameof(SkillsController.Create), "Skills");
            }

            var vm = new SwapRequestCreateViewModel
            {
                ReceiverSkillId = receiverSkillId,
                ReceiverSkill = receiverSkill,
                MyOfferSkills = myOfferSkills
            };

            return View(vm);
        }

        // POST: /SwapRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SwapRequestCreateViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval. You can't request swaps yet.";
                return RedirectToAction("Index", "Home");
            }

            var receiverSkill = await _context.Skills.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == vm.ReceiverSkillId);
            var requesterSkill = await _context.Skills.FindAsync(vm.RequesterSkillId);

            if (receiverSkill == null || requesterSkill == null) return NotFound();

            if (requesterSkill.UserId != user.Id)
            {
                ModelState.AddModelError(string.Empty, "You can only offer a skill that belongs to you.");
            }

            if (!ModelState.IsValid)
            {
                vm.ReceiverSkill = receiverSkill;
                vm.MyOfferSkills = await _context.Skills
                    .Where(s => s.UserId == user.Id && s.Type == SkillType.Offer && s.IsApproved)
                    .ToListAsync();
                return View(vm);
            }

            var swapRequest = new SwapRequest
            {
                RequesterId = user.Id,
                ReceiverId = receiverSkill.UserId,
                RequesterSkillId = requesterSkill.Id,
                ReceiverSkillId = receiverSkill.Id,
                Message = vm.Message,
                Status = SwapStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.SwapRequests.Add(swapRequest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Swap request sent!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /SwapRequests/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var swap = await _context.SwapRequests
                .Include(sr => sr.Requester)
                .Include(sr => sr.Receiver)
                .Include(sr => sr.RequesterSkill)
                .Include(sr => sr.ReceiverSkill)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (swap == null) return NotFound();
            if (swap.RequesterId != userId && swap.ReceiverId != userId) return Forbid();

            return View(swap);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id) => await ChangeStatus(id, SwapStatus.Accepted, receiverOnly: true);

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id) => await ChangeStatus(id, SwapStatus.Rejected, receiverOnly: true);

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id) => await ChangeStatus(id, SwapStatus.Cancelled, receiverOnly: false);

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id) => await ChangeStatus(id, SwapStatus.Completed, receiverOnly: false);

        private async Task<IActionResult> ChangeStatus(int id, SwapStatus newStatus, bool receiverOnly)
        {
            var userId = _userManager.GetUserId(User);
            var swap = await _context.SwapRequests.FindAsync(id);
            if (swap == null) return NotFound();

            var isReceiver = swap.ReceiverId == userId;
            var isRequester = swap.RequesterId == userId;
            if (!isReceiver && !isRequester) return Forbid();
            if (receiverOnly && !isReceiver) return Forbid();

            if (newStatus == SwapStatus.Accepted || newStatus == SwapStatus.Rejected)
            {
                if (swap.Status != SwapStatus.Pending)
                {
                    TempData["Error"] = "This request has already been responded to.";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            if (newStatus == SwapStatus.Cancelled && swap.Status != SwapStatus.Pending)
            {
                TempData["Error"] = "Only a pending request can be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (newStatus == SwapStatus.Completed && swap.Status != SwapStatus.Accepted)
            {
                TempData["Error"] = "Only an accepted swap can be marked completed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            swap.Status = newStatus;
            swap.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Swap request marked as {newStatus}.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}