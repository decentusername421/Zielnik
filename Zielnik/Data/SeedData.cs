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
                    context.PlantCategories.Add(new PlantCategory
                    {
                        Name = name
                    });
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

            // BLACK CHERRY
            if (!context.Plants.Any(p => p.Name == "Black Cherry"))
            {
                var plant = new Plant
                {
                    Name = "Black Cherry",
                    Species = "Solanum lycopersicum",
                    WateringFrequencyDays = 2
                };

                plant.Categories.Add(vegetables);
                plant.Categories.Add(tomatoes);
                plant.Categories.Add(annuals);

                context.Plants.Add(plant);
            }

            // MALINOWY
            if (!context.Plants.Any(p => p.Name == "Malinowy Warszawski"))
            {
                var plant = new Plant
                {
                    Name = "Malinowy Warszawski",
                    Species = "Solanum lycopersicum",
                    WateringFrequencyDays = 2
                };

                plant.Categories.Add(vegetables);
                plant.Categories.Add(tomatoes);
                plant.Categories.Add(annuals);

                context.Plants.Add(plant);
            }

            // OGÓREK
            if (!context.Plants.Any(p => p.Name == "Ogórek Śremski"))
            {
                var plant = new Plant
                {
                    Name = "Ogórek Śremski",
                    Species = "Cucumis sativus",
                    WateringFrequencyDays = 1
                };

                plant.Categories.Add(vegetables);
                plant.Categories.Add(cucumbers);
                plant.Categories.Add(annuals);

                context.Plants.Add(plant);
            }

            // PAPRYKA
            if (!context.Plants.Any(p => p.Name == "California Wonder"))
            {
                var plant = new Plant
                {
                    Name = "California Wonder",
                    Species = "Capsicum annuum",
                    WateringFrequencyDays = 3
                };

                plant.Categories.Add(vegetables);
                plant.Categories.Add(peppers);
                plant.Categories.Add(annuals);

                context.Plants.Add(plant);
            }

            // LIGOL
            if (!context.Plants.Any(p => p.Name == "Ligol"))
            {
                var plant = new Plant
                {
                    Name = "Ligol",
                    Species = "Malus domestica",
                    WateringFrequencyDays = 7
                };

                plant.Categories.Add(fruits);
                plant.Categories.Add(trees);
                plant.Categories.Add(longLived);

                context.Plants.Add(plant);
            }

            // BAZYLIA
            if (!context.Plants.Any(p => p.Name == "Bazylia Genovese"))
            {
                var plant = new Plant
                {
                    Name = "Bazylia Genovese",
                    Species = "Ocimum basilicum",
                    WateringFrequencyDays = 2
                };

                plant.Categories.Add(herbs);
                plant.Categories.Add(annuals);

                context.Plants.Add(plant);
            }

            context.SaveChanges();
        }
    }
}
