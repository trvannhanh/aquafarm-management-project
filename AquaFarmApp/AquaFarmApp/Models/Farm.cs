using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("Farm")]
public partial class Farm
{
    [Key]
    [Column("farm_id")]
    public int FarmId { get; set; }

    [Column("farm_name")]
    [StringLength(50)]
    public string FarmName { get; set; } = null!;

    [Column("farm_location")]
    [StringLength(200)]
    public string FarmLocation { get; set; } = null!;

    [Column("area_total")]
    public int AreaTotal { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Farm")]
    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();

    [ForeignKey("FarmId")]
    [InverseProperty("Farms")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
