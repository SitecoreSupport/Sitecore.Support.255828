namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Sitecore.Shell.Applications.Dialogs;

    public class SelectItemBasicOptions : SelectDatasourceOptions
    {
        protected override string GetXmlControl()
        {
            return "Sitecore.Shell.Applications.Dialogs.SelectItemBasic";
        }
    }
}