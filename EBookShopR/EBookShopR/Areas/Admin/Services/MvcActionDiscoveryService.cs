using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EBookShopR.Areas.Admin.Services
{
    public class MvcActionDiscoveryService:IMvcActionsDiscoveryService
    {
        private readonly ConcurrentDictionary<string, Lazy<ICollection<ControllerViewModel>>> _allSecuredActionsWithPloicy = new ConcurrentDictionary<string, Lazy<ICollection<ControllerViewModel>>>();
        public ICollection<ControllerViewModel> MvcControllers { get; }
        public MvcActionDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            if (actionDescriptorCollectionProvider == null)
            {
                throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));
            }
            MvcControllers=new List<ControllerViewModel>();
            var LastControllerName = string.Empty;
            ControllerViewModel currentController = null;
            var actionDescriptors= actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                if(!(actionDescriptor is ControllerActionDescriptor descriptor))
                {
                    continue;
                }
                var controllerTypeInfo = descriptor.ControllerTypeInfo;
                var actionMethodInfo = descriptor.MethodInfo;
                if (LastControllerName != descriptor.ControllerName)
                {
                    currentController = new ControllerViewModel()
                    {
                        AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                        ControllerAttributes = GetAttributes(controllerTypeInfo),
                        ControllerDisplayName = controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                        ControllerName = descriptor.ControllerName,

                    };

                    MvcControllers.Add(currentController);
                    LastControllerName = descriptor.ControllerName;
                }
                currentController?.MvcAction.Add(new ActionViewModel()
                {
                    ControllerId = currentController.ControllerId,
                    ActionName = descriptor.ActionName,
                    ActionDisplayName = actionMethodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                    ActionAttributes = GetAttributes(actionMethodInfo),
                    IsSecuredAction = IsSecurdAction(controllerTypeInfo, actionMethodInfo),
                }) ;
                
            }
        }
        public ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName)
        {
            var getter = _allSecuredActionsWithPloicy.GetOrAdd(policyName, y => new Lazy<ICollection<ControllerViewModel>>(
                () =>
                {
                    var controllers = new List<ControllerViewModel>(MvcControllers);
                    foreach (var controller in controllers)
                    {
                        controller.MvcAction = controller.MvcAction.Where(
                            model => model.IsSecuredAction &&
                            (
                            model.ActionAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy == policyName ||
                            controller.ControllerAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy == policyName
                            )).ToList();
                    }
                    return controllers.Where(model => model.MvcAction.Any()).ToList();
                }));
            return getter.Value;
        }
        private static List<Attribute> GetAttributes(MemberInfo actionMethodInfo)
        {
            return actionMethodInfo.GetCustomAttributes(inherit: true)
                                   .Where(attribute =>
                                   {
                                       var attributeNamespace = attribute.GetType().Namespace;
                                       return attributeNamespace != typeof(CompilerGeneratedAttribute).Namespace &&
                                              attributeNamespace != typeof(DebuggerStepThroughAttribute).Namespace;
                                   })
                                    .Cast<Attribute>()
                                   .ToList();
        }

        private static bool IsSecurdAction(MemberInfo controllerInfo,MemberInfo actionMethodInfo)
        {
            var actionHasAllowAnonymousAttribute = actionMethodInfo.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true) != null;
            if (actionHasAllowAnonymousAttribute)
            {
                return false;
            }
            var controllerHasAuthorizeAttribute = controllerInfo.GetCustomAttributes<AuthorizeAttribute>(inherit: true) != null;
            if(controllerHasAuthorizeAttribute)
            {
                return true;
            }
            var actionMethodHasAuthorizeAttribute = actionMethodInfo.GetCustomAttributes<AuthorizeAttribute>(inherit: true) != null;
            if(actionHasAllowAnonymousAttribute)
            {
                return true;
            }
            return false;
        }
    }
}
