namespace GibUsers.Api
{

    public interface IHangfireJobs
    {
        Task GibPkUsersSync();
        Task GibGbUsersSync();
    }
}
