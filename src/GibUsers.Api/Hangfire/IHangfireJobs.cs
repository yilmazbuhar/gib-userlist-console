namespace GibUsers.Api
{

    public interface IHangfireJobs
    {
        Task GibUsersSync();
    }
}
