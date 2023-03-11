using EBookShopR.Models.ViewModels;

namespace EBookShopR.Areas.Admin.Services
{
    public interface IMvcActionsDiscoveryService
    {
        ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
    }
}
