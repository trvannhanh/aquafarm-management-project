using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("HealthCheck")]
public partial class HealthCheck
{
    [Key]
    [Column("check_id")]
    public int CheckId { get; set; }

    [Column("disease_signs")]
    [StringLength(255)]
    public string DiseaseSigns { get; set; } = null!;

    [Column("health_status")]
    [StringLength(50)]
    [Unicode(false)]
    public string HealthStatus { get; set; } = null!;

    [Column("notes")]
    [StringLength(255)]
    public string? Notes { get; set; }

    [Column("check_date", TypeName = "datetime")]
    public DateTime? CheckDate { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("area_id")]
    public int? AreaId { get; set; }

    [ForeignKey("AreaId")]
    [InverseProperty("HealthChecks")]
    public virtual Area? Area { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("HealthChecks")]
    public virtual User? User { get; set; }
}
