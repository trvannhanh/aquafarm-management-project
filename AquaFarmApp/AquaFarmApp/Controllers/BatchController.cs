using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AquaFarmApp.Controllers
{
    [Authorize(Roles = "Admin, Owner")]
    public class BatchController : Controller
    {
        private readonly AquaFarmContext _context;

        public BatchController(AquaFarmContext context)
        {
            _context = context;
        }

        // Danh sách AreaBatch
        [HttpGet]
        public async Task<IActionResult> AreaBatchIndex(int page = 1, int pageSize = 10)
        {
            var areaBatches = from ab in _context.AreaBatches
                              join a in _context.Areas on ab.AreaId equals a.AreaId
                              join b in _context.Batches on ab.BatchId equals b.BatchId
                              select new AreaBatchViewModel
                              {
                                  AreaBatchId = ab.Id,
                                  AreaId = ab.AreaId,
                                  AreaName = a.AreaName,
                                  BatchId = ab.BatchId,
                                  Quantity = ab.Quantity,
                                  AquaticBreed = b.AquaticBreed,
                                  BatchStatus = b.BatchStatus
                              };

            var paginatedList = PaginatedList<AreaBatchViewModel>.Create(areaBatches, page, pageSize);
            return View("AreaBatchIndex", paginatedList);
        }

        // Danh sách Batch
        [HttpGet]
        public async Task<IActionResult> BatchIndex(int page = 1, int pageSize = 10)
        {
            var batches = from b in _context.Batches
                          select new BatchViewModel
                          {
                              BatchId = b.BatchId,
                              Source = b.Source,
                              AquaticBreed = b.AquaticBreed,
                              TotalQuantity = b.TotalQuantity,
                              StartDate = b.StartDate,
                              EstimatedHarvestDate = b.EstimatedHarvestDate,
                              BatchStatus = b.BatchStatus,
                              AssignedAreaCount = b.AreaBatches.Count()
                          };

            var paginatedList = PaginatedList<BatchViewModel>.Create(batches, page, pageSize);
            return View("BatchIndex", paginatedList);
        }

        // Action Delete cho AreaBatch
        [HttpGet]
        public async Task<IActionResult> DeleteAreaBatch(int id)
        {
            var areaBatch = await _context.AreaBatches
                .Include(ab => ab.Area)
                .FirstOrDefaultAsync(ab => ab.Id == id);

            if (areaBatch == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                return RedirectToAction("AreaBatchIndex");
            }

            return View(areaBatch);
        }

        [HttpPost, ActionName("DeleteAreaBatch")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAreaBatchConfirmed(int id)
        {
            var areaBatch = await _context.AreaBatches
                .Include(ab => ab.Area)
                .FirstOrDefaultAsync(ab => ab.Id == id);

            if (areaBatch == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                return RedirectToAction("AreaBatchIndex");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.AreaBatches.Remove(areaBatch);
                    areaBatch.Area.AreaStatus = AreaStatus.Avail.ToString();
                    _context.Areas.Update(areaBatch.Area);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = "Xóa phân bổ và cập nhật trạng thái khu vực thành công.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = $"Lỗi khi xóa phân bổ: {ex.Message}";
                }
            }

            return RedirectToAction("AreaBatchIndex");
        }

        // Action Delete cho Batch
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.AreaBatches)
                .ThenInclude(ab => ab.Area)
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null)
            {
                TempData["Error"] = "Không tìm thấy lứa nuôi.";
                return RedirectToAction("BatchIndex");
            }

            return View(batch);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.AreaBatches)
                .ThenInclude(ab => ab.Area)
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null)
            {
                TempData["Error"] = "Không tìm thấy lứa nuôi.";
                return RedirectToAction("BatchIndex");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Cập nhật trạng thái khu vực về Avail
                    foreach (var areaBatch in batch.AreaBatches)
                    {
                        areaBatch.Area.AreaStatus = AreaStatus.Avail.ToString();
                        _context.Areas.Update(areaBatch.Area);
                    }

                    // Xóa tất cả AreaBatch liên quan
                    _context.AreaBatches.RemoveRange(batch.AreaBatches);

                    // Xóa Batch
                    _context.Batches.Remove(batch);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = "Xóa lứa nuôi và cập nhật trạng thái khu vực thành công.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = $"Lỗi khi xóa lứa nuôi: {ex.Message}";
                }
            }

            return RedirectToAction("BatchIndex");
        }

        [HttpGet]
        public IActionResult CreateBatch()
        {
            return View(new CreateBatchViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBatch(CreateBatchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                return View(model);
            }

            try
            {
                var batch = new Batch
                {
                    BatchStatus = BatchStatus.Avail.ToString(),
                    Source = model.Source,
                    AquaticBreed = model.AquaticBreed,
                    StartDate = model.StartDate,
                    EstimatedHarvestDate = model.EstimatedHarvestDate,
                    TotalQuantity = model.TotalQuantity
                };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.Batches.Add(batch);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        TempData["Success"] = "Tạo lứa nuôi thành công!";
                        // Redirect với AssignAreaViewModel
                        return RedirectToAction("AssignArea", new { batchId = batch.BatchId });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["Error"] = $"Lỗi khi lưu batch: {ex.Message}";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignArea(int batchId)
        {
            var model = new AssignAreaViewModel
            {
                BatchId = batchId,
                AreaBatches = new List<AreaBatchInput>()
            };

            // Lấy danh sách khu vực có trạng thái Avail
            var areas = await _context.Areas
                .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                .ToListAsync();

            // Lưu danh sách khu vực vào ViewData
            ViewData["AvailableAreas"] = areas;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignArea(AssignAreaViewModel model)
        {
            // Loại bỏ lỗi xác thực cho AreaBatches nếu không có khu vực nào được chọn
            if (model.AreaBatches == null || !model.AreaBatches.Any())
            {
                ModelState.Remove("AreaBatches");
                model.AreaBatches = new List<AreaBatchInput>();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Dữ liệu phân bổ không hợp lệ: " + string.Join("; ", errors);

                // Tải lại danh sách khu vực
                var areas = await _context.Areas
                    .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                    .ToListAsync();
                ViewData["AvailableAreas"] = areas;
                return View(model);
            }

            try
            {
                var batch = await _context.Batches.FindAsync(model.BatchId);
                if (batch == null)
                {
                    TempData["Error"] = "Không tìm thấy batch.";
                    ViewData["AvailableAreas"] = await _context.Areas
                        .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                        .ToListAsync();
                    return View(model);
                }

                var selectedAreaIds = model.AreaBatches?.Select(ab => ab.AreaId).ToList() ?? new List<int>();
                if (!selectedAreaIds.Any())
                {
                    TempData["Error"] = "Vui lòng chọn ít nhất một khu vực.";
                    ViewData["AvailableAreas"] = await _context.Areas
                        .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                        .ToListAsync();
                    return View(model);
                }

                var areas = await _context.Areas
                    .Where(a => selectedAreaIds.Contains(a.AreaId) && a.AreaStatus == AreaStatus.Avail.ToString())
                    .ToListAsync();

                if (areas.Count != selectedAreaIds.Count)
                {
                    TempData["Error"] = "Một số khu vực không khả dụng.";
                    ViewData["AvailableAreas"] = await _context.Areas
                        .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                        .ToListAsync();
                    return View(model);
                }

                int totalAssigned = model.AreaBatches.Sum(ab => ab.Quantity);
                if (totalAssigned > batch.TotalQuantity)
                {
                    TempData["Error"] = $"Tổng số lượng phân bổ ({totalAssigned}) vượt quá số lượng batch ({batch.TotalQuantity}).";
                    ViewData["AvailableAreas"] = await _context.Areas
                        .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                        .ToListAsync();
                    return View(model);
                }

                foreach (var area in areas)
                {
                    var assignedQuantity = model.AreaBatches.FirstOrDefault(ab => ab.AreaId == area.AreaId)?.Quantity ?? 0;
                    if (assignedQuantity > area.AreaSize)
                    {
                        TempData["Error"] = $"Khu vực {area.AreaName} không đủ dung lượng ({area.AreaSize}) cho số lượng {assignedQuantity}.";
                        ViewData["AvailableAreas"] = await _context.Areas
                            .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                            .ToListAsync();
                        return View(model);
                    }
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var areaBatch in model.AreaBatches)
                        {
                            _context.AreaBatches.Add(new AreaBatch
                            {
                                AreaId = areaBatch.AreaId,
                                BatchId = model.BatchId,
                                Quantity = areaBatch.Quantity
                            });
                        }

                        // Cập nhật trạng thái khu vực thành Occupied
                        foreach (var area in areas)
                        {
                            area.AreaStatus = AreaStatus.NotAvail.ToString();
                            _context.Areas.Update(area);
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        TempData["Success"] = "Phân bổ batch thành công.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["Error"] = $"Lỗi khi phân bổ khu vực: {ex.Message}";
                        ViewData["AvailableAreas"] = await _context.Areas
                            .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                            .ToListAsync();
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                ViewData["AvailableAreas"] = await _context.Areas
                    .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                    .ToListAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableAreas(int page = 1, int pageSize = 10)
        {
            var areas = await _context.Areas
                .Where(a => a.AreaStatus == AreaStatus.Avail.ToString())
                .Select(a => new { a.AreaId, a.AreaName, a.TypeOfWater, a.AreaSize })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Json(areas);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var batches = await _context.Batches
                .Select(b => new
                {
                    b.BatchId,
                    b.BatchStatus,
                    b.Source,
                    b.AquaticBreed,
                    b.TotalQuantity,
                    Areas = b.AreaBatches.Select(ab => new { ab.Area.AreaName, ab.Quantity })
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return View(batches);
        }
    }
}