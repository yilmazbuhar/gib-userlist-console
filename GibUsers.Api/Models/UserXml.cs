using System.Xml.Serialization;

namespace GibUsers.Api
{
	[XmlRoot(ElementName = "User")]
	public class UserXml
	{
		[XmlElement(ElementName = "Identifier")]
		public string Identifier { get; set; }
		[XmlElement(ElementName = "Title")]
		public string Title { get; set; }
		[XmlElement(ElementName = "Type")]
		public string Type { get; set; }
		[XmlElement(ElementName = "FirstCreationTime")]
		public string FirstCreationTime { get; set; }
		[XmlElement(ElementName = "AccountType")]
		public string AccountType { get; set; }
		[XmlElement(ElementName = "Documents")]
		public Documents Documents { get; set; }
	}

	[XmlRoot(ElementName = "Documents")]
	public class Documents
	{
		[XmlElement(ElementName = "Document")]
		public List<Document> Document { get; set; }
	}

	[XmlRoot(ElementName = "Document")]
	public class Document
	{
		[XmlElement(ElementName = "Alias")]
		public List<Alias> Alias { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "Alias")]
	public class Alias
	{
		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "CreationTime")]
		public DateTime? CreationTime { get; set; }
		[XmlElement(ElementName = "DeletionTime")]
		public DateTime? DeletionTime { get; set; }
	}
}