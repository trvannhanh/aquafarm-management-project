using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Models;

[Table("User")]
[Index("UserName", Name = "UQ__User__F3DBC572A7B6B1EE", IsUnique = true)]
public partial class User : IdentityUser<int>
{
    [Key]
    [Column("user_id")]
    public override int Id { get; set; } // Ghi đè Id để ánh xạ với user_id

    [Column("username")]
    [StringLength(50)]
    [Unicode(false)]
    public override string UserName { get; set; } = null!; // Ghi đè UserName

    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public override string PasswordHash { get; set; } = null!; // Ghi đè PasswordHash

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public override string? Email { get; set; } // Ghi đè Email

    [Column("normalized_user_name")]
    [StringLength(256)]
    [Unicode(false)]
    public override string? NormalizedUserName { get; set; }

    [Column("normalized_email")]
    [StringLength(256)]
    [Unicode(false)]
    public override string? NormalizedEmail { get; set; }

    [Column("email_confirmed")]
    public override bool EmailConfirmed { get; set; }

    [Column("access_failed_count")]
    public override int AccessFailedCount { get; set; }

    [Column("concurrency_stamp")]
    public override string? ConcurrencyStamp { get; set; }

    [Column("lockout_end")]
    public override DateTimeOffset? LockoutEnd { get; set; }

    [Column("lockout_enabled")]
    public override bool LockoutEnabled { get; set; }

    [Column("two_factor_enabled")]
    public override bool TwoFactorEnabled { get; set; }

    [Column("phone_number")]
    [StringLength(50)]
    [Unicode(false)]
    public override string? PhoneNumber { get; set; }

    [Column("phone_number_confirmed")]
    public override bool PhoneNumberConfirmed { get; set; }

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
