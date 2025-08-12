namespace AquaFarmApp.ViewModels
{
    public class EnvironmentReportViewModel
    {
        public List<DateTime> Dates { get; set; } = new List<DateTime>(); // Trục X: Thời gian
        public List<double> Temperatures { get; set; } = new List<double>(); // Dữ liệu nhiệt độ
        public List<double> PhLevels { get; set; } = new List<double>(); // Dữ liệu pH
        public List<double> OxygenLevels { get; set; } = new List<double>(); // Dữ liệu oxy
        public List<double> Salinities { get; set; } = new List<double>(); // Dữ liệu độ mặn
        public int? AreaId { get; set; } // Để lọc theo khu vực
        public DateTime? FromDate { get; set; } // Lọc từ ngày
        public DateTime? ToDate { get; set; } // Lọc đến ngày
    }
}
