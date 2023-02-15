using EWS_NetCore_Scheduler.Service;
DotEnv.Load(@"..\Server\data\Creds.txt");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
/*builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
var app = builder.Build();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
var config =
    new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();
app.Run();
