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
                .Must(m => m
                .QueryString(qs => qs
                .DefaultField("_all")
                .Query(term))))));

            return query.Documents.ToList();
        }
    }
}
