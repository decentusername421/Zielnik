using Zielnik.Entities;

namespace Zielnik.Data
{
    public static class SeedData
    {
        public static void Seed(ZielnikDbContext context)
        {
            if (context.PlantCategories.Any())
                return;

            var categories = new List<PlantCategory>
            {
                new() { Name = "Warzywa" },
                new() { Name = "Pomidory" },
                new() { Name = "Ogórki" },
                new() { Name = "Papryki" },
                new() { Name = "Owoce" },
                new() { Name = "Drzewa" },
                new() { Name = "Krzewy" },
                new() { Name = "Zioła" },
                new() { Name = "Ozdobne" },
                new() { Name = "Byliny" },
                new() { Name = "Trawy" },
                new() { Name = "Jednoroczne" },
                new() { Name = "Wieloletnie" },
                new() { Name = "Dwuletnie" },
                new() { Name = "Cebulkowe" },
                new() { Name = "Pnącza" }
            };

            context.PlantCategories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
