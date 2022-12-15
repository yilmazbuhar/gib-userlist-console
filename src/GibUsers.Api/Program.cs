using GibUsers.Api;
using Hangfire;

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
//await hangfirejob.GibUsersSync();
RecurringJob.AddOrUpdate<IHangfireJobs>(service => hangfirejob.GibUsersSync(), Cron.Hourly);

app.MapPost("/todoitems", async (string todo) =>
{
    //db.Todos.Add(todo);
    //await db.SaveChangesAsync();

    //return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.Run();


//async Task AddUserFromXmlNode(string xml)
//{
//    if (string.IsNullOrEmpty(xml))
//        return;

//    var serializer = new XmlSerializer(typeof(UserXml));

//    using (TextReader reader = new StringReader(xml))
//    {
//        var userXml = (UserXml)serializer.Deserialize(reader);

//        if (userXml == null)
//            return;

//        UserJsonModel baseuser = new UserJsonModel(userXml);

//        foreach (var doc in userXml.Documents.Document)
//        {
//            var alias = doc.Alias.FirstOrDefault(x => x.DeletionTime == null) ??
//                doc.Alias.OrderByDescending(x => x.DeletionTime).FirstOrDefault();

//            var user = (UserJsonModel)baseuser.Clone();

//            if (alias?.DeletionTime != null)
//                user.DeactivateDate = alias.DeletionTime;

//            user.AliasCreationTime = alias.CreationTime;
//            user.AppType = doc.Type;
//            user.Alias = alias.Name;

//            // set id programmatically to update existing data
//            // we assume that the Identifier is unique
//            user.Id = $"{userXml.Identifier}{doc.Type.ToLower(CultureInfo.InvariantCulture)}";

//            users.Add(user);
//        }
//    }
//}