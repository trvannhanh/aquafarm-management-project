using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AquaFarmApp.Controllers
{
    public class HarvestSaleController : Controller
    {
        private readonly AquaFarmContext _context;
        private readonly UserManager<User> _userManager;

        public HarvestSaleController(AquaFarmContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = new CreateSaleViewModel
            {
                AreaBatchId = id
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không thể xác định người dùng hiện tại.";
                var areaBatchForView = await _context.AreaBatches
                    .Include(ab => ab.Area)
                    .Include(ab => ab.Batch)
                    .FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);
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
            var hsale = new HarvestSale
            {
                HarvestDate = model.harvestdate,
                BuyerName = model.buyername,
                PricePerKg = model.priceperkg,
                QuantityKg = model.quantitykg,
                Revenue = model.revenue,
                EstimatedCost = model.estimatedcost,
                Profit = model.profit,
                Note = model.note,
                UserId = user.Id,
                AreabatchId = model.AreaBatchId
            };
            areaBatch.Quantity = (int)(areaBatch.Quantity - hsale.QuantityKg);
            _context.HarvestSales.Add(hsale);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Area");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var harvestSales = await _context.HarvestSales.Include(h => h.Areabatch).Include(h => h.User).ToListAsync();
            return View(harvestSales);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _context.HarvestSales.FindAsync(id);
            if (sale == null) return NotFound();
            return View(sale);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HarvestSale sale)
        {
            if (id != sale.SaleId) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.HarvestSales.FindAsync(id);
            if (sale == null) return NotFound();
            return RedirectToAction("Index");
        }
    }
}