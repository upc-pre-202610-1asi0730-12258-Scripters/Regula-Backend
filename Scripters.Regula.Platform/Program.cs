using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scripters.Regula.Platform.Billing.Application.CommandServices;
using Scripters.Regula.Platform.Billing.Application.Internal.CommandServices;
using Scripters.Regula.Platform.Billing.Application.Internal.OutboundServices;
using Scripters.Regula.Platform.Billing.Application.Internal.QueryServices;
using Scripters.Regula.Platform.Billing.Application.QueryServices;
using Scripters.Regula.Platform.Billing.Domain.Repositories;
using Scripters.Regula.Platform.Billing.Infrastructure.ExternalServices.StripeGateway;
using Scripters.Regula.Platform.Billing.Infrastructure.Persistence.EFC.Repositories;
using Scripters.Regula.Platform.CommercialManagement.Application.CommandServices;
using Scripters.Regula.Platform.CommercialManagement.Application.Internal.CommandServices;
using Scripters.Regula.Platform.CommercialManagement.Domain.Repositories;
using Scripters.Regula.Platform.CommercialManagement.Infrastructure.Persistence.EFC.Repositories;
using Scripters.Regula.Platform.CommercialManagement.Application.Internal.QueryServices;
using Scripters.Regula.Platform.CommercialManagement.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Application.CommandServices;
using Scripters.Regula.Platform.DeliveryTracking.Application.Internal.CommandServices;
using Scripters.Regula.Platform.DeliveryTracking.Application.Internal.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Application.QueryServices;
using Scripters.Regula.Platform.DeliveryTracking.Domain.Repositories;
using Scripters.Regula.Platform.DeliveryTracking.Infrastructure.Persistence.EFC.Repositories;
using Scripters.Regula.Platform.Iam.Application.Internal.CommandServices;
using Scripters.Regula.Platform.Iam.Application.Internal.OutboundServices;
using Scripters.Regula.Platform.Iam.Domain.Repositories;
using Scripters.Regula.Platform.Iam.Infrastructure.Hashing.BCrypt;
using Scripters.Regula.Platform.Iam.Infrastructure.Persistence.EFC.Repositories;
using Scripters.Regula.Platform.Iam.Infrastructure.Tokens.JWT;
using Scripters.Regula.Platform.InventoryManagement.Application.CommandServices;
using Scripters.Regula.Platform.InventoryManagement.Application.Internal.CommandServices;
using Scripters.Regula.Platform.InventoryManagement.Application.Internal.QueryServices;
using Scripters.Regula.Platform.InventoryManagement.Application.QueryServices;
using Scripters.Regula.Platform.InventoryManagement.Domain.Repositories;
using Scripters.Regula.Platform.InventoryManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Scripters.Regula.Platform.Shared.Domain.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Scripters.Regula.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Scripters.Regula.Platform.Shared.Infrastructure.Pipeline.Middleware.Components;
using Scripters.Regula.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Cortex.Mediator.DependencyInjection;
using Microsoft.OpenApi;

/// <summary>
/// The main entry point for the Scripters.Regula.Platform application.
/// Configures services, middleware, and the HTTP request pipeline.
/// </summary>
public class Program
{
    /// <summary>
    /// The main method that starts the application.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Required by all IStringLocalizer<T> instances in the project
        // (e.g., InventoryCommandService, GlobalExceptionHandlerMiddleware, ProblemDetailsFactory).
        builder.Services.AddLocalization();

        // Enables IMediator.PublishAsync and scans this assembly for IEventHandler<T>
        // (e.g., UserRegisteredEventHandler). Without this, domain events written
        // in the project would never be dispatched.
        builder.Services.AddCortexMediator(
            new[] { typeof(Program) },
            options => options.AddDefaultBehaviors());

        // Add CORS Policy.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        // Configure Database.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionStringTemplate))
                throw new InvalidOperationException("Database connection string is not set in the configuration.");

            var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Database connection string is not set in the configuration.");

            options.UseMySQL(connectionString)
                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
                .EnableDetailedErrors();

            if (builder.Environment.IsDevelopment())
                options.EnableSensitiveDataLogging();
        });

        // Inventory Management Bounded Context services.
        builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
        builder.Services.AddScoped<IInventoryCommandService, InventoryCommandService>();
        builder.Services.AddScoped<IInventoryQueryService, InventoryQueryService>();

        // Delivery Tracking Bounded Context services.
        builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
        builder.Services.AddScoped<IDeliveryLocationQueryService, DeliveryLocationQueryService>();
        builder.Services.AddScoped<IDeliveryQueryService, DeliveryQueryService>();
        builder.Services.AddScoped<IDeliveryCommandService, DeliveryCommandService>();
        builder.Services.AddScoped<IDeliveryResponsibleRepository, DeliveryResponsibleRepository>();
        builder.Services.AddScoped<IDeliveryVehicleRepository, DeliveryVehicleRepository>();

        // Commercial Management Bounded Context services.
        builder.Services.AddScoped<ICommercialCustomerRepository, CommercialCustomerRepository>();
        builder.Services.AddScoped<ICommercialDebtRepository, CommercialDebtRepository>();
        builder.Services.AddScoped<ICustomerDebtCommandService, CustomerDebtCommandService>();
        builder.Services.AddScoped<ICommercialDebtPaymentRepository, CommercialDebtPaymentRepository>();
        builder.Services.AddScoped<ICommercialDailySaleRepository, CommercialDailySaleRepository>();
        builder.Services.AddScoped<IDailySaleCommandService, DailySaleCommandService>();
        builder.Services.AddScoped<IDailySaleQueryService, DailySaleQueryService>();

        // IAM Bounded Context services.
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserCommandService, UserCommandService>();
        builder.Services.AddScoped<IHashingService, HashingService>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        // Shared Bounded Context services.
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<ProblemDetailsFactory>();

        // Billing Bounded Context (Stripe subscriptions) services.
        builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        builder.Services.AddScoped<IStripeService, StripeService>();
        builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
        builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

        // Configure Authentication.
        var jwtSecret = builder.Configuration["JwtSettings:Secret"];

        if (!string.IsNullOrWhiteSpace(jwtSecret))
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                    };
                });
        }

        // Configure Swagger/OpenAPI.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            // Without this, Swagger does not show the "Authorize" button or send the
            // Authorization header in "Try it out", even if the API already validates JWT.
            // NOTE: Microsoft.OpenApi 2.x (which Swashbuckle.AspNetCore 10.x includes in
            // .NET 10) moved these classes from Microsoft.OpenApi.Models to Microsoft.OpenApi,
            // and OpenApiSecurityScheme no longer has the "Reference" property — now the
            // schema is referenced with OpenApiSecuritySchemeReference.
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste only the token (without the word 'Bearer', Swagger adds it automatically)."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        // Must be first to catch any exceptions from the rest of the pipeline.
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        // Apply CORS Policy.
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Apply database migrations on startup.
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }

        app.Run();
    }
}