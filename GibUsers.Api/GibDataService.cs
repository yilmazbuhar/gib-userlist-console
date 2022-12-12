namespace GibUsers.Api
{
    public class GibDataService : IGibDataService
    {
        private readonly HttpClient _httpClient;
        public GibDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //https://merkeztest.efatura.gov.tr
        public async Task<Stream> GetNewUserGbList()
        {
            return await DownloadUserList($"/EFaturaMerkez/newUserGbListxml.zip");
        }

        public async Task<Stream> GetNewUserPkList()
        {
            return await DownloadUserList($"/EFaturaMerkez/newUserPkListxml.zip");
        }

        private async Task<Stream> DownloadUserList(string path)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);

            var httpResult = await _httpClient.SendAsync(requestMessage);

            return await httpResult.Content.ReadAsStreamAsync();
        }
    }
}
