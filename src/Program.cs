// See https://aka.ms/new-console-template for more information
using GibUserSync;
using System.Xml;
using System.Xml.Serialization;

using (XmlReader reader = XmlReader.Create(@"gibuser.xml"))
{
    int i = 0;
    while (reader.Read())
    {
        if (reader.IsStartElement() && reader.Name.ToString() == "User")
        {
            var user = GetUser(reader.ReadOuterXml());
            Console.WriteLine(user.Identifier);

            Console.WriteLine(i++);
        }

    }
}
Console.ReadKey();

UserJsonModel GetUser(string xml)
{
    if (string.IsNullOrEmpty(xml))
        return null;

    var serializer = new XmlSerializer(typeof(User));

    using (TextReader reader = new StringReader(xml))
    {
        var userXml = (User)serializer.Deserialize(reader);
        UserJsonModel user = new UserJsonModel
        {
            Identifier = userXml.Identifier,
            FirstCreationTime = userXml.FirstCreationTime,
            Title = userXml.Title,
            AppType = userXml.Type
        };

        foreach (var doc in userXml.Documents.Document)
        {
            var alias = doc.Alias.FirstOrDefault(x => x.DeletionTime == null) ??
                doc.Alias.OrderByDescending(x => x.DeletionTime).FirstOrDefault();

            if (alias?.DeletionTime != null)
                user.DeactivateDate = alias.DeletionTime;

        }

        return user;
    }
}