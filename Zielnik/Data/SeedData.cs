using Zielnik.Entities;

namespace Zielnik.Data
{
    public static class SeedData
    {
        public static void Seed(ZielnikDbContext context)
        {
            if (context.PlantCategories.Any()) return;

            var categoryNames = new[]
            {
                "Warzywa", "Owoce", "Kwiaty", "Drzewa",
                "Pomidory", "Ogórki", "Papryki",
                "Krzewy", "Zioła", "Ozdobne",
                "Byliny", "Trawy", "Jednoroczne",
                "Wieloletnie", "Dwuletnie",
                "Cebulkowe", "Pnącza"
            };

            foreach (var name in categoryNames)
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
        }
    }
}
