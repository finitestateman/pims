namespace Pims.Core.Services
{
    public interface IAppInfoService
    {
        string GetAppDisplayName();
    }

    public sealed class AppInfoService : IAppInfoService
    {
        public string GetAppDisplayName() => "Pims";
    }
}
