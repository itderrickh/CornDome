using CornDome.Repository.Main;

namespace CornDome.Helpers
{
    public class AppFlagHelper(IAppFlagRepository appFlagRepository)
    {
        public bool IsActive(string name)
        {
            return appFlagRepository.IsActive(name);
        }
    }
}
