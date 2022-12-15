namespace GibUsers.Api
{
    public class HangfireJobs : IHangfireJobs
    {
        private readonly IGibDataService _gibDataService;
        private readonly ISyncService _syncService;
        public HangfireJobs(IGibDataService gibDataService,
            ISyncService syncService)
        {
            _gibDataService = gibDataService;
            _syncService = syncService;
        }

        public async Task GibUsersSync()
        {
            var stream = await _gibDataService.GetNewUserGbList();
            await _syncService.SyncDataAsync(stream);
        }
    }
}
