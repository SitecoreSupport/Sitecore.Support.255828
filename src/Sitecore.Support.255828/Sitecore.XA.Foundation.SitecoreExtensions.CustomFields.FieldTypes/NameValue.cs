namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Text;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.XA.Foundation.Abstractions;
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.UI;

    [UsedImplicitly]
    public class NameValue : Sitecore.Shell.Applications.ContentEditor.NameValue
    {
        protected override string NameStyle => "width:300px";

        protected IContext SitecoreContext { get; } = ServiceLocator.ServiceProvider.GetService<IContext>();


        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            if (SitecoreContext.ClientPage.IsEvent)
            {
                LoadValue();
            }
            else
            {
                BuildControl();
            }
        }

        protected virtual void LoadValue()
        {
            if (!ReadOnly && !Disabled)
            {
                System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
                NameValueCollection nameValueCollection =
                    (page != null) ? page.Request.Form : new NameValueCollection();
                UrlString urlString = new UrlString();
                foreach (string key in nameValueCollection.Keys)
                {
                    if (!string.IsNullOrEmpty(key) &&
                        key.StartsWith(ID + "_Param", StringComparison.InvariantCulture) &&
                        !key.EndsWith("_value", StringComparison.InvariantCulture))
                    {
                        string text2 = nameValueCollection[key];
                        string text3 = nameValueCollection[key + "_value"];
                        if (!string.IsNullOrEmpty(text2))
                        {
                            urlString[text2] = (text3 ?? string.Empty);
                        }
                    }
                }

                string text4 = urlString.ToString();
                if (Value != text4)
                {
                    Value = text4;
                    SetModified();
                }
            }
        }

        protected virtual void BuildControl()
        {
            UrlString urlString = new UrlString
            {
                Query = Value
            };
            foreach (string key in urlString.Parameters.Keys)
            {
                if (key.Length > 0)
                {
                    Controls.Add(new LiteralControl(BuildParameterKeyValue(key, urlString.Parameters[key])));
                }
            }

            Controls.Add(new LiteralControl(BuildParameterKeyValue(string.Empty, string.Empty)));
        }

        [UsedImplicitly]
        protected new void ParameterChange()
        {
            ClientPage clientPage = SitecoreContext.ClientPage;
            if (clientPage.ClientRequest.Source ==
                StringUtil.GetString(clientPage.ServerProperties[ID + "_LastParameterID"]) &&
                !string.IsNullOrEmpty(clientPage.ClientRequest.Form[clientPage.ClientRequest.Source]))
            {
                string value = BuildParameterKeyValue(string.Empty, string.Empty);
                clientPage.ClientResponse.Insert(ID, "beforeEnd", value);
            }

            clientPage.ClientResponse.SetReturnValue(value: true);
        }

        protected virtual string BuildParameterKeyValue(string key, string value)
        {
            Assert.ArgumentNotNull(key, "key");
            Assert.ArgumentNotNull(value, "value");
            string uniqueID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID(ID + "_Param");
            SitecoreContext.ClientPage.ServerProperties[ID + "_LastParameterID"] = uniqueID;
            string clientEvent = SitecoreContext.ClientPage.GetClientEvent(ID + ".ParameterChange");
            string text = ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string text2 = Disabled ? " disabled=\"disabled\"" : string.Empty;
            string arg = IsVertical ? "</tr><tr>" : string.Empty;
            return string.Format(
                "<table width=\"100%\" class='scAdditionalParameters'><tr><td>{0}</td>{2}<td width=\"100%\">{1}</td></tr></table>",
                string.Format(
                    "<input id=\"{0}\" name=\"{1}\" type=\"text\"{2}{3} style=\"{6}\" value=\"{4}\" onchange=\"{5}\"/>",
                    uniqueID, uniqueID, text, text2, StringUtil.EscapeQuote(key), clientEvent, NameStyle),
                GetValueHtmlControl(uniqueID, StringUtil.EscapeQuote(HttpUtility.UrlDecode(value))), arg);
        }
    }
}