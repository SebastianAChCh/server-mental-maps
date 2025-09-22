using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using serber_mental_maps.middleware;
using server_mental_maps.DataBase;
using server_mental_maps.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<MongoDb>((op) =>
{ 
    op.UseMongoDB(builder.Configuration["MentalMaps:ConnectionString"] ?? "");
});

builder.Services.AddAuthentication().AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration["MentalMaps:ConnectionString"] ?? ""));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("MentalMapsDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<RenewalToken>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
