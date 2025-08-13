using AquaFarmApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AquaFarmApp.Data;

public partial class AquaFarmContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AquaFarmContext()
    {
    }

    public AquaFarmContext(DbContextOptions<AquaFarmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<AreaBatch> AreaBatches { get; set; }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<EnvironmentLog> EnvironmentLogs { get; set; }

    public virtual DbSet<Farm> Farms { get; set; }

    public virtual DbSet<FeedSchedule> FeedSchedules { get; set; }

    public virtual DbSet<HarvestSale> HarvestSales { get; set; }

    public virtual DbSet<HealthCheck> HealthChecks { get; set; }

    public virtual DbSet<LiveStockTran> LiveStockTrans { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("Data Source=DESKTOP-2C3J11U; Initial Catalog=AquaFarm;Integrated Security=True;Pooling=False;Encrypt=False;Trust Server Certificate=True");
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=TRANVANNHANH\\SQLEXPRESS;Initial Catalog=AquaFarm;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Area__985D6D6BEC7658ED");

            entity.HasOne(d => d.Farm).WithMany(p => p.Areas).HasConstraintName("FK__Area__farm_id__5535A963");
        });

        modelBuilder.Entity<AreaBatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Area_Bat__3213E83FC459BE3F");

            entity.HasOne(d => d.Area).WithMany(p => p.AreaBatches).HasConstraintName("FK__Area_Batc__area___5BE2A6F2");

            entity.HasOne(d => d.Batch).WithMany(p => p.AreaBatches).HasConstraintName("FK__Area_Batc__batch__5CD6CB2B");
        });

        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__Batch__DBFC04310334B0D9");
        });

        modelBuilder.Entity<EnvironmentLog>(entity =>
        {
            entity.HasKey(e => e.EnvLogId).HasName("PK__Environm__C575FA679E1A87D9");

            entity.HasOne(d => d.Area).WithMany(p => p.EnvironmentLogs).HasConstraintName("FK__Environme__area___656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.EnvironmentLogs).HasConstraintName("FK__Environme__user___66603565");
        });

        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.FarmId).HasName("PK__Farm__23F321B45DFAAB03");
        });

        modelBuilder.Entity<FeedSchedule>(entity =>
        {
            entity.HasKey(e => e.FeedId).HasName("PK__FeedSche__FDD969A98551C2A5");

            entity.HasOne(d => d.Areabatch).WithMany(p => p.FeedSchedules).HasConstraintName("FK__FeedSched__areab__619B8048");

            entity.HasOne(d => d.User).WithMany(p => p.FeedSchedules).HasConstraintName("FK__FeedSched__user___628FA481");
        });

        modelBuilder.Entity<HarvestSale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("PK__HarvestS__E1EB00B2C10F574D");

            entity.HasOne(d => d.Areabatch).WithMany(p => p.HarvestSales).HasConstraintName("FK__HarvestSa__areab__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.HarvestSales).HasConstraintName("FK__HarvestSa__user___72C60C4A");
        });

        modelBuilder.Entity<HealthCheck>(entity =>
        {
            entity.HasKey(e => e.CheckId).HasName("PK__HealthCh__C0EB87181490AB71");

            entity.HasOne(d => d.Area).WithMany(p => p.HealthChecks).HasConstraintName("FK__HealthChe__area___6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.HealthChecks).HasConstraintName("FK__HealthChe__user___6B24EA82");
        });

        modelBuilder.Entity<LiveStockTran>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__LiveStoc__85C600AF3FB161A2");

            entity.HasOne(d => d.Areabatch).WithMany(p => p.LiveStockTrans).HasConstraintName("FK__LiveStock__areab__6FE99F9F");

            entity.HasOne(d => d.User).WithMany(p => p.LiveStockTrans).HasConstraintName("FK__LiveStock__user___6EF57B66");
        });

        // Ánh xạ bảng User cho ASP.NET Identity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(e => e.Id).HasName("PK__User__B9BE370F71487F35");

            // Ánh xạ các thuộc tính Identity
            entity.Property(e => e.Id).HasColumnName("user_id");
            entity.Property(e => e.UserName).HasColumnName("username").HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_user_name").HasMaxLength(256).IsUnicode(false);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(256).IsUnicode(false);
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.PasswordHash).HasColumnName("password").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
            entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");

            // Ánh xạ các thuộc tính tùy chỉnh
            entity.Property(e => e.FullName).HasColumnName("fullname").HasMaxLength(50);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("datetime");

            entity.HasIndex(e => e.UserName, "UQ__User__F3DBC572A7B6B1EE").IsUnique();

            // Cấu hình mối quan hệ nhiều-nhiều với Farm
            entity.HasMany(d => d.Farms).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Farm_User",
                    r => r.HasOne<Farm>().WithMany()
                        .HasForeignKey("FarmId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Farm_User__farm___5070F446"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Farm_User__user___4F7CD00D"),
                    j =>
                    {
                        j.HasKey("UserId", "FarmId").HasName("PK__Farm_Use__EB810514B45ACAF9");
                        j.ToTable("Farm_User");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("FarmId").HasColumnName("farm_id");
                    });
        });

        // Ánh xạ các bảng Identity
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
