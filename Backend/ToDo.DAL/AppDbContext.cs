using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.DAL.Models;

namespace ToDo.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);


            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TaskItems");

                entity.HasKey(x => x.Id);

                // Title: varchar(100), required
                entity.Property(x => x.Title)
                      .IsRequired()
                      .HasColumnType("varchar(100)");

                // Description: varchar(200), optional
                entity.Property(x => x.Description)
                      .HasColumnType("varchar(200)")
                      .IsRequired(false);

                // IsComplete: required (bool is non-nullable)
                entity.Property(x => x.IsComplete)
                      .IsRequired();

                // CreatedAt: required, default value now
                entity.Property(x => x.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(x => x.UserId)
                      .IsRequired(false);

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
