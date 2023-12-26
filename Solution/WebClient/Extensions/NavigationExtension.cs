using Models.Entities;
using WebClient.ViewModels.Others;
using static Models.Entities.EnumEntities;

namespace WebClient.Extensions
{
    public static class NavigationExtension
    {
        public static NavigationModel GetNavigation(List<GrantAccess> accesses, MasterRole role)
        {
            NavigationModel navigation = new();

            navigation.MasterNavigation = new MasterNavigationModel()
            {
                IsViewEmployee = accesses.FirstOrDefault(i => i.Module == EnumModule.Employee)?.IsView ?? false,
                IsViewWorkDay = accesses.FirstOrDefault(i => i.Module == EnumModule.WorkDay)?.IsView ?? false,
                IsViewRole = accesses.FirstOrDefault(i => i.Module == EnumModule.Role)?.IsView ?? false,
            };


            navigation.ConfigurationNavigation = new ConfigurationNavigationModel()
            {
                IsViewGrandAccess = accesses.FirstOrDefault(i => i.Module == EnumModule.GrandAccess)?.IsView ?? false,
            };

            return navigation;
        }
    }
}
