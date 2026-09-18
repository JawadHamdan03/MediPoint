using System.Threading.RateLimiting;
using MediPoint.Api.Registerations;
using MediPoint.Application.Common.Services;
using MediPoint.Infrastructure.Common.Services;
using MediPoint.Infrastructure.Common.Utils;
using MediPoint.Infrastructure.Data;
using MediPoint.Infrastructure.Ai;
using MediPoint.Infrastructure.Common.Jobs;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbRegisteration(builder.Configuration);
builder.Services.AddEmailRegisteration(builder.Configuration);
builder.Services.AddExceptionHandlingRegisertations();
builder.Services.AddApplicationLayerRegisteration();
builder.Services.AddAuthRegisterations(builder.Configuration);
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter("concurrent", opt =>
    {
        opt.PermitLimit = 3;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddScoped<IJwtTokenServiceProvider, JwtTokenServiceProvider>();

builder.Services.AddOpenAiChatClient(builder.Configuration);
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 100;
});


builder.Services.AddHostedService<NotifyAdminsPeriodiclyJob>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();

    await UsersDataSeed.SeedUsers(services);
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("concurrent");




app.Run();

