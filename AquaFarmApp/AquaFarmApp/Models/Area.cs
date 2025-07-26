using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("Area")]
public partial class Area
{
    [Key]
    [Column("area_id")]
    public int AreaId { get; set; }

    [Column("area_name")]
    [StringLength(50)]
    public string AreaName { get; set; } = null!;

    [Column("area_status")]
    [StringLength(10)]
    [Unicode(false)]
    public string AreaStatus { get; set; } = null!;

    [Column("area_size")]
    public double AreaSize { get; set; }

    [Column("type_of_water")]
    [StringLength(20)]
    public string TypeOfWater { get; set; } = null!;

    [Column("farm_id")]
    public int? FarmId { get; set; }

    [InverseProperty("Area")]
    public virtual ICollection<AreaBatch> AreaBatches { get; set; } = new List<AreaBatch>();

    [InverseProperty("Area")]
    public virtual ICollection<EnvironmentLog> EnvironmentLogs { get; set; } = new List<EnvironmentLog>();

    [ForeignKey("FarmId")]
    [InverseProperty("Areas")]
    public virtual Farm? Farm { get; set; }

    [InverseProperty("Area")]
    public virtual ICollection<HealthCheck> HealthChecks { get; set; } = new List<HealthCheck>();
}
