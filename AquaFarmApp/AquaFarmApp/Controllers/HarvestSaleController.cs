using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.EntityFrameworkCore;
using AquaFarmApp.ViewModels;
using Microsoft.VisualBasic;

namespace AquaFarmApp.Controllers
{
    public class HarvestSaleController : Controller
    {
        private readonly AquaFarmContext _context;

        public HarvestSaleController(AquaFarmContext context)
        {
            _context = context;
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
            var areaBatch = await _context.AreaBatches.FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);
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
                AreabatchId = model.AreaBatchId
            };
            areaBatch.Quantity = areaBatch.Quantity - hsale.QuantityKg;
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