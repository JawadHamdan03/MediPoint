using MediPoint.Application.Common.Services;
using MediPoint.Infrastructure.Common.Services;
using MediPoint.Infrastructure.Common.Utils;
using MediPoint.Infrastructure.Data;
using MediPoint.Infrastructure.MongoData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<MongoDbContext>(
    builder.Configuration.GetSection("MediPoint"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddScoped<IJwtTokenServiceProvider, JwtTokenServiceProvider>();

builder.Services.AddOpenApi();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
     var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    // Apply pending migrations
    await dbContext.Database.MigrateAsync();

    // Seed roles and admin user
    await UsersDataSeed.SeedUsers(services);
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

