namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore;
    using Sitecore.DependencyInjection;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.XA.Foundation.SitecoreExtensions.Services;
    using System;

    public class TreeList : Sitecore.Shell.Applications.ContentEditor.TreeList
    {
        private readonly IQueryService _queryService;

        public new string Source
        {
            get { return base.Source; }
            set
            {
                if (value == null)
                {
                    base.Source = null;
                    return;
                }

                string text = StringUtil.ExtractParameter("DataSource", value).Trim();
                if (text.StartsWith("query:", StringComparison.OrdinalIgnoreCase))
                {
                    base.Source = value.Replace(text, _queryService.Resolve(text, base.ItemID));
                }
                else if (value.StartsWith("query:", StringComparison.OrdinalIgnoreCase))
                {
                    base.Source = _queryService.Resolve(value, base.ItemID);
                }
                else
                {
                    base.Source = value;
                }
            }
        }

        public TreeList()
        {
            _queryService = ServiceLocator.ServiceProvider.GetService<IQueryService>();
        }
    }
}