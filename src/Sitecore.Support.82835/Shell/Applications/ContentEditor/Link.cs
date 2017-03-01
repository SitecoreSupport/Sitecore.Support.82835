using System;
using System.Collections.Specialized;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Support.Shell.Applications.ContentEditor
{
    public class Link : Sitecore.Shell.Applications.ContentEditor.Link
    {
        public XmlValue XmlValue
        {
            get
            {
                return new XmlValue(base.GetViewStateString("XmlValue"), "link");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                base.SetViewStateString("XmlValue", value.ToString());
            }
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (message["id"] == this.ID)
            {
                if (message.Name == "contentlink:internallink")
                {
                    this.Insert("/sitecore/shell/Applications/Dialogs/Internal link.aspx");
                }
                else
                {
                    base.HandleMessage(message);
                }
            }
        }

        protected new void Insert(string url)
        {
            Assert.ArgumentNotNull(url, "url");
            this.Insert(url, new NameValueCollection());
        }

        protected new void Insert(string url, NameValueCollection additionalParameters)
        {
            Assert.ArgumentNotNull(url, "url");
            Assert.ArgumentNotNull(additionalParameters, "additionalParameters");
            NameValueCollection parameters = new NameValueCollection
            {
                {
                    "url",
                    url
                },
                additionalParameters
            };
            Sitecore.Context.ClientPage.Start(this, "InsertLink", parameters);
        }

        protected new void InsertLink(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.IsPostBack)
            {
                if (!string.IsNullOrEmpty(args.Result) && args.Result != "undefined")
                {
                    string value = Uri.UnescapeDataString(args.Result).Replace("&", "&amp;");
                    base.SetValue(value);
                    this.SetModified();
                    Sitecore.Context.ClientPage.ClientResponse.SetAttribute(this.ID, "value", this.Value);
                    SheerResponse.Eval("scContent.startValidators()");
                }
            }
            else
            {
                UrlString urlString = new UrlString(args.Parameters["url"]);
                UrlHandle urlHandle = new UrlHandle();
                string width = args.Parameters["width"];
                string height = args.Parameters["height"];
                urlHandle["va"] = this.XmlValue.ToString();
                urlHandle.Add(urlString);
                urlString.Append("ro", base.Source);
                urlString.Add("la", base.ItemLanguage);
                urlString.Append("sc_content", WebUtil.GetQueryString("sc_content"));
                Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(urlString.ToString(), width, height, string.Empty, true);
                args.WaitForPostBack();
            }
        }

    }
}