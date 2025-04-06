using CurrencyXchange.Business.Factory;
using CurrencyXchange.Business.Services.Exchange;
using CurrencyXchange.Data.CurrencyConverter;
using System.Text.Json.Serialization;
using OpenTelemetry.Trace;
using Serilog.Events;
using Serilog;
using CurrencyXChange.Middleware;
using AspNetCoreRateLimit;
using CurrencyXChange.Authentication;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        builder.Services.AddMemoryCache();

        //Rate Limiting

        builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // Add OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();// Can be intrgrated with some tracing system like Jaeger, Zipkin, etc.
            });

        //Ignore null values in JSON serialization
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        //Options
        builder.Services.AddOptions<FrankfurterApiOptions>()
            .Bind(builder.Configuration.GetSection("FrankfurterApiOptions"))
            .ValidateDataAnnotations();

        // Frankfurter Client
        builder.Services.AddHttpClient<FrankfurterCurrencyConvertorClient>();

        // Currency Convertor Fasctory
        builder.Services.AddSingleton<ICurrencyConvertorFactory, CurrencyConvertorFactory>();


        //Services
        builder.Services.AddSingleton<IExchangeServices, ExchangeServices>();

        //JWT
        builder.Services.AddSingleton<JwtTokenGenerator>();
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });



        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine("Serilog ERROR: " + msg));
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .CreateLogger();

        builder.Host.UseSerilog();


        //SWagger

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CurrencyXchange API", Version = "v1" });

            // Add JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token like: Bearer {your token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });

        var app = builder.Build();

        app.UseIpRateLimiting();

        app.UseMiddleware<HttpRequestLoggingMiddleware>();

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