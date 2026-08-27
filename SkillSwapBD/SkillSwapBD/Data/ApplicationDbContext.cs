using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSwapBD.Models;
using System.Reflection.Emit;

namespace SkillSwapBD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Skill>().HasOne(s => s.User).WithMany(u => u.Skills)
                .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Skill>().HasOne(s => s.Category).WithMany(c => c.Skills)
                .HasForeignKey(s => s.CategoryId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SwapRequest>().HasOne(sr => sr.Requester).WithMany()
                .HasForeignKey(sr => sr.RequesterId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SwapRequest>().HasOne(sr => sr.Receiver).WithMany()
                .HasForeignKey(sr => sr.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SwapRequest>().HasOne(sr => sr.RequesterSkill).WithMany()
                .HasForeignKey(sr => sr.RequesterSkillId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SwapRequest>().HasOne(sr => sr.ReceiverSkill).WithMany()
                .HasForeignKey(sr => sr.ReceiverSkillId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>().HasOne(r => r.Reviewer).WithMany()
                .HasForeignKey(r => r.ReviewerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Review>().HasOne(r => r.Reviewee).WithMany()
                .HasForeignKey(r => r.RevieweeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Review>().HasOne(r => r.SwapRequest).WithMany(sr => sr.Reviews)
                .HasForeignKey(r => r.SwapRequestId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Review>().HasIndex(r => new { r.SwapRequestId, r.ReviewerId }).IsUnique();

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming & IT" },
                new Category { Id = 2, Name = "Languages" },
                new Category { Id = 3, Name = "Music" },
                new Category { Id = 4, Name = "Design & Art" }
                );
        }
    
    }
}
