using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("User")]
[Index("Username", Name = "UQ__User__F3DBC572A7B6B1EE", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("username")]
    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("fullname")]
    [StringLength(50)]
    public string Fullname { get; set; } = null!;

    [Column("role")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Role { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<EnvironmentLog> EnvironmentLogs { get; set; } = new List<EnvironmentLog>();

    [InverseProperty("User")]
    public virtual ICollection<FeedSchedule> FeedSchedules { get; set; } = new List<FeedSchedule>();

    [InverseProperty("User")]
    public virtual ICollection<HarvestSale> HarvestSales { get; set; } = new List<HarvestSale>();

    [InverseProperty("User")]
    public virtual ICollection<HealthCheck> HealthChecks { get; set; } = new List<HealthCheck>();

    [InverseProperty("User")]
    public virtual ICollection<LiveStockTran> LiveStockTrans { get; set; } = new List<LiveStockTran>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();
}
