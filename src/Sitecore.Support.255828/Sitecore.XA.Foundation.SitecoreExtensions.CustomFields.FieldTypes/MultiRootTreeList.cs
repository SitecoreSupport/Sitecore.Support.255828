namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore;
    using Sitecore.Buckets.Extensions;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.XA.Foundation.Abstractions;
    using Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls;
    using Sitecore.XA.Foundation.SitecoreExtensions.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

    public class MultiRootTreeList : TreeList
    {
        private const string QueryPrefix = "query:";

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
                else if (value.Contains("|"))
                {
                    IEnumerable<string> values = from s in value.Split('|').Select(delegate(string source)
                        {
                            string text2 = source.Trim();
                            if (text2.StartsWith("query:", StringComparison.OrdinalIgnoreCase))
                            {
                                text2 = _queryService.Resolve(text2, base.ItemID);
                            }

                            return text2;
                        })
                        where !s.IsNullOrEmpty()
                        select s;
                    base.Source = string.Join("|", values);
                }
                else
                {
                    base.Source = value;
                }
            }
        }

        public MultiRootTreeList()
        {
            _queryService = ServiceLocator.ServiceProvider.GetService<IQueryService>();
        }

        protected override void OnLoad(EventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            base.OnLoad(args);
            if (!ServiceLocator.ServiceProvider.GetService<IContext>().ClientPage.IsEvent)
            {
                TreeviewEx treeviewEx = (TreeviewEx) WebUtil.FindControlOfType(this, typeof(TreeviewEx));
                System.Web.UI.Control parent = treeviewEx.Parent;
                treeviewEx.Parent.Controls.Clear();
                DataContext dataContext = (DataContext) WebUtil.FindControlOfType(this, typeof(DataContext));
                System.Web.UI.Control parent2 = dataContext.Parent;
                parent2.Controls.Remove(dataContext);
                DatasourceMultiRootTreeview datasourceMultiRootTreeview = new DatasourceMultiRootTreeview();
                datasourceMultiRootTreeview.ID = treeviewEx.ID;
                datasourceMultiRootTreeview.DblClick = treeviewEx.DblClick;
                datasourceMultiRootTreeview.Enabled = treeviewEx.Enabled;
                datasourceMultiRootTreeview.DisplayFieldName = treeviewEx.DisplayFieldName;
                DataContext[] array = ParseDataContexts(dataContext);
                datasourceMultiRootTreeview.DataContext = string.Join("|", from x in array
                    select x.ID);
                DataContext[] array2 = array;
                foreach (DataContext child in array2)
                {
                    parent2.Controls.Add(child);
                }

                parent.Controls.Add(datasourceMultiRootTreeview);
            }
        }

        protected virtual DataContext[] ParseDataContexts(DataContext originalDataContext)
        {
            return (from x in new ListString(DataSource)
                select CreateDataContext(originalDataContext, x)).ToArray();
        }

        protected virtual DataContext CreateDataContext(DataContext baseDataContext, string dataSource)
        {
            DataContext dataContext = new DataContext();
            dataContext.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("D");
            dataContext.Filter = baseDataContext.Filter;
            dataContext.DataViewName = "Master";
            if (!string.IsNullOrEmpty(base.DatabaseName))
            {
                dataContext.Parameters = "databasename=" + base.DatabaseName;
            }

            dataContext.Root = dataSource;
            dataContext.Language = Language.Parse(base.ItemLanguage);
            return dataContext;
        }
    }
}