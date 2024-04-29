using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LowTrustWeb.Pages
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Uri spUri = new Uri("https://sharepoint.cevanapps.com");
            string code = Request.QueryString["code"];
            ClientContext clientContext = TokenHelper.GetClientContextWithAuthorizationCode(spUri.ToString(), code, new Uri("https://myapp.app-host.cevanapps.com/Pages/RedirectAccept.aspx"));
            
            clientContext.Load(clientContext.Web, web => web.Title);
            clientContext.PendingRequest.ToString();
            clientContext.ExecuteQuery();
            Response.Write(clientContext.Web.Title);
        }
    }
}