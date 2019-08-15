namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.WebControls;
    using System.Linq;

    public class ExtendedMultiRootTreeview : MultiRootTreeview
    {
        protected virtual bool IsRoot(Item item)
        {
            return GetDataContexts().FirstOrDefault((DataContext context) => context.GetRoot().ID == item.ID) != null;
        }

        protected override string GetNodeID(string shortID)
        {
            foreach (DataContext dataContext in GetDataContexts())
            {
                Item root = dataContext.GetRoot();
                Item item = root.Database.GetItem(Sitecore.Data.ID.Parse(shortID));
                if (item != null && item.Axes.IsDescendantOf(root))
                {
                    return ID + "_" + ((dataContext != null) ? (dataContext + "_") : string.Empty) + shortID;
                }
            }

            return string.Empty;
        }
    }
}