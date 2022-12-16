using Nest;

namespace GibUsers.Api.ElasticSearch
{
    public interface IElasticService
    {
        Task<BulkResponse> BulkIndex(List<UserJsonModel>? users);
        Task<List<UserJsonModel>> Search(string term);
    }

    public class ElasticService : IElasticService
    {
        private readonly ElasticClient _elasticsearchClient;
        public ElasticService(ElasticClient elasticClient)
        {
            _elasticsearchClient = elasticClient;
        }

        public async Task<BulkResponse> BulkIndex(List<UserJsonModel>? users)
        {
            return await _elasticsearchClient.IndexManyAsync(users);
        }

        public async Task<List<UserJsonModel>> Search(string term)
        {
            var query = await _elasticsearchClient.SearchAsync<UserJsonModel>(s => s
            .From(0)
            .Take(10)
            .Query(qry => qry
                .Bool(b => b
                .Should(m => m.Match(qs => qs.Field("identifier.keyword").Query(term))))));

            //{"query":{"bool":{"must":[],"must_not":[],"should":[{"match":{"identifier.keyword":"67711226990"}}]}},"from":0,"size":1000,"sort":[],"aggs":{}}

            return query.Documents.ToList();
        }
    }
}
