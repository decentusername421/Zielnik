using Microsoft.EntityFrameworkCore;
using Zielnik.Entities;
using Zielnik.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Zielnik.Data
{
    public class ZielnikDbContext : IdentityDbContext
    {
        public ZielnikDbContext(
            DbContextOptions<ZielnikDbContext> options)
            : base(options)
        {
        }
        public DbSet<Garden> Gardens { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<PlantCategory> PlantCategories { get; set; }
        public DbSet<UserPlant> UserPlants { get; set; }

        public DbSet<PlantNote> PlantNotes { get; set; }

        public DbSet<PlantTreatment> PlantTreatments { get; set; }

        public DbSet<Harvest> Harvests { get; set; }

        public DbSet<PlantPhoto> PlantPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Plant -> UserPlant (1:N)
            modelBuilder.Entity<UserPlant>()
                .HasOne(up => up.Plant)
                .WithMany(p => p.UserPlants)
                .HasForeignKey(up => up.PlantId);

            // Garden -> UserPlant (1:N)
            modelBuilder.Entity<UserPlant>()
                .HasOne(up => up.Garden)
                .WithMany(g => g.Plants)
                .HasForeignKey(up => up.GardenId);

            // UserPlant -> Notes (1:N)
            modelBuilder.Entity<PlantNote>()
                .HasOne(n => n.UserPlant)
                .WithMany(up => up.Notes)
                .HasForeignKey(n => n.UserPlantId);

            // UserPlant -> Treatments (1:N)
            modelBuilder.Entity<PlantTreatment>()
                .HasOne(t => t.UserPlant)
                .WithMany(up => up.Treatments)
                .HasForeignKey(t => t.UserPlantId);

            // UserPlant -> Harvests (1:N)
            modelBuilder.Entity<Harvest>()
                .HasOne(h => h.UserPlant)
                .WithMany(up => up.Harvests)
                .HasForeignKey(h => h.UserPlantId);

            // UserPlant -> Photos (1:N)
            modelBuilder.Entity<PlantPhoto>()
                .HasOne(p => p.UserPlant)
                .WithMany(up => up.Photos)
                .HasForeignKey(p => p.UserPlantId);

            modelBuilder.Entity<UserPlant>()
    .Property(up => up.SowingDate)
    .HasColumnType("date");

            modelBuilder.Entity<UserPlant>()
                .Property(up => up.PlantingDate)
                .HasColumnType("date");
        }
    }
}
