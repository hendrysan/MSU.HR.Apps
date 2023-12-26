using Models.Entities;

namespace WebClient.ViewModels.Others
{
    public class NavigationModel
    {
        public MasterNavigationModel MasterNavigation { get; set; } = new();
        public ConfigurationNavigationModel ConfigurationNavigation { get; set; } = new();
    }

    public class ConfigurationNavigationModel
    {
        public bool IsViewGrandAccess { get; set; } = false;
    }

    public class MasterNavigationModel
    {
    
        public bool IsViewEmployee { get; set; } = false;
        public bool IsViewWorkDay { get; set; } = false;
        public bool IsViewRole { get; set; } = false;
    }


}
