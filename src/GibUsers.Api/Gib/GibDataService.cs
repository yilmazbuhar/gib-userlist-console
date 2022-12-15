using System.IO.Compression;

namespace GibUsers.Api
{
    public class GibDataService : IGibDataService
    {
        private readonly HttpClient _httpClient;
        public GibDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        async Task<MemoryStream> UnzipStreamFile(Stream zipStream)
        {
            var unzipStream = new MemoryStream();
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            using var entryStream = zipArchive.Entries[0].Open();
            await entryStream.CopyToAsync(unzipStream);
            unzipStream.Position = 0;
            return unzipStream;
        }

        //https://merkeztest.efatura.gov.tr
        public async Task<Stream> GetNewUserGbList()
        {
            return await GetUserList($"/EFaturaMerkez/newUserGbListxml.zip");
        }

        public async Task<Stream> GetNewUserPkList()
        {
            return await GetUserList($"/EFaturaMerkez/newUserPkListxml.zip");
        }

        private async Task<Stream> GetUserList(string path)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
            var httpResult = await _httpClient.SendAsync(requestMessage);
            var zipstream = await httpResult.Content.ReadAsStreamAsync();
            //var zipstream = new FileStream("newUserPkListxml.zip", FileMode.Open); //await httpResult.Content.ReadAsStreamAsync();

            return await UnzipStreamFile(zipstream);
        }
    }
}
