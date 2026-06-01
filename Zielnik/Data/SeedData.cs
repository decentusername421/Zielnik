using Zielnik.Entities;

namespace Zielnik.Data
{
    public static class SeedData
    {
        public static void Seed(ZielnikDbContext context)
        {
            if (context.PlantCategories.Any()) return;

            var categories = new List<PlantCategory>
            {
                new PlantCategory { Name = "Warzywa" },
                new PlantCategory { Name = "Owoce" },
                new PlantCategory { Name = "Kwiaty" },
                new PlantCategory { Name = "Drzewa" }
            };

            context.PlantCategories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
