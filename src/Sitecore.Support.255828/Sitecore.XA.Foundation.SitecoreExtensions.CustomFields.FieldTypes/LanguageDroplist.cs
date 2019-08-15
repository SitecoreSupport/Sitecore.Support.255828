namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Data.Managers;
    using Sitecore.DependencyInjection;
    using Sitecore.Globalization;
    using Sitecore.XA.Foundation.Abstractions;
    using Sitecore.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes;
    using System.Collections.Generic;
    using System.Linq;

    public class LanguageDroplist : CustomDroplist
    {
        protected override IDictionary<string, string> GetItems()
        {
            if (ServiceLocator.ServiceProvider.GetService<IContext>().ContentDatabase != null)
            {
                return (from l in LanguageManager.GetLanguages(ServiceLocator.ServiceProvider.GetService<IContext>()
                        .ContentDatabase)
                    select l.Name).ToDictionary((string s) => s);
            }

            return new Dictionary<string, string>();
        }
    }
}