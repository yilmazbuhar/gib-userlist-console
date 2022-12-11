using GibUsers.Api;
using Hangfire;
using System.Globalization;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Hangfire
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfire")));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

RecurringJob.AddOrUpdate<IGibDownloadService>(service => service.GetNewUserPkList(), Cron.Hourly);

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateTime.Now.AddDays(index),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");



app.Run();


async Task AddUserFromXmlNode(string xml)
{
    if (string.IsNullOrEmpty(xml))
        return;

    var serializer = new XmlSerializer(typeof(UserXml));

    using (TextReader reader = new StringReader(xml))
    {
        var userXml = (UserXml)serializer.Deserialize(reader);

        if (userXml == null)
            return;

        UserJsonModel baseuser = new UserJsonModel(userXml);

        foreach (var doc in userXml.Documents.Document)
        {
            var alias = doc.Alias.FirstOrDefault(x => x.DeletionTime == null) ??
                doc.Alias.OrderByDescending(x => x.DeletionTime).FirstOrDefault();

            var user = (UserJsonModel)baseuser.Clone();

            if (alias?.DeletionTime != null)
                user.DeactivateDate = alias.DeletionTime;

            user.AliasCreationTime = alias.CreationTime;
            user.AppType = doc.Type;
            user.Alias = alias.Name;

            // set id programmatically to update existing data
            // we assume that the Identifier is unique
            user.Id = $"{userXml.Identifier}{doc.Type.ToLower(CultureInfo.InvariantCulture)}";

            users.Add(user);
        }
    }
}

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary);

internal class WeatherForecastAlt {

    public DateOnly Date { get; init; }
    public int Temperature { get; init; }
    public string Summary { get; init; }
}