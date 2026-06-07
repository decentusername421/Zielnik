using Microsoft.AspNetCore.Identity;
using Zielnik.Data;
using Zielnik.Entities;

public static class SeedData
{
    public static void Initialize(ZielnikDbContext context)

    {
        var hasher = new PasswordHasher<IdentityUser>();

        IdentityUser EnsureUser(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    EmailConfirmed = true
                };

                user.PasswordHash =
                    hasher.HashPassword(user, "Zielnik123!");

                context.Users.Add(user);
                context.SaveChanges();
            }

            return user;
        }

        var admin = EnsureUser("admin@zielnik.pl");
        var anna = EnsureUser("anna@zielnik.pl");
        var jan = EnsureUser("jan@zielnik.pl");

        if (!context.Roles.Any(r => r.Name == "Admin"))
        {
            context.Roles.Add(new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            });
        }

        if (!context.Roles.Any(r => r.Name == "User"))
        {
            context.Roles.Add(new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER"
            });

        }

        context.SaveChanges();

        var adminRole = context.Roles.First(r => r.Name == "Admin");
        var userRole = context.Roles.First(r => r.Name == "User");

        if (!context.UserRoles.Any(x =>
            x.UserId == admin.Id &&
            x.RoleId == adminRole.Id))
        {
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            });
        }

        if (!context.UserRoles.Any(x =>
            x.UserId == anna.Id &&
            x.RoleId == userRole.Id))
        {
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = anna.Id,
                RoleId = userRole.Id
            });
        }

        if (!context.UserRoles.Any(x =>
            x.UserId == jan.Id &&
            x.RoleId == userRole.Id))
        {
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = jan.Id,
                RoleId = userRole.Id
            });
        }

        context.SaveChanges();



        var categoryNames = new[]
        {
            "Warzywa",
            "Pomidory",
            "Ogórki",
            "Papryki",

            "Owoce",
            "Drzewa",
            "Krzewy",

            "Zioła",

            "Ozdobne",
            "Byliny",
            "Trawy",

            "Jednoroczne",
            "Wieloletnie",
            "Dwuletnie",

            "Cebulkowe",
            "Pnącza"
        };

        foreach (var categoryName in categoryNames)
        {
            if (!context.PlantCategories.Any(c => c.Name == categoryName))
            {
                context.PlantCategories.Add(new PlantCategory
                {
                    Name = categoryName
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

        var annuals = context.PlantCategories.First(c => c.Name == "Jednoroczne");
        var biennial = context.PlantCategories.First(c => c.Name == "Dwuletnie");
        var longLived = context.PlantCategories.First(c => c.Name == "Wieloletnie");
        var perennials = context.PlantCategories.First(c => c.Name == "Byliny");
        var bulbs = context.PlantCategories.First(c => c.Name == "Cebulkowe");

        var herbs = context.PlantCategories.First(c => c.Name == "Zioła");
        var ornamental = context.PlantCategories.First(c => c.Name == "Ozdobne");
        var grasses = context.PlantCategories.First(c => c.Name == "Trawy");
        var climbers = context.PlantCategories.First(c => c.Name == "Pnącza");


        // Black Cherry
        if (!context.Plants.Any(p => p.Name == "Black Cherry"))
        {
            var plant = new Plant
            {
                Name = "Black Cherry",
                Species = "Solanum lycopersicum",
                WateringFrequencyDays = 3,
                FertilizingFrequencyDays = 30,
                SprayingFrequencyDays = 60,
                HarvestAfterDays = 120,
                Description = "Pomidor koktajlowy o ciemnych, słodkich owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(tomatoes);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // Malinowy Warszawski
        if (!context.Plants.Any(p => p.Name == "Malinowy Warszawski"))
        {
            var plant = new Plant
            {
                Name = "Malinowy Warszawski",
                Species = "Solanum lycopersicum",
                WateringFrequencyDays = 2,
                Description = "Polska odmiana o dużych malinowych owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(tomatoes);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // Bawole Serce
        if (!context.Plants.Any(p => p.Name == "Bawole Serce"))
        {
            var plant = new Plant
            {
                Name = "Bawole Serce",
                Species = "Solanum lycopersicum",
                WateringFrequencyDays = 2,
                Description = "Mięsiste owoce idealne do sałatek.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(tomatoes);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // San Marzano
        if (!context.Plants.Any(p => p.Name == "San Marzano"))
        {
            var plant = new Plant
            {
                Name = "San Marzano",
                Species = "Solanum lycopersicum",
                WateringFrequencyDays = 2,
                Description = "Włoska odmiana polecana do sosów i przetworów.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(tomatoes);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // Ogórek Śremski
        if (!context.Plants.Any(p => p.Name == "Ogórek Śremski"))
        {
            var plant = new Plant
            {
                Name = "Ogórek Śremski",
                Species = "Cucumis sativus",
                WateringFrequencyDays = 1,
                Description = "Popularna polska odmiana gruntowa.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(cucumbers);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // Julian F1
        if (!context.Plants.Any(p => p.Name == "Julian F1"))
        {
            var plant = new Plant
            {
                Name = "Julian F1",
                Species = "Cucumis sativus",
                WateringFrequencyDays = 1,
                Description = "Plenna odmiana ogórka do uprawy gruntowej.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(cucumbers);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // California Wonder
        if (!context.Plants.Any(p => p.Name == "California Wonder"))
        {
            var plant = new Plant
            {
                Name = "California Wonder",
                Species = "Capsicum annuum",
                WateringFrequencyDays = 3,
                Description = "Klasyczna odmiana papryki słodkiej.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(peppers);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // Marta Polka
        if (!context.Plants.Any(p => p.Name == "Marta Polka"))
        {
            var plant = new Plant
            {
                Name = "Marta Polka",
                Species = "Capsicum annuum",
                WateringFrequencyDays = 3,
                Description = "Polska odmiana papryki o czerwonych owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(vegetables);
            plant.Categories.Add(peppers);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }
        // Ligol
        if (!context.Plants.Any(p => p.Name == "Ligol"))
        {
            var plant = new Plant
            {
                Name = "Ligol",
                Species = "Malus domestica",
                WateringFrequencyDays = 7,
                Description = "Popularna polska odmiana jabłoni o dużych owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(trees);
            plant.Categories.Add(longLived);


            context.Plants.Add(plant);
        }

        // Szampion
        if (!context.Plants.Any(p => p.Name == "Szampion"))
        {
            var plant = new Plant
            {
                Name = "Szampion",
                Species = "Malus domestica",
                WateringFrequencyDays = 7,
                Description = "Deserowa odmiana jabłoni o słodkich owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(trees);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Antonówka Zwykła
        if (!context.Plants.Any(p => p.Name == "Antonówka Zwykła"))
        {
            var plant = new Plant
            {
                Name = "Antonówka Zwykła",
                Species = "Malus domestica",
                WateringFrequencyDays = 7,
                Description = "Klasyczna odmiana idealna na przetwory.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(trees);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Łutówka
        if (!context.Plants.Any(p => p.Name == "Łutówka"))
        {
            var plant = new Plant
            {
                Name = "Łutówka",
                Species = "Prunus cerasus",
                WateringFrequencyDays = 7,
                Description = "Najpopularniejsza odmiana wiśni w Polsce.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(trees);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Northstar
        if (!context.Plants.Any(p => p.Name == "Northstar"))
        {
            var plant = new Plant
            {
                Name = "Northstar",
                Species = "Prunus cerasus",
                WateringFrequencyDays = 7,
                Description = "Mrozoodporna odmiana wiśni o ciemnych owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(trees);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Polka
        if (!context.Plants.Any(p => p.Name == "Polka"))
        {
            var plant = new Plant
            {
                Name = "Polka",
                Species = "Rubus idaeus",
                WateringFrequencyDays = 3,
                Description = "Popularna odmiana maliny jesiennej.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(shrubs);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Laszka
        if (!context.Plants.Any(p => p.Name == "Laszka"))
        {
            var plant = new Plant
            {
                Name = "Laszka",
                Species = "Rubus idaeus",
                WateringFrequencyDays = 3,
                Description = "Wczesna odmiana malin o dużych owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(shrubs);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Bluecrop
        if (!context.Plants.Any(p => p.Name == "Bluecrop"))
        {
            var plant = new Plant
            {
                Name = "Bluecrop",
                Species = "Vaccinium corymbosum",
                WateringFrequencyDays = 3,
                FertilizingFrequencyDays = 30,
                SprayingFrequencyDays = 60,
                HarvestAfterDays = 120,
                Description = "Jedna z najpopularniejszych odmian borówki amerykańskiej.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(shrubs);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Chandler
        if (!context.Plants.Any(p => p.Name == "Chandler"))
        {
            var plant = new Plant
            {
                Name = "Chandler",
                Species = "Vaccinium corymbosum",
                WateringFrequencyDays = 3,
                Description = "Borówka o bardzo dużych owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(shrubs);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Bazylia Genovese
        if (!context.Plants.Any(p => p.Name == "Bazylia Genovese"))
        {
            var plant = new Plant
            {
                Name = "Bazylia Genovese",
                Species = "Ocimum basilicum",
                WateringFrequencyDays = 2,
                FertilizingFrequencyDays = 14,
                SprayingFrequencyDays = 30,
                HarvestAfterDays = 60,
                Description = "Klasyczna bazylia włoska.",
                IsCustomPlant = false
            };

            plant.Categories.Add(herbs);
            plant.Categories.Add(annuals);

            context.Plants.Add(plant);
        }

        // Mięta pieprzowa
        if (!context.Plants.Any(p => p.Name == "Mięta pieprzowa"))
        {
            var plant = new Plant
            {
                Name = "Mięta pieprzowa",
                Species = "Mentha × piperita",
                WateringFrequencyDays = 2,
                FertilizingFrequencyDays = 14,
                SprayingFrequencyDays = 40,
                HarvestAfterDays = 10,
                Description = "Aromatyczna bylina zielarska.",
                IsCustomPlant = false
            };

            plant.Categories.Add(herbs);
            plant.Categories.Add(perennials);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Rozmaryn lekarski
        if (!context.Plants.Any(p => p.Name == "Rozmaryn lekarski"))
        {
            var plant = new Plant
            {
                Name = "Rozmaryn lekarski",
                Species = "Salvia rosmarinus",

                WateringFrequencyDays = 4,
                FertilizingFrequencyDays = 30,
                SprayingFrequencyDays = 45,
                HarvestAfterDays = 90,
                Description = "Zioło śródziemnomorskie.",
                IsCustomPlant = false
            };

            plant.Categories.Add(herbs);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Tymianek pospolity
        if (!context.Plants.Any(p => p.Name == "Tymianek pospolity"))
        {
            var plant = new Plant
            {
                Name = "Tymianek pospolity",
                Species = "Thymus vulgaris",

                WateringFrequencyDays = 4,
                FertilizingFrequencyDays = 30,
                SprayingFrequencyDays = 45,
                HarvestAfterDays = 90,
                Description = "Niskie zioło o intensywnym aromacie.",
                IsCustomPlant = false
            };

            plant.Categories.Add(herbs);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Lawenda Hidcote Blue
        if (!context.Plants.Any(p => p.Name == "Lawenda Hidcote Blue"))
        {
            var plant = new Plant
            {
                Name = "Lawenda Hidcote Blue",
                Species = "Lavandula angustifolia",
                WateringFrequencyDays = 5,
                Description = "Popularna lawenda o fioletowych kwiatach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(ornamental);
            plant.Categories.Add(perennials);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Jeżówka purpurowa Magnus
        if (!context.Plants.Any(p => p.Name == "Jeżówka Magnus"))
        {
            var plant = new Plant
            {
                Name = "Jeżówka Magnus",
                Species = "Echinacea purpurea",
                WateringFrequencyDays = 4,
                Description = "Długo kwitnąca bylina miododajna.",
                IsCustomPlant = false
            };

            plant.Categories.Add(ornamental);
            plant.Categories.Add(perennials);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Funkia Patriot
        if (!context.Plants.Any(p => p.Name == "Funkia Patriot"))
        {
            var plant = new Plant
            {
                Name = "Funkia Patriot",
                Species = "Hosta hybrid",
                WateringFrequencyDays = 3,
                Description = "Popularna bylina do półcienia.",
                IsCustomPlant = false
            };

            plant.Categories.Add(ornamental);
            plant.Categories.Add(perennials);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Miskant Gracillimus
        if (!context.Plants.Any(p => p.Name == "Miskant Gracillimus"))
        {
            var plant = new Plant
            {
                Name = "Miskant Gracillimus",
                Species = "Miscanthus sinensis",
                WateringFrequencyDays = 5,
                Description = "Wysoka trawa ozdobna o przewieszających się liściach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(grasses);
            plant.Categories.Add(ornamental);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Rozplenica Hameln
        if (!context.Plants.Any(p => p.Name == "Rozplenica Hameln"))
        {
            var plant = new Plant
            {
                Name = "Rozplenica Hameln",
                Species = "Pennisetum alopecuroides",
                WateringFrequencyDays = 4,
                Description = "Zwarta trawa ozdobna z puszystymi kwiatostanami.",
                IsCustomPlant = false
            };

            plant.Categories.Add(grasses);
            plant.Categories.Add(ornamental);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Tulipan Queen of Night
        if (!context.Plants.Any(p => p.Name == "Tulipan Queen of Night"))
        {
            var plant = new Plant
            {
                Name = "Tulipan Queen of Night",
                Species = "Tulipa",
                WateringFrequencyDays = 7,
                Description = "Tulipan o bardzo ciemnych kwiatach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(bulbs);
            plant.Categories.Add(ornamental);

            context.Plants.Add(plant);
        }

        // Narcyz Ice Follies
        if (!context.Plants.Any(p => p.Name == "Narcyz Ice Follies"))
        {
            var plant = new Plant
            {
                Name = "Narcyz Ice Follies",
                Species = "Narcissus",
                WateringFrequencyDays = 7,
                Description = "Popularny biało-kremowy narcyz.",
                IsCustomPlant = false
            };

            plant.Categories.Add(bulbs);
            plant.Categories.Add(ornamental);

            context.Plants.Add(plant);
        }

        // Hiacynt Delft Blue
        if (!context.Plants.Any(p => p.Name == "Hiacynt Delft Blue"))
        {
            var plant = new Plant
            {
                Name = "Hiacynt Delft Blue",
                Species = "Hyacinthus orientalis",
                WateringFrequencyDays = 7,
                Description = "Niebieski hiacynt o intensywnym zapachu.",
                IsCustomPlant = false
            };

            plant.Categories.Add(bulbs);
            plant.Categories.Add(ornamental);

            context.Plants.Add(plant);
        }

        // Winorośl Regent
        if (!context.Plants.Any(p => p.Name == "Regent"))
        {
            var plant = new Plant
            {
                Name = "Regent",
                Species = "Vitis vinifera",
                WateringFrequencyDays = 5,
                Description = "Popularna odmiana winorośli odporna na choroby.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(climbers);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Winorośl Solaris
        if (!context.Plants.Any(p => p.Name == "Solaris"))
        {
            var plant = new Plant
            {
                Name = "Solaris",
                Species = "Vitis vinifera",
                WateringFrequencyDays = 5,
                Description = "Wczesna odmiana winorośli o słodkich owocach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(fruits);
            plant.Categories.Add(climbers);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Powojnik Jackmanii
        if (!context.Plants.Any(p => p.Name == "Jackmanii"))
        {
            var plant = new Plant
            {
                Name = "Jackmanii",
                Species = "Clematis × jackmanii",
                WateringFrequencyDays = 4,
                Description = "Klasyczny powojnik o dużych fioletowych kwiatach.",
                IsCustomPlant = false
            };

            plant.Categories.Add(ornamental);
            plant.Categories.Add(climbers);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Wiciokrzew pomorski
        if (!context.Plants.Any(p => p.Name == "Wiciokrzew pomorski"))
        {
            var plant = new Plant
            {
                Name = "Wiciokrzew pomorski",
                Species = "Lonicera periclymenum",
                WateringFrequencyDays = 4,
                Description = "Silnie pachnące pnącze ozdobne.",
                IsCustomPlant = false
            };

            plant.Categories.Add(ornamental);
            plant.Categories.Add(climbers);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }

        // Bluszcz pospolity
        if (!context.Plants.Any(p => p.Name == "Bluszcz pospolity"))
        {
            var plant = new Plant
            {
                Name = "Bluszcz pospolity",
                Species = "Hedera helix",
                WateringFrequencyDays = 7,
                Description = "Zimozielone pnącze okrywowe i ścienne.",
                IsCustomPlant = false
            };

            plant.Categories.Add(ornamental);
            plant.Categories.Add(climbers);
            plant.Categories.Add(longLived);

            context.Plants.Add(plant);
        }


        // Gardens

        var homeGarden = context.Gardens.FirstOrDefault(g => g.Name == "Ogród przydomowy");
        if (homeGarden == null)
        {
            homeGarden = new Garden
            {
                Name = "Ogród przydomowy",
                UserId = admin.Id
            };

            context.Gardens.Add(homeGarden);
        }

        var tunnel = context.Gardens.FirstOrDefault(g => g.Name == "Tunel foliowy");
        if (tunnel == null)
        {
            tunnel = new Garden
            {
                Name = "Tunel foliowy",
                UserId = jan.Id
            };

            context.Gardens.Add(tunnel);
        }

        var balcony = context.Gardens.FirstOrDefault(g => g.Name == "Balkon");
        if (balcony == null)
        {
            balcony = new Garden
            {
                Name = "Balkon",
                UserId = anna.Id
            };

            context.Gardens.Add(balcony);
        }

        var pergola = context.Gardens.FirstOrDefault(g => g.Name == "Pergola");
        if (pergola == null)
        {
            pergola = new Garden
            {
                Name = "Pergola",
                UserId = admin.Id
            };

            context.Gardens.Add(pergola);
        }
        context.SaveChanges();

        homeGarden.UserId = admin.Id;
        tunnel.UserId = jan.Id;
        balcony.UserId = anna.Id;
        pergola.UserId = admin.Id;

        context.SaveChanges();

        var annaBalcony = context.Gardens
    .First(g => g.Name == "Balkon");

        var janTunnel = context.Gardens
            .First(g => g.Name == "Tunel foliowy");

        var basil = context.Plants
            .First(p => p.Name == "Bazylia Genovese");

        var rosemary = context.Plants
            .First(p => p.Name == "Rozmaryn lekarski");


        var blueberry = context.Plants
            .First(p => p.Name == "Bluecrop");

        var blackCherry = context.Plants
    .First(p => p.Name == "Black Cherry");

        var malinowy = context.Plants
            .First(p => p.Name == "Malinowy Warszawski");

        var lawenda = context.Plants
            .First(p => p.Name == "Lawenda Hidcote Blue");

        var regent = context.Plants
            .First(p => p.Name == "Regent");

        var annaBasil = new UserPlant
        {
            PlantId = basil.Id,
            GardenId = annaBalcony.Id,

            Nickname = "Bazylia balkonowa",

            SowingDate = new DateTime(2025, 4, 10),
            PlantingDate = new DateTime(2025, 5, 1),

            Status = PlantStatus.Active
        };

        var annaRosemary = new UserPlant
        {
            PlantId = rosemary.Id,
            GardenId = annaBalcony.Id,

            Nickname = "Rozmaryn do kuchni",

            PlantingDate = new DateTime(2025, 4, 15),

            Status = PlantStatus.Active
        };

        var janTomato = new UserPlant
        {
            PlantId = blackCherry.Id,
            GardenId = janTunnel.Id,

            Nickname = "Pomidory do szklarni",

            SowingDate = new DateTime(2025, 3, 10),
            PlantingDate = new DateTime(2025, 4, 15),

            HarvestReminderDays = 3,
            NextHarvestReminder = DateTime.UtcNow.Date.AddDays(1),

            Status = PlantStatus.Active
        };

        var janBlueberry = new UserPlant
        {
            PlantId = blueberry.Id,
            GardenId = janTunnel.Id,

            Nickname = "Borówki",

            PlantingDate = new DateTime(2024, 4, 1),

            HarvestReminderDays = 7,
            NextHarvestReminder = DateTime.UtcNow.Date,

            Status = PlantStatus.Active
        };

        if (!context.UserPlants.Any(up => up.Nickname == "Bazylia balkonowa"))
        {
            context.UserPlants.Add(annaBasil);
        }

        if (!context.UserPlants.Any(up => up.Nickname == "Rozmaryn do kuchni"))
        {
            context.UserPlants.Add(annaRosemary);
        }

        if (!context.UserPlants.Any(up => up.Nickname == "Pomidory do szklarni"))
        {
            context.UserPlants.Add(janTomato);
        }

        if (!context.UserPlants.Any(up => up.Nickname == "Borówki"))
        {
            context.UserPlants.Add(janBlueberry);
        }

        context.SaveChanges();


        var annaBasilDb = context.UserPlants
        .First(up => up.Nickname == "Bazylia balkonowa");

        var annaRosemaryDb = context.UserPlants
           .First(up => up.Nickname == "Rozmaryn do kuchni");

        var janTomatoDb = context.UserPlants
           .First(up => up.Nickname == "Pomidory do szklarni");

        var janBlueberryDb = context.UserPlants
           .First(up => up.Nickname == "Borówki");

        if (!context.UserPlants.Any(up => up.Nickname == "Tunel 2025"))
        {
            context.UserPlants.Add(new UserPlant
            {
                PlantId = blackCherry.Id,
                GardenId = tunnel.Id,

                Nickname = "Tunel 2025",

                SowingDate = new DateTime(2025, 3, 15),
                PlantingDate = new DateTime(2025, 5, 1),

                Status = PlantStatus.Harvested
            });

        }
        if (!context.UserPlants.Any(up => up.Nickname == "Grządka południowa"))
        {
            context.UserPlants.Add(new UserPlant
            {
                PlantId = blackCherry.Id,
                GardenId = homeGarden.Id,

                Nickname = "Grządka południowa",

                SowingDate = new DateTime(2025, 4, 1),
                PlantingDate = new DateTime(2025, 5, 20),

                Status = PlantStatus.Active
            });
        }


        if (!context.UserPlants.Any(up => up.Nickname == "Eksperyment"))
        {
            context.UserPlants.Add(new UserPlant
            {
                PlantId = malinowy.Id,
                GardenId = tunnel.Id,

                Nickname = "Eksperyment",

                SowingDate = new DateTime(2025, 3, 10),
                PlantingDate = new DateTime(2025, 5, 2),

                Status = PlantStatus.Dead
            });
        }

        if (!context.UserPlants.Any(up => up.Nickname == "Rabata przy wejściu"))
        {
            context.UserPlants.Add(new UserPlant
            {
                PlantId = lawenda.Id,
                GardenId = homeGarden.Id,

                Nickname = "Rabata przy wejściu",

                Status = PlantStatus.Active
            });
        }

        if (!context.UserPlants.Any(up => up.Nickname == "Pergola południowa"))
        {
            context.UserPlants.Add(new UserPlant
            {
                PlantId = regent.Id,
                GardenId = pergola.Id,

                Nickname = "Pergola południowa",

                Status = PlantStatus.Active
            });
        }

        context.SaveChanges();



        if (!context.PlantTreatments.Any())

        {
            var treatmentLavender = context.UserPlants
             .FirstOrDefault(up => up.Nickname == "Rabata przy wejściu");

            context.PlantTreatments.AddRange(

                new PlantTreatment
                {
                    UserPlantId = janTomatoDb.Id,
                    TreatmentType = "Watering",
                    Quantity = 5,
                    Unit = "l",
                    PerformedAt = DateTime.UtcNow.AddDays(-2),
                    Notes = "Podlewanie po upalnym dniu."
                },

                new PlantTreatment
                {
                    UserPlantId = janTomatoDb.Id,
                    TreatmentType = "Fertilizing",
                    ProductName = "Biohumus",
                    Quantity = 50,
                    Unit = "ml",
                    PerformedAt = DateTime.UtcNow.AddDays(-15),
                    Notes = "Nawożenie wspomagające kwitnienie."
                },

                new PlantTreatment
                {
                    UserPlantId = janTomatoDb.Id,
                    TreatmentType = "Spraying",
                    ProductName = "Miedzian 50 WP",
                    Quantity = 10,
                    Unit = "g",
                    PerformedAt = DateTime.UtcNow.AddDays(-18),
                    Notes = "Profilaktyka przeciw chorobom grzybowym."
                },

                new PlantTreatment
                {
                    UserPlantId = janTomatoDb.Id,
                    TreatmentType = "Spraying",
                    ProductName = "Polyversum WP",
                    PerformedAt = DateTime.UtcNow.AddDays(-8),
                    Notes = "Oprysk przeciw zarazie ziemniaczanej."
                },

                new PlantTreatment
                {
                    UserPlantId = janBlueberryDb.Id,
                    TreatmentType = "Watering",
                    PerformedAt = DateTime.UtcNow.AddDays(-4),
                    Notes = "Podlewanie w okresie dojrzewania owoców."
                },

                new PlantTreatment
                {
                    UserPlantId = janBlueberryDb.Id,
                    TreatmentType = "Spraying",
                    ProductName = "Emulpar 940 EC",
                    PerformedAt = DateTime.UtcNow.AddDays(-11),
                    Notes = "Oprysk przeciw mszycom."
                },

                new PlantTreatment
                {
                    UserPlantId = janBlueberryDb.Id,
                    TreatmentType = "Spraying",
                    ProductName = "NeemAzal",
                    PerformedAt = DateTime.UtcNow.AddDays(-6),
                    Notes = "Zwalczanie szkodników."
                },

                new PlantTreatment
                {
                    UserPlantId = annaBasilDb.Id,
                    TreatmentType = "Watering",
                    PerformedAt = DateTime.UtcNow.AddDays(-2),
                    Notes = "Regularne podlewanie doniczki balkonowej."
                }

           );

            if (treatmentLavender != null)
            {
                context.PlantTreatments.Add(
                    new PlantTreatment
                    {
                        UserPlantId = treatmentLavender.Id,
                        TreatmentType = "Pruning",
                        PerformedAt = DateTime.UtcNow.AddDays(-7),
                        Notes = "Usunięto przekwitłe pędy."
                    });
            }

            context.SaveChanges();
        }

        if (!context.Harvests.Any())
        {
            var harvestTomato = context.UserPlants
                .FirstOrDefault(up => up.Nickname == "Tunel 2025");

            if (harvestTomato != null)
            {
                // przypomnienia o zbiorach
                harvestTomato.HarvestReminderDays = 3;
                harvestTomato.NextHarvestReminder =
                    DateTime.UtcNow.Date.AddDays(2);

                context.Harvests.AddRange(
                    new Harvest
                    {
                        UserPlantId = harvestTomato.Id,
                        HarvestDate = DateTime.UtcNow.AddDays(-30),
                        Quantity = 1.7m,
                        Unit = "kg",
                        FruitsCount = 28,
                        Notes = "Pierwszy zbiór dojrzałych owoców Black Cherry."
                    },
                    new Harvest
                    {
                        UserPlantId = harvestTomato.Id,
                        HarvestDate = DateTime.UtcNow.AddDays(-20),
                        Quantity = 2.5m,
                        Unit = "kg",
                        FruitsCount = 45,
                        Notes = "Regularny zbiór w szczycie sezonu."
                    },
                    new Harvest
                    {
                        UserPlantId = harvestTomato.Id,
                        HarvestDate = DateTime.UtcNow.AddDays(-10),
                        Quantity = 3.8m,
                        Unit = "kg",
                        FruitsCount = 62,
                        Notes = "Największy zbiór w sezonie."
                    },
                    new Harvest
                    {
                        UserPlantId = harvestTomato.Id,
                        HarvestDate = DateTime.UtcNow.AddDays(-3),
                        Quantity = 1.7m,
                        Unit = "kg",
                        FruitsCount = 28,
                        Notes = "Końcówka sezonu."
                    }
                );
            }


            var blueberryPlant = context.UserPlants
                .FirstOrDefault(up => up.Nickname == "Borówki");

            if (blueberryPlant != null)
            {
                blueberryPlant.HarvestReminderDays = 7;
                blueberryPlant.NextHarvestReminder =
                    DateTime.UtcNow.Date.AddDays(5);

                context.Harvests.AddRange(
                    new Harvest
                    {
                        UserPlantId = blueberryPlant.Id,
                        HarvestDate = DateTime.UtcNow.AddDays(-14),
                        Quantity = 0.6m,
                        Unit = "kg",
                        FruitsCount = 180,
                        Notes = "Pierwszy zbiór borówek."
                    },
                    new Harvest
                    {
                        UserPlantId = blueberryPlant.Id,
                        HarvestDate = DateTime.UtcNow.AddDays(-7),
                        Quantity = 0.9m,
                        Unit = "kg",
                        FruitsCount = 260,
                        Notes = "Najlepszy zbiór w sezonie."
                    }
                );
            }

            context.SaveChanges();
        }
    

        if (!context.PlantNotes.Any(n =>
            n.UserPlantId == annaBasilDb.Id &&
            n.CreatedAt == new DateTime(2025, 5, 10)))
        {
        context.PlantNotes.AddRange(

    new PlantNote
    {
        UserPlantId = annaBasilDb.Id,
        Title = "Pierwsze liście",
        Content = "Bazylia zaczęła szybko rosnąć.",
        CreatedAt = new DateTime(2025, 5, 10)
    },

    new PlantNote
    {
        UserPlantId = annaBasilDb.Id,
        Title = "Przesadzenie",
        Content = "Przeniesiona do większej doniczki.",
        CreatedAt = new DateTime(2025, 5, 20)
    },

    new PlantNote
    {
        UserPlantId = annaRosemaryDb.Id,
        Title = "Dobra kondycja",
        Content = "Rozmaryn bardzo dobrze znosi upały.",
        CreatedAt = new DateTime(2025, 6, 1)
    }
);
        if (!context.PlantPhotos.Any())
        {
            var photoTomato = context.UserPlants
                .FirstOrDefault(up => up.Nickname == "Tunel 2025");

            var photoLavender = context.UserPlants
                .FirstOrDefault(up => up.Nickname == "Rabata przy wejściu");

            if (photoTomato != null)
            {
                context.PlantPhotos.AddRange(
                    new PlantPhoto
                    {
                        UserPlantId = photoTomato.Id,
                        FilePath = "/photos/tomato-1.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new PlantPhoto
                    {
                        UserPlantId = photoTomato.Id,
                        FilePath = "/photos/tomato-2.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    }
                );
            }

            if (photoLavender != null)
            {
                context.PlantPhotos.Add(
                    new PlantPhoto
                    {
                        UserPlantId = photoLavender.Id,
                        FilePath = "/photos/lavender-1.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-7)
                    });
            }

            context.SaveChanges();

        }
            context.PlantTreatments.AddRange(

    new PlantTreatment
    {
        UserPlantId = annaBasilDb.Id,
        TreatmentType = "Watering",
        Notes = "Obfite podlewanie",
        PerformedAt = new DateTime(2025, 5, 12)
    },

    new PlantTreatment
    {
        UserPlantId = annaBasilDb.Id,
        TreatmentType = "Fertilizing",
        ProductName = "Biohumus",
        PerformedAt = new DateTime(2025, 5, 25)
    },

    new PlantTreatment
    {
        UserPlantId = janTomatoDb.Id,
        TreatmentType = "Spraying",
        ProductName = "Miedzian",
        PerformedAt = new DateTime(2025, 6, 10)
    },

    new PlantTreatment
    {
        UserPlantId = janTomatoDb.Id,
        TreatmentType = "Watering",
        PerformedAt = new DateTime(2025, 6, 12)
    }
);
        
        context.Harvests.AddRange(

    new Harvest
    {
        UserPlantId = janTomatoDb.Id,
        HarvestDate = new DateTime(2025, 8, 1),
        Quantity = 2.5m,
        Unit = "kg",
        FruitsCount = 40
    },

    new Harvest
    {
        UserPlantId = janBlueberryDb.Id,
        HarvestDate = new DateTime(2025, 7, 15),
        Quantity = 1.8m,
        Unit = "kg"
    }
); context.PlantPhotos.AddRange(

    new PlantPhoto
    {
        UserPlantId = annaBasilDb.Id,
        FilePath = "photos/basil_1.jpg",
        CreatedAt = new DateTime(2025, 5, 15)
    },

    new PlantPhoto
    {
        UserPlantId = janTomatoDb.Id,
        FilePath = "photos/tomato_1.jpg",
        CreatedAt = new DateTime(2025, 6, 20)
    }
);

        context.SaveChanges();
        }
    }
}
