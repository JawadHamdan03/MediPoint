using System.Text;
using FluentValidation;
using MediatR;
using MediPoint.Api.Exceptions;
using MediPoint.Application;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Behaviors;
using MediPoint.Application.Common.Services;

using MediPoint.Infrastructure.Common.Services;
using MediPoint.Infrastructure.Common.Utils;
using MediPoint.Infrastructure.Data;
using MediPoint.Infrastructure.MongoData;
using MediPoint.Infrastructure.MongoData.Services;
using MediPoint.Infrastructure.Ai;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IAppDbContext,AppDbContext>();
builder.Services.Configure<MongoDbContext>(
    builder.Configuration.GetSection("MediPoint"));
builder.Services.AddSingleton<IMedicalRecordsService,MedicalRecordService>();
builder.Services.AddSingleton<IMedicineService,MedicineService>();
builder.Services.AddSingleton<ILabResultService,LabResultService>();

builder.Services.AddOpenAiChatClient(builder.Configuration);


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddScoped<IJwtTokenServiceProvider, JwtTokenServiceProvider>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId",context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId",activity?.Id);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();

builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));

builder.Services.AddAuthentication(options =>
{
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var JwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = JwtSettings["SecretKey"];
     options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings["Issuer"],
        ValidAudience = JwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),

    };
});

builder.Services.AddAuthorization();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();




app.Run();

