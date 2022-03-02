using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GibUserSync
{
    internal class ElasticSearchConfig
    {
        public string Host { get; set; }
        public string Index { get; set; }
        public int BulkInsertCount { get; set; } = 5000;
    }
}
