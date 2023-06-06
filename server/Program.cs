using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.Authentication.Negotiate;

DotEnv.Load(@"..\Server\data\Creds.txt");

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader();                         
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//win auth

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();
builder.Services.AddAuthorization(options=>
    {
        options.FallbackPolicy = options.DefaultPolicy;
    });

builder.Services.AddHttpContextAccessor();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();


app.MapControllers();

app.MapControllers().AllowAnonymous();

var config =
    new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

app.Run();
