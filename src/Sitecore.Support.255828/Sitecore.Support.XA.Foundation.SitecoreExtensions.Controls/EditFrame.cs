namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.XA.Foundation.Abstractions;

    public class EditFrame : Sitecore.Web.UI.WebControls.EditFrame
    {
        public bool IsControlEditable { get; set; }

        public EditFrame()
        {
            IPageMode service = ServiceLocator.ServiceProvider.GetService<IPageMode>();
            IsControlEditable = service.IsExperienceEditorEditing;
        }

        public EditFrame(bool isControlEditable)
        {
            IsControlEditable = isControlEditable;
        }

        protected override bool ShouldRender()
        {
            if (Enabled)
            {
                return IsControlEditable;
            }

            return false;
        }
    }
}