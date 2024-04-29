using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LowTrustWeb.Pages
{
    public partial class Redirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(TokenHelper.GetAuthorizationUrl("https://sharepoint.cevanapps.com", "", "https://myapp.app-host.cevanapps.com/Pages/Default.aspx"));
        }
    }
}