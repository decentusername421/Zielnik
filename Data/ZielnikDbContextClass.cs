using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zielnik.Entities;

namespace Zielnik.Data
{
    public class ZielnikDbContext : IdentityDbContext<IdentityUser>
    {
        public ZielnikDbContext(DbContextOptions<ZielnikDbContext> options)
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

            modelBuilder.Entity<UserPlant>()
                .HasOne(x => x.Plant)
                .WithMany(x => x.UserPlants)
                .HasForeignKey(x => x.PlantId);

            modelBuilder.Entity<UserPlant>()
                .HasOne(x => x.Garden)
                .WithMany(x => x.Plants)
                .HasForeignKey(x => x.GardenId);

            modelBuilder.Entity<PlantNote>()
                .HasOne(x => x.UserPlant)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.UserPlantId);

            modelBuilder.Entity<PlantTreatment>()
                .HasOne(x => x.UserPlant)
                .WithMany(x => x.Treatments)
                .HasForeignKey(x => x.UserPlantId);

            modelBuilder.Entity<Harvest>()
                .HasOne(x => x.UserPlant)
                .WithMany(x => x.Harvests)
                .HasForeignKey(x => x.UserPlantId);

            modelBuilder.Entity<PlantPhoto>()
                .HasOne(x => x.UserPlant)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.UserPlantId);

            modelBuilder.Entity<UserPlant>()
                .Property(x => x.SowingDate)
                .HasColumnType("date");

            modelBuilder.Entity<UserPlant>()
                .Property(x => x.PlantingDate)
                .HasColumnType("date");
        }
    }
}