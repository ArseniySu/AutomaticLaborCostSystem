using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutomaticLaborCostSystem.DataBase;

public partial class AutoassemblyContext : DbContext
{
    public AutoassemblyContext()
    {
    }

    public AutoassemblyContext(DbContextOptions<AutoassemblyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveWindow> ActiveWindows { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<MissedDay> MissedDays { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<ProductionCalendar> ProductionCalendars { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StatusTask> StatusTasks { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskCompletion> TaskCompletions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkingHour> WorkingHours { get; set; }

    public virtual DbSet<WriteDown> WriteDowns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-6697S0P\\SQLEXPRESS;Database=autoassembly;Trusted_Connection=True; Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveWindow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActiveWi__3213E83F1B344FEE");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.IdWorkingHoursNavigation).WithMany(p => p.ActiveWindows)
                .HasForeignKey(d => d.IdWorkingHours)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActiveWin__IdWor__47DBAE45");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3213E83F80726FA7");

            entity.ToTable("Department");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<MissedDay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MissedDa__3213E83F90247F24");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DatetimeFinish).HasColumnType("date");
            entity.Property(e => e.DatetimeStart).HasColumnType("date");
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.IdUsersNavigation).WithMany(p => p.MissedDays)
                .HasForeignKey(d => d.IdUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MissedDay__IdUse__534D60F1");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3213E83FF4EAAEF9");

            entity.ToTable("Post");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<ProductionCalendar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producti__3213E83FA05EE4F6");

            entity.ToTable("ProductionCalendar");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date_).HasColumnName("Date_");
            entity.Property(e => e.NormHours).HasMaxLength(100);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3213E83F9DC324FE");

            entity.ToTable("Project");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DatetimeFinish).HasColumnType("datetime");
            entity.Property(e => e.DatetimeStart).HasColumnType("datetime");
            entity.Property(e => e.PlannedEndDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F8E5DCEC2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<StatusTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusTa__3213E83FC026F942");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3213E83F984CF4A9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdDepartment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tasks__IdDepartm__5070F446");

            entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdProject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tasks__IdProject__4E88ABD4");

            entity.HasOne(d => d.IdStatusTasksNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdStatusTasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tasks__IdStatusT__4F7CD00D");
        });

        modelBuilder.Entity<TaskCompletion>(entity =>
        {
            entity.HasKey(e => new { e.IdUsers, e.IdTasks }).HasName("PK__TaskComp__C900E2D307E5A1C9");

            entity.ToTable("TaskCompletion");

            entity.Property(e => e.DatetimeFinish).HasColumnType("datetime");
            entity.Property(e => e.DatetimeStart).HasColumnType("datetime");

            entity.HasOne(d => d.IdTasksNavigation).WithMany(p => p.TaskCompletions)
                .HasForeignKey(d => d.IdTasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaskCompl__IdTas__571DF1D5");

            entity.HasOne(d => d.IdUsersNavigation).WithMany(p => p.TaskCompletions)
                .HasForeignKey(d => d.IdUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaskCompl__IdUse__5629CD9C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F0B7F8CFC");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Logins)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MiddlName).HasMaxLength(100);
            entity.Property(e => e.Passwords)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SecondName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("Status_");
            entity.Property(e => e.Surname).HasMaxLength(100);

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdDepartment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__IdDepartm__3E52440B");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__IdPost__3F466844");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__IdRole__3D5E1FD2");
        });

        modelBuilder.Entity<WorkingHour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkingH__3213E83F956E7A76");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DatetimeFinish).HasColumnType("datetime");
            entity.Property(e => e.DatetimeStart).HasColumnType("datetime");

            entity.HasOne(d => d.IdUsersNavigation).WithMany(p => p.WorkingHours)
                .HasForeignKey(d => d.IdUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkingHo__IdUse__4222D4EF");
        });

        modelBuilder.Entity<WriteDown>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WriteDow__3213E83F6BEE8376");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.IdWorkingHoursNavigation).WithMany(p => p.WriteDowns)
                .HasForeignKey(d => d.IdWorkingHours)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WriteDown__IdWor__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
