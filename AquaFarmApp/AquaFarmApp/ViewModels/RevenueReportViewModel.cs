namespace AquaFarmApp.ViewModels
{
    public class RevenueReportViewModel
    {
        public List<string> Labels { get; set; } = new List<string>(); // Quý/Năm hoặc BatchId
        public List<double> Revenues { get; set; } = new List<double>(); // Tổng doanh thu
        public List<double> Profits { get; set; } = new List<double>(); // Tổng lợi nhuận
        public int? BatchId { get; set; } // Lọc theo lứa nuôi
        public string GroupBy { get; set; } = "Quarter"; // "Quarter" hoặc "Year"
    }
}
