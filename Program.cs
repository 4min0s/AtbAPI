using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApi.Models;
using TodoApi.Services;




var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DigiBankContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddScoped<PdfGenerationService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHostedService<EcheanceProcessorService>();



// adjust paths to match your setup
builder.Services.AddSingleton<CreditSimulatorService>(_ =>
    new CreditSimulatorService("D:\\codes\\projetExcel\\TodoApi\\Assets\\model de calcul crédit.xlsx"));



builder.Services.AddSingleton<PdfGeneratorService>(_ =>
    new PdfGeneratorService(
        xlsxPath: "D:\\codes\\projetExcel\\TodoApi\\Assets\\model de calcul crédit.xlsx",
        outputRootFolder: "D:\\codes\\projetExcel\\TodoApi\\Documents\\",
        logoPath: "D:\\codes\\projetExcel\\TodoApi\\Assets\\logo.jpg"
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
// Expose le dossier Documents comme fichiers statiques
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Documents")
    ),
    RequestPath = "/documents"
});


app.UseHttpsRedirection();

app.UseAuthentication(); // must be before UseAuthorization


app.UseAuthorization();
app.MapControllers();



app.Run();