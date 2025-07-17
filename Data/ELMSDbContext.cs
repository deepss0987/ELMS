using ELMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELMS.Data
{
    public class ELMSDbContext : DbContext
    {
        public ELMSDbContext(DbContextOptions<ELMSDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveBudget> LeaveBudgets { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveRequestStatus> LeaveRequestStatuses { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EmployeeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveBudgetTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveRequestTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveRequestStatusTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveTypeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleTypeConfiguration());

        }

        internal class EmployeeTypeConfiguration : IEntityTypeConfiguration<Employee>
        {
            public void Configure(EntityTypeBuilder<Employee> builder)
            {
                builder.ToTable("Employee");
                builder.HasKey(m => m.Id);

                builder.HasOne(m => m.Role)
                    .WithMany()
                    .HasForeignKey(m => m.RoleId);

                builder.HasOne(m => m.Manager)
                     .WithMany()
                     .HasForeignKey(m => m.ManagerId);
            }
        }
        internal class LeaveBudgetTypeConfiguration : IEntityTypeConfiguration<LeaveBudget>
        {
            public void Configure(EntityTypeBuilder<LeaveBudget> builder)
            {
                builder.ToTable("LeaveBudget");
                builder.HasKey(m => m.Id);

                builder.HasOne(m => m.Employee)
                    .WithMany()
                    .HasForeignKey(m => m.EmployeeId);

                builder.HasOne(m => m.LeaveType)
                    .WithMany()
                    .HasForeignKey(m => m.LeaveTypeId);
            }
        }
        internal class LeaveRequestTypeConfiguration : IEntityTypeConfiguration<LeaveRequest>
        {
            public void Configure(EntityTypeBuilder<LeaveRequest> builder)
            {
                builder.ToTable("LeaveRequest");
                builder.HasKey(m => m.Id);

                builder.HasOne(m => m.Employee)
                    .WithMany()
                    .HasForeignKey(m => m.EmployeeId);

                builder.HasOne(m => m.LeaveType)
                    .WithMany()
                    .HasForeignKey(m => m.LeaveTypeId);

                builder.HasOne(m => m.Status)
                    .WithMany()
                    .HasForeignKey(m => m.StatusId);

            }
        }
        internal class LeaveRequestStatusTypeConfiguration : IEntityTypeConfiguration<LeaveRequestStatus>
        {
            public void Configure(EntityTypeBuilder<LeaveRequestStatus> builder)
            {
                builder.ToTable("LeaveRequestStatus");
                builder.HasKey(m => m.Id);
            }
        }
        internal class LeaveTypeTypeConfiguration : IEntityTypeConfiguration<LeaveType>
        {
            public void Configure(EntityTypeBuilder<LeaveType> builder)
            {
                builder.ToTable("LeaveType");
                builder.HasKey(m => m.Id);
            }
        }
        internal class RoleTypeConfiguration : IEntityTypeConfiguration<Role>
        {
            public void Configure(EntityTypeBuilder<Role> builder)
            {
                builder.ToTable("Role");
                builder.HasKey(m => m.Id);
            }
        }
    }
}

