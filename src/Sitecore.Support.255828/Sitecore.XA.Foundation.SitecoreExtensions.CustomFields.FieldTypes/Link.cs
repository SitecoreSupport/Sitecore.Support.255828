namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.XA.Foundation.SitecoreExtensions.Services;
    using System;

    public class Link : Sitecore.Shell.Applications.ContentEditor.Link
    {
        private readonly IQueryService _queryService;

        public string ItemID { get; set; }

        public new string Source
        {
            get { return base.Source; }
            set
            {
                if (value == null)
                {
                    base.Source = null;
                }
                else
                {
                    base.Source = (value.StartsWith("query:", StringComparison.OrdinalIgnoreCase)
                        ? _queryService.Resolve(value, ItemID)
                        : value);
                }
            }
        }

        public Link()
        {
            _queryService = ServiceLocator.ServiceProvider.GetService<IQueryService>();
        }
    }
}