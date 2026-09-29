
using DriverLicenseApi.Data;
using DriverLicenseApi.IServices;
using DriverLicenseApi.Middleware;
using DriverLicenseApi.Models;
using DriverLicenseApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace DriverLicenseApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"))
                );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "DriverLicenseApi")
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true
                    },
                    columnOptions: new ColumnOptions
                    {
                        AdditionalColumns = new Collection<SqlColumn>
                        {
                new SqlColumn
                {
                    ColumnName = "Application",
                    PropertyName = "Application",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 100
                }
                        }
                    })
                .CreateLogger();

            builder.Host.UseSerilog();
            

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();                      

            
            var jwtSecret = builder.Configuration["JWT:Secret"];
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!))
                };
            });

            builder.Services.AddHttpClient<IExternalVerificationService, ExternalVerificationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ILicenseApplicationService, LicenseApplicationService>();
            
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Driver License API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\". Enter your JWT token."
                });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] =
                            new List<string>()
                    });
            });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
                await DbSeeder.SeedRolesAndUsersAsync(services);
            }

            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
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
