using AquaFarmApp.Models;

namespace AquaFarmApp.ViewModels
{
    public class HomeViewModel
    {
        public int TotalFarms { get; set; }
        public int TotalAreas { get; set; }
        public int TotalStaff { get; set; }
        public List<Farm> Farms { get; set; } = new List<Farm>();
        public bool IsOwner { get; set; }
    }
}
