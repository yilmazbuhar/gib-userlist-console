using GibUsers.Api;
using GibUsers.Api.ElasticSearch;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddApplicationServices(builder.Configuration)
    .AddElasticClient(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

var hangfirejob = app.Services.GetService<IHangfireJobs>();

app.Services.AddHangFireJobs(builder.Configuration);

app.MapGet("/api/search/{identifier}", async (string identifier, IElasticService elasticService) =>
{
    return Results.Ok(await elasticService.Search(identifier));
});

app.Run();