namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Text;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.HtmlControls.Data;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.XA.Foundation.Abstractions;
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    [UsedImplicitly]
    public class LookupNameLookupValue : Input
    {
        public string FieldName
        {
            get { return GetViewStateString("FieldName"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("FieldName", value);
            }
        }

        public string ItemId
        {
            get { return GetViewStateString("ItemID"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("ItemID", value);
            }
        }

        public string Source
        {
            get { return GetViewStateString("Source"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("Source", value);
            }
        }

        protected IContext SitecoreContext { get; } = ServiceLocator.ServiceProvider.GetService<IContext>();


        public LookupNameLookupValue()
        {
            base.Activation = true;
            Class = "scContentControl";
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
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
                        key.StartsWith(ID + "_Param", StringComparison.OrdinalIgnoreCase) &&
                        !key.EndsWith("_value", StringComparison.OrdinalIgnoreCase))
                    {
                        string text2 = nameValueCollection[key];
                        string text3 = nameValueCollection[key + "_value"];
                        if (!string.IsNullOrEmpty(text2))
                        {
                            urlString[text2] = (text3 ?? string.Empty);
                        }
                    }
                }

                string text4 = HttpUtility.UrlEncode(urlString.ToString());
                if (Value != text4)
                {
                    Value = text4;
                    SetModified();
                }
            }
        }

        protected override void SetModified()
        {
            base.SetModified();
            if (base.TrackModified)
            {
                SitecoreContext.ClientPage.Modified = true;
            }
        }

        protected virtual void BuildControl()
        {
            UrlString urlString = new UrlString(HttpUtility.UrlDecode(Value));
            foreach (string key in urlString.Parameters.Keys)
            {
                if (key.Length > 0)
                {
                    Controls.Add(new LiteralControl(BuildParameterKeyValue(key, urlString.Parameters[key])));
                }
            }

            Controls.Add(new LiteralControl(BuildParameterKeyValue(string.Empty, string.Empty)));
        }

        protected virtual string BuildParameterKeyValue(string key, string value)
        {
            Assert.ArgumentNotNull(key, "key");
            Assert.ArgumentNotNull(value, "value");
            string uniqueID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID(ID + "_Param");
            SitecoreContext.ClientPage.ServerProperties[ID + "_LastParameterID"] = uniqueID;
            string clientEvent = SitecoreContext.ClientPage.GetClientEvent(ID + ".ParameterChange");
            string readOnly = ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string disabled = Disabled ? " disabled=\"disabled\"" : string.Empty;
            string keyHtmlControl = GetKeyHtmlControl(uniqueID, StringUtil.EscapeQuote(HttpUtility.UrlDecode(key)),
                readOnly, disabled, clientEvent);
            string valueHtmlControl =
                GetValueHtmlControl(uniqueID, StringUtil.EscapeQuote(HttpUtility.UrlDecode(value)));
            return
                $"<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\"><tr><td width=\"50%\">{keyHtmlControl}</td><td width=\"50%\">{valueHtmlControl}</td></tr></table>";
        }

        protected virtual string GetKeyHtmlControl(string id, string key, string readOnly, string disabled,
            string clientEvent)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter());
            Item item = SitecoreContext.ContentDatabase.GetItem(ItemId);
            Item[] items = LookupSources.GetItems(item, SourcePart(0, item));
            htmlTextWriter.Write("<select id=\"" + id + "\" name=\"" + id + "\"" + readOnly + disabled +
                                 " style=\"width:100%\" onchange=\"" + clientEvent + "\"" + GetControlAttributes() +
                                 ">");
            htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(key) ? " selected=\"selected\"" : string.Empty) +
                                 " value=\"\"></option>");
            Item[] array = items;
            foreach (Item item2 in array)
            {
                string itemHeader = GetItemHeader(item2);
                bool flag = item2.ID.ToString() == key;
                htmlTextWriter.Write("<option value=\"" + item2.ID + "\"" +
                                     (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
            }

            htmlTextWriter.Write("</select>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        protected virtual string GetValueHtmlControl(string id, string value)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter());
            Item item = SitecoreContext.ContentDatabase.GetItem(ItemId);
            Item[] items = LookupSources.GetItems(item, SourcePart(1, item));
            htmlTextWriter.Write("<select id=\"" + id + "_value\" name=\"" + id + "_value\" style=\"width:100%\"" +
                                 GetControlAttributes() + ">");
            htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(value) ? " selected=\"selected\"" : string.Empty) +
                                 " value=\"\"></option>");
            Item[] array = items;
            foreach (Item item2 in array)
            {
                string itemHeader = GetItemHeader(item2);
                bool flag = item2.ID.ToString() == value;
                htmlTextWriter.Write("<option value=\"" + item2.ID + "\"" +
                                     (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
            }

            htmlTextWriter.Write("</select>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        protected virtual string GetItemHeader(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            string @string = StringUtil.GetString(FieldName);
            if (@string.StartsWith("@", StringComparison.Ordinal))
            {
                return item[@string.Substring(1)];
            }

            if (@string.Length > 0)
            {
                return item[FieldName];
            }

            return item.DisplayName;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            SetWidthAndHeightStyle();
            output.Write("<div" + base.ControlAttributes + ">");
            RenderChildren(output);
            output.Write("</div>");
        }

        [UsedImplicitly]
        protected virtual void ParameterChange()
        {
            ClientPage clientPage = SitecoreContext.ClientPage;
            if (clientPage.ClientRequest.Source ==
                StringUtil.GetString(clientPage.ServerProperties[ID + "_LastParameterID"]) &&
                !string.IsNullOrEmpty(clientPage.ClientRequest.Form[clientPage.ClientRequest.Source]))
            {
                string value = BuildParameterKeyValue(string.Empty, string.Empty);
                clientPage.ClientResponse.Insert(ID, "beforeEnd", value);
            }

            NameValueCollection nameValueCollection = null;
            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
            {
                nameValueCollection = page.Request.Form;
            }

            if (nameValueCollection != null)
            {
                clientPage.ClientResponse.SetReturnValue(value: true);
            }
        }

        protected virtual string SourcePart(int index, Item item)
        {
            string[] array = Source.Split(new string[1]
            {
                "||"
            }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length <= index)
            {
                return string.Empty;
            }

            return array[index].Trim();
        }
    }
}