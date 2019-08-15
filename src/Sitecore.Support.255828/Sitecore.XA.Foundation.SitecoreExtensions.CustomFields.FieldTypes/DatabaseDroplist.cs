namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Sitecore.Configuration;
    using Sitecore.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes;
    using System.Collections.Generic;
    using System.Linq;

    public class DatabaseDroplist : CustomDroplist
    {
        protected override IDictionary<string, string> GetItems()
        {
            return Factory.GetDatabaseNames().ToDictionary((string s) => s);
        }
    }
}