namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.DependencyInjection;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.XA.Foundation.SitecoreExtensions.Controls;
    using Sitecore.XA.Foundation.SitecoreExtensions.Services;
    using System;
    using System.Text;
    using System.Web.UI;

    public class DatasourceMultiRootTreeview : SourcedMultiRootTreeview
    {
        protected virtual string InitialDataContext
        {
            get
            {
                if (base.CurrentDataContext.Equals(string.Empty))
                {
                    DataContext dataContext = GetDataContext();
                    if (dataContext != null)
                    {
                        return dataContext.ID;
                    }

                    return string.Empty;
                }

                return base.CurrentDataContext;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("DatasourceMultiRootTreeview"))
            {
                Page.ClientScript.RegisterClientScriptInclude("DatasourceMultiRootTreeview",
                    "/sitecore/shell/Controls/DatasourceMultiRootTreeview/DatasourceMultiRootTreeview.js");
            }
        }

        protected override void RenderNodeBegin(HtmlTextWriter output, IDataView dataView, string filter, Item item,
            bool active, bool isExpanded)
        {
            base.RenderNodeBegin(output, dataView, filter, item, active, isExpanded);
            if (active && InitialDataContext.Equals(base.CurrentDataContext))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<span class=\"treeNodeCommnds\">");
                IRenderingDatasourceCommandService service = ServiceLocator.ServiceProvider.GetService<IRenderingDatasourceCommandService>();
                if (service != null && Sitecore.Context.PageMode.IsExperienceEditorEditing)
                {
                    foreach (string command in service.GetCommands(item))
                    {
                        stringBuilder.Append(command);
                    }

                    stringBuilder.Append("</span>");
                    output.Write(stringBuilder.ToString());
                }
            }
        }
    }
}