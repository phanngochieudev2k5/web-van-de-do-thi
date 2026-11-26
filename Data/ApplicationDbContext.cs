using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebDoThi.Models;

namespace WebDoThi.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBadge> TblBadges { get; set; }

    public virtual DbSet<TblCategory> TblCategories { get; set; }

    public virtual DbSet<TblComment> TblComments { get; set; }

    public virtual DbSet<TblDepartment> TblDepartments { get; set; }

    public virtual DbSet<TblFundCampaign> TblFundCampaigns { get; set; }

    public virtual DbSet<TblFundTransaction> TblFundTransactions { get; set; }

    public virtual DbSet<TblInteraction> TblInteractions { get; set; }

    public virtual DbSet<TblMenu> TblMenus { get; set; }

    public virtual DbSet<TblNews> TblNews { get; set; }

    public virtual DbSet<TblNewsComment> TblNewsComments { get; set; }

    public virtual DbSet<TblReport> TblReports { get; set; }

    public virtual DbSet<TblReportImage> TblReportImages { get; set; }

    public virtual DbSet<TblReward> TblRewards { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserBadge> TblUserBadges { get; set; }

    public virtual DbSet<TblUserReward> TblUserRewards { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBadge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Badg__3213E83F73897F8C");

            entity.ToTable("tbl_Badges");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.RequiredReports)
                .HasDefaultValue(0)
                .HasColumnName("required_reports");
        });

        modelBuilder.Entity<TblCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Cate__3213E83F70A4B246");

            entity.ToTable("tbl_Categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("icon");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TblComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Comm__3213E83FFB9ADDB2");

            entity.ToTable("tbl_Comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Report).WithMany(p => p.TblComments)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK__tbl_Comme__repor__7E37BEF6");

            entity.HasOne(d => d.User).WithMany(p => p.TblComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Comme__user___7F2BE32F");
        });

        modelBuilder.Entity<TblDepartment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Depa__3213E83FB93E69E6");

            entity.ToTable("tbl_Departments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_email");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TblFundCampaign>(entity =>
        {
            entity.ToTable("TblFundCampaign");

            entity.Property(e => e.CurrentAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("ACTIVE");
            entity.Property(e => e.TargetAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<TblFundTransaction>(entity =>
        {
            entity.ToTable("TblFundTransaction");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderInfo).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING");
            entity.Property(e => e.VnpayTranId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Campaign).WithMany(p => p.TblFundTransactions)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundTransaction_Campaign");
        });

        modelBuilder.Entity<TblInteraction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Inte__3213E83F59F26B79");

            entity.ToTable("tbl_Interactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Report).WithMany(p => p.TblInteractions)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK__tbl_Inter__repor__797309D9");

            entity.HasOne(d => d.User).WithMany(p => p.TblInteractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Inter__user___7A672E12");
        });

        modelBuilder.Entity<TblMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Menu__3213E83F7CB44E24");

            entity.ToTable("tbl_Menus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("icon");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.RequiredRoleId).HasColumnName("required_role_id");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("url");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__tbl_Menus__paren__5EBF139D");

            entity.HasOne(d => d.RequiredRole).WithMany(p => p.TblMenus)
                .HasForeignKey(d => d.RequiredRoleId)
                .HasConstraintName("FK__tbl_Menus__requi__5FB337D6");
        });

        modelBuilder.Entity<TblNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblNews__3214EC079A93AE0A");

            entity.Property(e => e.CommentsCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LikesCount).HasDefaultValue(0);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<TblNewsComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblNewsC__3214EC07F7101A2D");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.News).WithMany(p => p.TblNewsComments)
                .HasForeignKey(d => d.NewsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TblNewsCo__NewsI__1BC821DD");
        });

        modelBuilder.Entity<TblReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Repo__3213E83F5697BCD2");

            entity.ToTable("tbl_Reports");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(11, 8)")
                .HasColumnName("longitude");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("datetime")
                .HasColumnName("resolved_at");
            entity.Property(e => e.Severity)
                .HasDefaultValue((byte)1)
                .HasColumnName("severity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WardCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ward_code");

            entity.HasOne(d => d.Category).WithMany(p => p.TblReports)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Repor__categ__71D1E811");

            entity.HasOne(d => d.Department).WithMany(p => p.TblReports)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__tbl_Repor__depar__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.TblReports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Repor__user___70DDC3D8");
        });

        modelBuilder.Entity<TblReportImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Repo__3213E83FB95F5BD9");

            entity.ToTable("tbl_Report_Images");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.ReportId).HasColumnName("report_id");

            entity.HasOne(d => d.Report).WithMany(p => p.TblReportImages)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK__tbl_Repor__repor__75A278F5");
        });

        modelBuilder.Entity<TblReward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Rewa__3213E83F91A9FAF1");

            entity.ToTable("tbl_Rewards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.PointCost).HasColumnName("point_cost");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValue(0)
                .HasColumnName("stock_quantity");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_Role__3213E83F339C478A");

            entity.ToTable("tbl_Roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_User__3213E83F14787C0A");

            entity.ToTable("tbl_Users");

            entity.HasIndex(e => e.Email, "UQ__tbl_User__AB6E6164C70A20BC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");
            entity.Property(e => e.TrustPoints)
                .HasDefaultValue(0)
                .HasColumnName("trust_points");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Users__role___59FA5E80");
        });

        modelBuilder.Entity<TblUserBadge>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BadgeId }).HasName("PK__tbl_User__C7C7BE6AD844F7E2");

            entity.ToTable("tbl_User_Badges");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.BadgeId).HasColumnName("badge_id");
            entity.Property(e => e.EarnedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("earned_at");

            entity.HasOne(d => d.Badge).WithMany(p => p.TblUserBadges)
                .HasForeignKey(d => d.BadgeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_User___badge__6477ECF3");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserBadges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_User___user___6383C8BA");
        });

        modelBuilder.Entity<TblUserReward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_User__3213E83FA3034873");

            entity.ToTable("tbl_User_Rewards");

            entity.HasIndex(e => e.VoucherCode, "UQ__tbl_User__217310695608E43D").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RedeemedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("redeemed_at");
            entity.Property(e => e.RewardId).HasColumnName("reward_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("voucher_code");

            entity.HasOne(d => d.Reward).WithMany(p => p.TblUserRewards)
                .HasForeignKey(d => d.RewardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_User___rewar__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserRewards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_User___user___693CA210");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
