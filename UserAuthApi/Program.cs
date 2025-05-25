using Microsoft.EntityFrameworkCore;
using UserAuthApi.Models;

using Microsoft.EntityFrameworkCore;

namespace UserAuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options => // Konfigurera DbContext för att använda SQL Server
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Skapar en Configuration Builder som kan hämta enskilda värden från appsettings.json.
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            //Hämtar vår connection string inuti appsettings.json med ConfigurationBuilder objektet
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //Med vår connection string skapar vi en DbContextOption, alltså en inställning för vår databas.
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
             .UseSqlServer(connectionString)
             .Options;

            // Skapar ett objekt av ApplDbContext genom att skicka in våra inställningar som innehåller connection stringen.
            using var dbContext = new AppDbContext(contextOptions);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            // Skapa databasen och tillämpa eventuella migrationer
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
