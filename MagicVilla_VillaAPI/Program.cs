
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt",rollingInterval:RollingInterval.Day).CreateLogger();
//builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(
    q => {
        q.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
        });
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(q =>
{
    q.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    q.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata=false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllers(q => { 
    //q.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT\n" +
        "Enter 'Bearer' [your token]\n" +
        "Example: \"Bearer 123456\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"

    });
    option.AddSecurityRequirement( new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            { 
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="oauth2",
                Name="Bearer",
                In=ParameterLocation.Header,
            }, 
            new List<string>()
        }
    });
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Magic Villa",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://www.facebook.com/"),
        Contact = new OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("https://www.facebook.com/")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://www.facebook.com/")
        }
    });
    option.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Magic Villa V2",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://www.facebook.com/"),
        Contact = new OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("https://www.facebook.com/")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://www.facebook.com/")
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
          option.SwaggerEndpoint("/swagger/v1/swagger.json","Magic_VillaV1");
          option.SwaggerEndpoint("/swagger/v2/swagger.json","Magic_VillaV2");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
