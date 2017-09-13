namespace Sitecore.Support.Shell.Applications.ContentEditor
{
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections.Specialized;
    using System.Net;

    public class Link : Sitecore.Shell.Applications.ContentEditor.Link
    {
        public XmlValue XmlValue
        {
            get
            {
                return new XmlValue(this.GetViewStateString("XmlValue"), "link");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.SetViewStateString("XmlValue", value.ToString());
            }
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull((object)message, "message");
            if (!(message["id"] == this.ID))
                return;
            if (message.Name == "contentlink:internallink")
                this.Insert("/sitecore/shell/Applications/Dialogs/Internal link.aspx");
            else
                base.HandleMessage(message);
        }

        protected new void Insert(string url)
        {
            Assert.ArgumentNotNull((object)url, "url");
            this.Insert(url, new NameValueCollection());
        }

        protected new void Insert(string url, NameValueCollection additionalParameters)
        {
            Assert.ArgumentNotNull((object)url, "url");
            Assert.ArgumentNotNull((object)additionalParameters, "additionalParameters");
            Sitecore.Context.ClientPage.Start((object)this, "InsertLink", new NameValueCollection()
      {
        {
          "url",
          url
        },
        additionalParameters
      });
        }

        protected new void InsertLink(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            if (args.IsPostBack)
            {
                if (string.IsNullOrEmpty(args.Result) || !(args.Result != "undefined"))
                    return;
                NameValueCollection attributes = HtmlUtil.ParseTagAttributes(args.Result);
                XmlValue encodedResultAttributes = new XmlValue(args.Result, "link");
                foreach (var item in attributes)
                {
                    string name = item.ToString();
                    string redecodeValue = WebUtility.HtmlDecode(Uri.UnescapeDataString(attributes[name]));
                    encodedResultAttributes.SetAttribute(name, redecodeValue);
                }
                base.SetValue(encodedResultAttributes.ToString());
                this.SetModified();
                Sitecore.Context.ClientPage.ClientResponse.SetAttribute(this.ID, "value", this.Value);
                SheerResponse.Eval("scContent.startValidators()");
            }
            else
            {
                UrlString urlString1 = new UrlString(args.Parameters["url"]);
                UrlHandle urlHandle = new UrlHandle();
                string parameter1 = args.Parameters["width"];
                string parameter2 = args.Parameters["height"];
                string index = "va";
                string str = this.XmlValue.ToString();
                urlHandle[index] = str;
                UrlString urlString2 = urlString1;
                urlHandle.Add(urlString2);
                urlString1.Append("ro", this.Source);
                urlString1.Add("la", this.ItemLanguage);
                urlString1.Append("sc_content", WebUtil.GetQueryString("sc_content"));
                Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(urlString1.ToString(), parameter1, parameter2, string.Empty, true);
                args.WaitForPostBack();
            }
        }
    }
}