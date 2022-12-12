﻿namespace GibUsers.Api
{
    public class ElasticSearchConfig
    {
        public string Host { get; set; }
        public string Index { get; set; }
        public int BulkInsertCount { get; set; } = 5000;
    }
}
