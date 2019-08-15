namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Controls
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Resources.Media;
    using Sitecore.Shell.Applications.Dialogs.GeneralLink;
    using System;

    public class GeneralLinkForm : Sitecore.Shell.Applications.Dialogs.GeneralLink.GeneralLinkForm
    {
        private string CurrentMode
        {
            get
            {
                string text = ServerProperties["current_mode"] as string;
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }

                return "internal";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            HandleInitialMediaItemSelected();
        }

        protected virtual void HandleInitialMediaItemSelected()
        {
            if (!(CurrentMode == "media") && !string.IsNullOrEmpty(base.LinkAttributes["url"]))
            {
                return;
            }

            string text = base.LinkAttributes["id"];
            if (!string.IsNullOrEmpty(text) && ID.IsID(text))
            {
                MediaLinkDataContext.SetFolder(new ItemUri(new ID(text), Client.ContentDatabase));
                Item item = MediaLinkTreeview.GetDataView().GetItem(text);
                if (item != null)
                {
                    UpdateMediaPreview(item);
                }
            }
        }

        protected virtual void UpdateMediaPreview(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            MediaUrlOptions thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(item);
            thumbnailOptions.UseDefaultIcon = true;
            thumbnailOptions.Width = 96;
            thumbnailOptions.Height = 96;
            thumbnailOptions.Language = item.Language;
            thumbnailOptions.AllowStretch = false;
            MediaPreview.InnerHtml = "<img src=\"" + MediaManager.GetMediaUrl(item, thumbnailOptions) +
                                     "\" width=\"96px\" height=\"96px\" border=\"0\" alt=\"\" />";
        }
    }
}