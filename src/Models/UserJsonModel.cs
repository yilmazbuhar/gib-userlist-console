using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GibUserSync
{
    internal class UserJsonModel
    {
        public string Alias { get; set; }
        public string AliasCreationTime { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public string AppType { get; set; }
        public string FirstCreationTime { get; set; }
        public string GibAliasType { get; set; }
        public string GibUserType { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }
    }
}
