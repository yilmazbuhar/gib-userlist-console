using System.Diagnostics;

namespace GibUsers.Api
{
    public interface IGibDataService
    {
        Task<Stream> GetNewUserGbList();
        Task<Stream> GetNewUserPkList();
    }
}
