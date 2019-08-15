namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Sitecore.Data.Items;
    using Sitecore.Pipelines;
    using Sitecore.XA.Foundation.SitecoreExtensions.Controls;
    using Sitecore.XA.Foundation.SitecoreExtensions.Pipelines.GetItemSourceInfo;

    public class SourcedMultiRootTreeview : ExtendedMultiRootTreeview
    {
        protected override string GetHeaderValue(Item item)
        {
            string text = base.GetHeaderValue(item);
            if (IsRoot(item))
            {
                ItemSourceInfoArgs itemSourceInfoArgs = new ItemSourceInfoArgs(item);
                CorePipeline.Run("getItemSourceInfo", itemSourceInfoArgs, failIfNotExists: false);
                string sourceInfo = itemSourceInfoArgs.SourceInfo;
                if (!string.IsNullOrEmpty(sourceInfo))
                {
                    text = text + " (" + sourceInfo + ")";
                }
            }

            return text;
        }
    }
}