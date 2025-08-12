using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquaFarmApp.Controllers
{
    public class LiveStockTranController : Controller
    {
        private readonly AquaFarmContext _context;
        private readonly UserManager<User> _userManager;

        public LiveStockTranController(AquaFarmContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: LiveStockTran
        [HttpGet]
        public async Task<IActionResult> LiveStockTransIndex(int id, int page = 1, int pageSize = 10)
        {
            // Kiểm tra xem AreaBatch có tồn tại không
            var areaBatch = await _context.AreaBatches.FindAsync(id);

            if (areaBatch == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                return RedirectToAction("AreaBatchIndex");
            }

            // Lấy danh sách LiveStockTran liên quan đến AreaBatch
            var liveStockTrans = from lst in _context.LiveStockTrans
                                 where lst.AreabatchId == id
                                 select new LiveStockTranViewModel
                                 {
                                     TransactionId = lst.TransactionId,
                                     AreaBatchId = lst.AreabatchId,
                                     TransDate = lst.TransDate,
                                     Quantity = lst.Quantity,
                                     TransType = lst.TransType,
                                     Note = lst.Note
                                 };

            var paginatedList = PaginatedList<LiveStockTranViewModel>.Create(liveStockTrans, page, pageSize);
            return View("LiveStockTransIndex", paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTrans(int id)
        {
            // Kiểm tra và lấy AreaBatch với thông tin Area và Batch
            var areaBatch = await _context.AreaBatches.FindAsync(id);

            if (areaBatch == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                return RedirectToAction("AreaBatchIndex");
            }

            // Tạo ViewModel với thông tin từ AreaBatch
            var model = new LiveStockTranViewModel
            {
                AreaBatchId = id,
                AreaName = areaBatch.Area?.AreaName ?? "N/A",
                BatchName = areaBatch.Batch?.AquaticBreed ?? "N/A",
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrans(LiveStockTranViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy lại thông tin AreaBatch để hiển thị trong view nếu lỗi
                var areaBatchForView = await _context.AreaBatches
                    .Include(ab => ab.Area)
                    .Include(ab => ab.Batch)
                    .FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);

                if (areaBatchForView != null)
                {
                    model.AreaName = areaBatchForView.Area?.AreaName ?? "N/A";
                    model.BatchName = areaBatchForView.Batch?.AquaticBreed ?? "N/A";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                    return RedirectToAction("AreaBatchIndex");
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                return View(model);
            }

            try
            {
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
                        model.AreaName = areaBatchForView.Area?.AreaName ?? "N/A";
                        model.BatchName = areaBatchForView.Batch?.AquaticBreed ?? "N/A";
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

                var trans = new LiveStockTran
                {
                    TransType = model.TransType,
                    Reason = model.Reason,
                    Quantity = model.Quantity,
                    TransDate = model.TransDate,
                    Note = model.Note,
                    UserId = user.Id,
                    AreabatchId = model.AreaBatchId
                };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.LiveStockTrans.Add(trans);

                        if (model.TransType == "Import")
                        {
                            // Cập nhật số lượng trong AreaBatch
                            areaBatch.Quantity += model.Quantity;
                            _context.AreaBatches.Update(areaBatch);
                        }
                        else if (model.TransType == "Export")
                        {
                            // Kiểm tra số lượng trước khi xuất
                            if (areaBatch.Quantity < model.Quantity)
                            {
                                TempData["Error"] = "Số lượng xuất không thể lớn hơn số lượng hiện có trong phân bổ.";
                                return View(model);
                            }
                            areaBatch.Quantity -= model.Quantity;
                            _context.AreaBatches.Update(areaBatch);
                        }
                        else
                        {
                            TempData["Error"] = "Loại giao dịch không hợp lệ.";
                            return View(model); 
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        TempData["Success"] = "Tạo Giao dịch thành công!";
                        return RedirectToAction("LiveStockTransIndex", new { id = model.AreaBatchId });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["Error"] = $"Lỗi khi lưu giao dịch: {ex.Message}";
                        model.AreaName = areaBatch.Area?.AreaName ?? "N/A";
                        model.BatchName = areaBatch.Batch?.AquaticBreed ?? "N/A";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                // Lấy lại thông tin AreaBatch để hiển thị trong view
                var areaBatchForView = await _context.AreaBatches
                    .Include(ab => ab.Area)
                    .Include(ab => ab.Batch)
                    .FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);

                if (areaBatchForView != null)
                {
                    model.AreaName = areaBatchForView.Area?.AreaName ?? "N/A";
                    model.BatchName = areaBatchForView.Batch?.AquaticBreed ?? "N/A";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy bản ghi phân bổ.";
                    return RedirectToAction("AreaBatchIndex");
                }
                return View(model);
            }
        }


        public IActionResult Index()
        {
            return View();
        }

        
    }
}
