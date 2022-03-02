namespace GibUserSync
{
    internal class UserJsonModel : ICloneable
    {
        public UserJsonModel(UserXml userXml)
        {
            Identifier = userXml.Identifier;
            FirstCreationTime = userXml.FirstCreationTime;
            Title = userXml.Title;
            GibUserType = userXml.Type;
        }

        public string Id { get; set; }
        public string Alias { get; set; }
        public DateTime? AliasCreationTime { get; set; }
        public DateTime? DeactivateDate { get; set; } = DateTime.MinValue;
        public string AppType { get; set; }
        public string FirstCreationTime { get; set; }
        public string GibAliasType { get; set; }
        public string GibUserType { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
