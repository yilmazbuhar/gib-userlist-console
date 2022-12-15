namespace GibUsers.Api
{
    public interface ISyncService
    {
        Task SyncDataAsync(Stream stream);
    }
}
