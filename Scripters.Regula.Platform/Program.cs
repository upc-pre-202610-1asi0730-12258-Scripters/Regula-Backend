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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Requerido por todos los IStringLocalizer<T> del proyecto
// (InventoryCommandService, GlobalExceptionHandlerMiddleware, ProblemDetailsFactory, etc.)
builder.Services.AddLocalization();

// Habilita IMediator.PublishAsync y escanea este assembly en busca de
// IEventHandler<T> (p.ej. UserRegisteredEventHandler). Sin esto, los eventos
// de dominio ya escritos en el proyecto nunca se despachaban a nadie.
builder.Services.AddCortexMediator(
    new[] { typeof(Program) },
    options => options.AddDefaultBehaviors());

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// Configure Database
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

// Inventory Management Bounded Context
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryCommandService, InventoryCommandService>();
builder.Services.AddScoped<IInventoryQueryService, InventoryQueryService>();

// Delivery Tracking Bounded Context
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
builder.Services.AddScoped<IDeliveryLocationQueryService, DeliveryLocationQueryService>();
builder.Services.AddScoped<IDeliveryQueryService, DeliveryQueryService>();
builder.Services.AddScoped<IDeliveryCommandService, DeliveryCommandService>();
builder.Services.AddScoped<IDeliveryResponsibleRepository, DeliveryResponsibleRepository>();
builder.Services.AddScoped<IDeliveryVehicleRepository, DeliveryVehicleRepository>();

// Commercial Management Bounded Context
builder.Services.AddScoped<ICommercialCustomerRepository, CommercialCustomerRepository>();
builder.Services.AddScoped<ICommercialDebtRepository, CommercialDebtRepository>();
builder.Services.AddScoped<ICustomerDebtCommandService, CustomerDebtCommandService>();
builder.Services.AddScoped<ICommercialDebtPaymentRepository, CommercialDebtPaymentRepository>();
builder.Services.AddScoped<ICommercialDailySaleRepository, CommercialDailySaleRepository>();
builder.Services.AddScoped<IDailySaleCommandService, DailySaleCommandService>();
builder.Services.AddScoped<IDailySaleQueryService, DailySaleQueryService>();

// IAM Bounded Context
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ProblemDetailsFactory>();

// Billing Bounded Context (Stripe subscriptions)
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

// Configure Authentication
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

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Sin esto, Swagger no muestra el botón "Authorize" ni manda el header
    // Authorization en "Try it out", aunque la API ya valide JWT.
    // NOTA: Microsoft.OpenApi 2.x (el que trae Swashbuckle.AspNetCore 10.x en
    // .NET 10) movió estas clases de Microsoft.OpenApi.Models a Microsoft.OpenApi,
    // y OpenApiSecurityScheme ya no tiene la propiedad "Reference" — ahora se
    // referencia el esquema con OpenApiSecuritySchemeReference.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega solo el token (sin la palabra 'Bearer', Swagger ya la agrega)."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Debe ir primero para capturar cualquier excepción del resto del pipeline
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Apply CORS Policy
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}
//ejecutar
app.Run();