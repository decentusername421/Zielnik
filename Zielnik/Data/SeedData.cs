using Zielnik.Data;
using Zielnik.Entities;

namespace Zielnik.Data
{
    public static class SeedData
    {
        public static void Seed(ZielnikDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.PlantCategories.Any())
                return;

            var categories = new[]
            {
                "Warzywa", "Pomidory", "Ogórki", "Papryki",
                "Owoce", "Drzewa", "Krzewy",
                "Zioła",
                "Ozdobne", "Byliny", "Trawy",
                "Jednoroczne", "Wieloletnie", "Dwuletnie",
                "Cebulkowe", "Pnącza"
            };

            foreach (var name in categories)
            {
                if (!context.PlantCategories.Any(c => c.Name == name))
                {
                    context.PlantCategories.Add(new PlantCategory { Name = name });
                }
            }

            context.SaveChanges();

            var vegetables = context.PlantCategories.First(c => c.Name == "Warzywa");
            var tomatoes = context.PlantCategories.First(c => c.Name == "Pomidory");
            var cucumbers = context.PlantCategories.First(c => c.Name == "Ogórki");
            var peppers = context.PlantCategories.First(c => c.Name == "Papryki");

            var fruits = context.PlantCategories.First(c => c.Name == "Owoce");
            var trees = context.PlantCategories.First(c => c.Name == "Drzewa");
            var shrubs = context.PlantCategories.First(c => c.Name == "Krzewy");

            var herbs = context.PlantCategories.First(c => c.Name == "Zioła");
            var ornamental = context.PlantCategories.First(c => c.Name == "Ozdobne");
            var grasses = context.PlantCategories.First(c => c.Name == "Trawy");

            var annuals = context.PlantCategories.First(c => c.Name == "Jednoroczne");
            var longLived = context.PlantCategories.First(c => c.Name == "Wieloletnie");
            var perennials = context.PlantCategories.First(c => c.Name == "Byliny");
            var bulbs = context.PlantCategories.First(c => c.Name == "Cebulkowe");
            var climbers = context.PlantCategories.First(c => c.Name == "Pnącza");

            // przykładowa roślina
            if (!context.Plants.Any(p => p.Name == "Black Cherry"))
            {
                var plant = new Plant
                {
                    Name = "Black Cherry",
                    Species = "Solanum lycopersicum",
                    WateringFrequencyDays = 2,
                    IsCustomPlant = false
                };

                plant.Categories.Add(vegetables);
                plant.Categories.Add(tomatoes);
                plant.Categories.Add(annuals);

                context.Plants.Add(plant);
            }

            context.SaveChanges();
        }
    }
}
