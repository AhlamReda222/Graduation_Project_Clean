using Graduation_Project.DAL.DataBase;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Graduation_Project.BLL.Seeders
{
    public static class PrintingTechniqueSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.PrintingTechniques.AnyAsync()) return;

            var techniques = new List<PrintingTechnique>
            {
                new() { TechniqueType = PrintingTechniqueType.EmbroideryS, Name = "Embroidery - S", Dimensions = "7x7cm",   Price = 125 },
                new() { TechniqueType = PrintingTechniqueType.EmbroideryM, Name = "Embroidery - M", Dimensions = "14x14cm", Price = 175 },
                new() { TechniqueType = PrintingTechniqueType.DTF_S,       Name = "DTF - S",        Dimensions = "10x10cm", Price = 35  },
                new() { TechniqueType = PrintingTechniqueType.DTF_M,       Name = "DTF - M",        Dimensions = "20x20cm", Price = 45  },
                new() { TechniqueType = PrintingTechniqueType.DTF_L,       Name = "DTF - L",        Dimensions = "30x30cm", Price = 55  },
                new() { TechniqueType = PrintingTechniqueType.DTF_XL,      Name = "DTF - XL",       Dimensions = "40x50cm", Price = 75  },
                new() { TechniqueType = PrintingTechniqueType.DTF_Column,  Name = "DTF - Column",   Dimensions = "15x40cm", Price = 55  },
                new() { TechniqueType = PrintingTechniqueType.DTF_Row,     Name = "DTF - Row",      Dimensions = "40x15cm", Price = 55  },
            };

            context.PrintingTechniques.AddRange(techniques);
            await context.SaveChangesAsync();
        }
    }
}