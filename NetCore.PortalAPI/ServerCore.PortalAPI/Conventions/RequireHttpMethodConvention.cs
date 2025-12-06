using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace ServerCore.PortalAPI.Conventions
{
    public class RequireHttpMethodConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            // If the action doesn't have any HTTP method attribute, hide it from API Explorer
            var hasHttpMethodAttribute = action.Attributes
                .Any(attr => attr.GetType().Name.StartsWith("Http") && 
                            attr.GetType().Name.EndsWith("Attribute"));
            
            if (!hasHttpMethodAttribute)
            {
                action.ApiExplorer.IsVisible = false;
            }
        }
    }
}
