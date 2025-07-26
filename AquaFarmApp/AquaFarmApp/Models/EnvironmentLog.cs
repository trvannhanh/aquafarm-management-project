using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("EnvironmentLog")]
public partial class EnvironmentLog
{
    [Key]
    [Column("env_log_id")]
    public int EnvLogId { get; set; }

    [Column("temperature")]
    public double Temperature { get; set; }

    [Column("ph_level")]
    public double PhLevel { get; set; }

    [Column("oxygen_level")]
    public double OxygenLevel { get; set; }

    [Column("salinity")]
    public double Salinity { get; set; }

    [Column("is_warning")]
    public bool? IsWarning { get; set; }

    [Column("recorded_at", TypeName = "datetime")]
    public DateTime? RecordedAt { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string? Note { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("area_id")]
    public int? AreaId { get; set; }

    [ForeignKey("AreaId")]
    [InverseProperty("EnvironmentLogs")]
    public virtual Area? Area { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("EnvironmentLogs")]
    public virtual User? User { get; set; }
}
