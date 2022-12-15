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
            var response = await _elasticsearchClient.IndexManyAsync(users);
            
            return response;
        }

        public Task<List<UserJsonModel>> Search(string term)
        {
            throw new NotImplementedException();
        }
    }
}
