using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAuthApi.Context;
using UserAuthApi.Services;

namespace UserAuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Konfigurera JWT-inst�llningar
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });


            builder.Services.AddScoped<UserService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddDbContext<AppDbContext>(options => // Konfigurera DbContext f�r att anv�nda SQL Server
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

            

            //Skapar en Configuration Builder som kan h�mta enskilda v�rden fr�n appsettings.json.
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            //H�mtar v�r connection string inuti appsettings.json med ConfigurationBuilder objektet
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //Med v�r connection string skapar vi en DbContextOption, allts� en inst�llning f�r v�r databas.
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
             .UseSqlServer(connectionString)
             .Options;

            // Skapar ett objekt av ApplDbContext genom att skicka in v�ra inst�llningar som inneh�ller connection stringen.
            using var dbContext = new AppDbContext(contextOptions);


            var app = builder.Build();

            
            // Skapa databasen och till�mpa eventuella migrationer
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); 

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
