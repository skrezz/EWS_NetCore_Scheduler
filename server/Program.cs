using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;

DotEnv.Load(@"..\Server\data\Creds.txt");

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder  =>
                      {
                          builder.WithOrigins("https://localhost:5152", "https://localhost:9080", "https://localhost:7151")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();                         
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthService.iss,
            ValidateAudience = true,
            ValidAudience = AuthService.aud,
            ValidateLifetime = true,
            IssuerSigningKey = AuthService.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true            
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
var config =
    new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();
app.Run();
