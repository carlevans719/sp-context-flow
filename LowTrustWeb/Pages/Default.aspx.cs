using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LowTrustWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointAcsContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get Sharepoint site URL from query
            Uri spUri = new Uri(Request.QueryString["SPHostUrl"]);

            // pull the context token off the request body
            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);
            if (contextTokenString != null)
            {
                // validate + parse context token
                SharePointContextToken contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                // User+Add-In token
                string accessToken = TokenHelper.GetAccessToken(contextToken, spUri.Authority).AccessToken;

                // Add-In only token (not used)
                string addinOnlyAccessToken = TokenHelper.GetAppOnlyAccessToken(contextToken.TargetPrincipalName, spUri.Authority, contextToken.Realm).AccessToken;

                // CSOM context
                ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spUri.ToString(), accessToken);

                // Test loading roles from api
                string permissionLevelName = "Carl";
                Web web = clientContext.Web;
                clientContext.Load(web, items => items.RoleDefinitions);
                clientContext.ExecuteQuery();
                RoleDefinition role = web.RoleDefinitions.Where(item => item.Name == permissionLevelName).FirstOrDefault();

                bool failed = false;
                try
                {
                    if (null != role)
                    {
                        clientContext.Load(role);
                        clientContext.ExecuteQuery();
                        // Should fail if USER doesn't have the correct permissions
                        role.DeleteObject();
                        clientContext.ExecuteQuery();
                        Console.WriteLine("Permission Level [{0}] Removed", permissionLevelName);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Response.Write(string.Format('Failed - {0}', exception.Message));
                    failed = true;
                }

                if (failed != true) {
                    Response.Write('Success');
                }

                return;
            }

            Response.Write("Error");
        }
    }
}
