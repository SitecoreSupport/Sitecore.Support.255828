namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Web;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using Sitecore.XA.Foundation.Abstractions;
    using System;

    public class YesNoForm : DialogForm
    {
        protected Literal Text;

        protected IContext Context { get; } = ServiceLocator.ServiceProvider.GetService<IContext>();


        protected void No()
        {
            Context.ClientPage.ClientResponse.SetDialogValue("no");
            Context.ClientPage.ClientResponse.CloseWindow();
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                string text = WebUtil.SafeEncode(WebUtil.GetQueryString("te"));
                text = text.Replace("&lt;br/&gt;", "<br/>");
                if (WebUtil.GetQueryString("pre") == "yes")
                {
                    text = "<pre>" + text + "</pre>";
                }

                Text.Text = text;
            }
        }

        protected void Yes()
        {
            Context.ClientPage.ClientResponse.SetDialogValue("yes");
            Context.ClientPage.ClientResponse.CloseWindow();
        }
    }
}