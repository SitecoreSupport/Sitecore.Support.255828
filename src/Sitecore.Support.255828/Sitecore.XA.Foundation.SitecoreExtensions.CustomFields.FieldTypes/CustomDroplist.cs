namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.CustomFields.FieldTypes
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.XA.Foundation.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    public abstract class CustomDroplist : Sitecore.Web.UI.HtmlControls.Control
    {
        private bool _hasPostData;

        protected CustomDroplist()
        {
            Class = "scContentControl";
            base.Activation = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!_hasPostData)
            {
                LoadPostData(string.Empty);
            }
        }

        protected override bool LoadPostData(string value)
        {
            _hasPostData = true;
            if (value == null)
            {
                return false;
            }

            if (GetViewStateString("Value") != value)
            {
                SetModified();
            }

            SetViewStateString("Value", value);
            return true;
        }

        protected virtual void SetModified()
        {
            ServiceLocator.ServiceProvider.GetService<IContext>().ClientPage.Modified = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnPreRender(e);
            base.ServerProperties["Value"] = base.ServerProperties["Value"];
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            output.Write("<select{0}>", GetControlAttributes());
            output.Write("<option value=\"\"></option>");
            bool flag = false;
            foreach (KeyValuePair<string, string> item in GetItems())
            {
                bool flag2 = item.Key == Value;
                flag |= flag2;
                output.Write("<option value=\"{0}\"{1}>{2}</option>", item.Key,
                    flag2 ? " selected=\"selected\"" : string.Empty, item.Value);
            }

            bool num = !string.IsNullOrEmpty(Value) && !flag;
            if (num)
            {
                output.Write("<optgroup label=\"{0}\">",
                    Translate.Text(Translate.Text("Value not in the selection list.")));
                output.Write("<option value=\"{0}\" selected=\"selected\">{1}</option>", Value, Value);
                output.Write("</optgroup>");
            }

            output.Write("</select>");
            if (num)
            {
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>",
                    Translate.Text("The field contains a value that is not in the selection list."));
            }
        }

        protected abstract IDictionary<string, string> GetItems();
    }
}