using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly AquaFarmContext _context;
        private readonly UserManager<User> _userManager;
        public ReportController(AquaFarmContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        public async Task<IActionResult> EnvironmentReport(int? areaId, DateTime? fromDate, DateTime? toDate)
        {
            var model = await GetEnvironmentDataAsync(areaId, fromDate, toDate);
            return View(model);
        }

        private async Task<EnvironmentReportViewModel> GetEnvironmentDataAsync(int? areaId, DateTime? fromDate, DateTime? toDate)
        {
            // Bước 1: Lấy User hiện tại
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Xử lý lỗi, ví dụ redirect login hoặc throw exception
                throw new Exception("Không tìm thấy user.");
            }

            // Bước 2: Lấy danh sách FarmIds thuộc User (mối quan hệ n-n)
            var userFarmIds = await _context.Farms
                .Where(f => f.Users.Any(u => u.Id == currentUser.Id)) // Lọc Farm có liên kết với User
                .Select(f => f.FarmId)
                .ToListAsync();

            if (!userFarmIds.Any())
            {
                // Không có farm, trả về model rỗng hoặc thông báo
                return new EnvironmentReportViewModel { /* Có thể thêm message */ };
            }

            // Bước 3: Lấy danh sách AreaIds thuộc các Farm của User
            var allowedAreaIds = await _context.Areas
                .Where(a => userFarmIds.Contains(a.FarmId ?? 0)) // Lọc Area thuộc Farm, xử lý FarmId nullable
                .Select(a => a.AreaId)
                .ToListAsync();

            if (!allowedAreaIds.Any())
            {
                // Không có area, trả về model rỗng
                return new EnvironmentReportViewModel { /* Có thể thêm message */ };
            }

            // Bước 4: Query EnvironmentLogs, lọc theo allowedAreaIds
            var query = _context.EnvironmentLogs
                .Where(log => log.RecordedAt != null)
                .Where(log => allowedAreaIds.Contains(log.AreaId ?? 0)) // Chỉ lấy log từ Area được phép, xử lý AreaId nullable
                .OrderBy(log => log.RecordedAt);

            // Lọc thêm theo areaId nếu người dùng chỉ định (nhưng phải thuộc allowedAreaIds)
            if (areaId.HasValue)
            {
                if (!allowedAreaIds.Contains(areaId.Value))
                {
                    // AreaId không thuộc quyền, xử lý lỗi hoặc bỏ qua
                    ModelState.AddModelError("", "Bạn không có quyền xem area này.");
                    return new EnvironmentReportViewModel();
                }
                query = (IOrderedQueryable<EnvironmentLog>)query.Where(log => log.AreaId == areaId);
            }

            // Lọc theo thời gian
            if (fromDate.HasValue)
            {
                query = (IOrderedQueryable<EnvironmentLog>)query.Where(log => log.RecordedAt >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = (IOrderedQueryable<EnvironmentLog>)query.Where(log => log.RecordedAt <= toDate.Value);
            }

            var logs = await query.ToListAsync(); // Thực thi query async

            // Bước 5: Chuẩn bị model
            var model = new EnvironmentReportViewModel
            {
                AreaId = areaId,
                FromDate = fromDate,
                ToDate = toDate
            };

            foreach (var log in logs)
            {
                model.Dates.Add(log.RecordedAt.Value);
                model.Temperatures.Add(log.Temperature);
                model.PhLevels.Add(log.PhLevel);
                model.OxygenLevels.Add(log.OxygenLevel);
                model.Salinities.Add(log.Salinity);
            }

            return model;
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        public async Task<IActionResult> RevenueReport(int? batchId, string groupBy = "Quarter")
        {
            var model = await GetRevenueDataAsync(batchId, groupBy);
            return View(model);
        }

        private async Task<RevenueReportViewModel> GetRevenueDataAsync(int? batchId, string groupBy)
        {
            // Bước 1: Lấy User hiện tại
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                throw new Exception("Không tìm thấy user.");
            }

            // Bước 2: Lấy danh sách FarmIds thuộc User
            var userFarmIds = await _context.Farms
                .Where(f => f.Users.Any(u => u.Id == currentUser.Id))
                .Select(f => f.FarmId)
                .ToListAsync();

            if (!userFarmIds.Any())
            {
                return new RevenueReportViewModel { GroupBy = groupBy };
            }

            // Bước 3: Lấy danh sách AreaIds thuộc các Farm
            var allowedAreaIds = await _context.Areas
                .Where(a => userFarmIds.Contains(a.FarmId ?? 0))
                .Select(a => a.AreaId)
                .ToListAsync();

            if (!allowedAreaIds.Any())
            {
                return new RevenueReportViewModel { GroupBy = groupBy };
            }

            // Bước 4: Query HarvestSales qua AreaBatch
            var query = _context.HarvestSales
                .Include(s => s.Areabatch)
                .ThenInclude(ab => ab.Area)
                .Where(s => s.HarvestDate != null)
                .Where(s => allowedAreaIds.Contains(s.Areabatch != null ? s.Areabatch.AreaId : 0));

            // Lọc theo BatchId nếu có
            if (batchId.HasValue)
            {
                var validBatch = await _context.AreaBatches
                    .AnyAsync(ab => ab.BatchId == batchId.Value && allowedAreaIds.Contains(ab.AreaId));
                if (!validBatch)
                {
                    ModelState.AddModelError("", "Bạn không có quyền xem lứa nuôi này.");
                    return new RevenueReportViewModel { GroupBy = groupBy };
                }
                // Replace the problematic line with the following code to avoid using the null propagating operator in an expression tree:
                query = query.Where(s => s.Areabatch != null && s.Areabatch.BatchId == batchId);
            }

            // Bước 5: Tổng hợp dữ liệu theo groupBy (Quarter/Year)
            var model = new RevenueReportViewModel { BatchId = batchId, GroupBy = groupBy };

            if (groupBy == "Year")
            {
                var groupedSales = await query
                    .GroupBy(s => s.HarvestDate.Value.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        TotalRevenue = g.Sum(s => s.Revenue),
                        TotalProfit = g.Sum(s => s.Profit)
                    })
                    .OrderBy(g => g.Year)
                    .ToListAsync();

                foreach (var group in groupedSales)
                {
                    model.Labels.Add(group.Year.ToString());
                    model.Revenues.Add(group.TotalRevenue);
                    model.Profits.Add(group.TotalProfit);
                }
            }
            else // Mặc định là Quarter
            {
                var groupedSales = await query
                    .GroupBy(s => new
                    {
                        Year = s.HarvestDate.Value.Year,
                        Quarter = (s.HarvestDate.Value.Month - 1) / 3 + 1
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Quarter = g.Key.Quarter,
                        TotalRevenue = g.Sum(s => s.Revenue),
                        TotalProfit = g.Sum(s => s.Profit)
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Quarter)
                    .ToListAsync();

                foreach (var group in groupedSales)
                {
                    model.Labels.Add($"Q{group.Quarter}-{group.Year}");
                    model.Revenues.Add(group.TotalRevenue);
                    model.Profits.Add(group.TotalProfit);
                }
            }

            return model;
        }

        [HttpGet]
        public async Task<JsonResult> RevenueReportByFarm(int farmId, string groupBy = "Quarter")
        {
            // Kiểm tra quyền (giả sử user đã login, kiểm tra Farm thuộc user)
            var currentUser = await _userManager.GetUserAsync(User);
            var isValidFarm = await _context.Farms.AnyAsync(f => f.FarmId == farmId && f.Users.Any(u => u.Id == currentUser.Id));
            if (!isValidFarm)
            {
                return Json(new { error = "Không có quyền truy cập trang trại này." });
            }

            // Query Areas của Farm
            var areaIds = await _context.Areas.Where(a => a.FarmId == farmId).Select(a => a.AreaId).ToListAsync();

            var query = _context.HarvestSales
                .Where(s => s.HarvestDate != null && s.Areabatch != null && areaIds.Contains(s.Areabatch.AreaId));

            var model = new RevenueReportViewModel { GroupBy = groupBy };

            if (groupBy == "Year")
            {
                var groupedSales = await query
                    .GroupBy(s => s.HarvestDate.Value.Year)
                    .Select(g => new { Year = g.Key, TotalRevenue = g.Sum(s => s.Revenue), TotalProfit = g.Sum(s => s.Profit) })
                    .OrderBy(g => g.Year)
                    .ToListAsync();

                foreach (var group in groupedSales)
                {
                    model.Labels.Add(group.Year.ToString());
                    model.Revenues.Add(group.TotalRevenue);
                    model.Profits.Add(group.TotalProfit);
                }
            }
            else
            {
                var groupedSales = await query
                    .GroupBy(s => new { Year = s.HarvestDate.Value.Year, Quarter = (s.HarvestDate.Value.Month - 1) / 3 + 1 })
                    .Select(g => new { Year = g.Key.Year, Quarter = g.Key.Quarter, TotalRevenue = g.Sum(s => s.Revenue), TotalProfit = g.Sum(s => s.Profit) })
                    .OrderBy(g => g.Year).ThenBy(g => g.Quarter)
                    .ToListAsync();

                foreach (var group in groupedSales)
                {
                    model.Labels.Add($"Q{group.Quarter}-{group.Year}");
                    model.Revenues.Add(group.TotalRevenue);
                    model.Profits.Add(group.TotalProfit);
                }
            }

            return Json(new { labels = model.Labels, revenues = model.Revenues, profits = model.Profits });
        }
    }
}
