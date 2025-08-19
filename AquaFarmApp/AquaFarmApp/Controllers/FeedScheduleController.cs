using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Controllers
{
    public class FeedScheduleController : Controller
    {
        private readonly AquaFarmContext _context;
        private readonly UserManager<User> _userManager;

        public FeedScheduleController(AquaFarmContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private List<string> GetFeedTypes() => new() { "Thức ăn viên công nghiệp", "Thức ăn tự chế", "Cám gạo", "Bột cá", "Trùn chỉ" };
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            // Kiểm tra và lấy AreaBatch với thông tin Area và Batch
            var areaBatch = await _context.AreaBatches.FindAsync(id);

            if (areaBatch == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                return RedirectToAction("AreaBatchIndex");
            }

            var model = new CreateFeedScheduleViewModel
            {
                AreaBatchId = id
            };
            ViewBag.FeedTypeList = GetFeedTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFeedScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FeedTypeList = GetFeedTypes();
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không thể xác định người dùng hiện tại.";
                var areaBatchForView = await _context.AreaBatches
                    .Include(ab => ab.Area)
                    .Include(ab => ab.Batch)
                    .FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);
                if (areaBatchForView != null)
                {
                    ViewBag.FeedTypeList = GetFeedTypes();
                }
                return View(model);
            }

            // Kiểm tra xem AreaBatch có tồn tại không
            var areaBatch = await _context.AreaBatches
                .Include(ab => ab.Area)
                .Include(ab => ab.Batch)
                .FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);
            if (areaBatch == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                return View(model);
            }

            var feedS = new FeedSchedule
            {
                FeedType = model.FeedType,
                FeedCost = model.FeedCost,
                FeedQuantity = model.QuantityKg,
                FeedTime = model.FeedTime,
                Note = model.Note,
                FeedStatus = "Not Done",
                UserId = user.Id,
                AreabatchId = model.AreaBatchId
            };
            _context.FeedSchedules.Add(feedS);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Area");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var feedSchedules = await _context.FeedSchedules.Include(f => f.Areabatch).Include(f => f.User).ToListAsync();
            return View(feedSchedules);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var feedSchedule = await _context.FeedSchedules.FindAsync(id);
            if (feedSchedule == null) return NotFound();
            return View(feedSchedule);
        }
        public async Task<IActionResult> Edit(int id, FeedSchedule feedS)
        {
            if (id != feedS.FeedId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(feedS);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var feedSchedule = await _context.FeedSchedules.FindAsync(id);
            if (feedSchedule == null) return NotFound();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var feed = await _context.FeedSchedules.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }

            feed.FeedStatus = "Done";
            _context.Update(feed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}