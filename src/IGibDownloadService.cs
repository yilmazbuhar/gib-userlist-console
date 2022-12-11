using System.Diagnostics;

namespace GibUserSync
{
    public interface IGibDownloadService
    {
        Task<Stream> GetNewUserGbList();
        Task<Stream> GetNewUserPkList();
    }

    public class GibDownloadService : IGibDownloadService
    {
        private readonly HttpClient _httpClient;
        public GibDownloadService(HttpClient httpClient)
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
