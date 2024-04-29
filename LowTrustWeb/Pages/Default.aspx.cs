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
            Uri spUri = new Uri(Request.QueryString["SPHostUrl"]);
            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);
            if (contextTokenString != null)
            {
                SharePointContextToken contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                string accessToken = TokenHelper.GetAccessToken(contextToken, spUri.Authority).AccessToken;
                string addinOnlyAccessToken = TokenHelper.GetAppOnlyAccessToken(contextToken.TargetPrincipalName, spUri.Authority, contextToken.Realm).AccessToken;
                ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spUri.ToString(), accessToken);
                //clientContext.Load(clientContext.Site, site => site.Re);
                //clientContext.ExecuteQuery();

                string permissionLevelName = "Carl";
                Web web = clientContext.Web;
                clientContext.Load(web, items => items.RoleDefinitions);
                clientContext.ExecuteQuery();
                RoleDefinition role = web.RoleDefinitions.Where(item => item.Name == permissionLevelName).FirstOrDefault();
                try
                {
                    if (null != role)
                    {
                        clientContext.Load(role);
                        clientContext.ExecuteQuery();
                        role.DeleteObject();
                        clientContext.ExecuteQuery();
                        Console.WriteLine("Permission Level [{0}] Removed", permissionLevelName);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }


                Response.Write(accessToken);
                Response.Write(addinOnlyAccessToken);
                return;
            }
            Response.Write("Error");
            //Uri spUri = new Uri("https://sharepoint.cevanapps.com");
            //string code = Request.QueryString["code"];
            //var token = TokenHelper.GetAccessToken(code, TokenHelper.SharePointPrincipal, spUri.Authority, TokenHelper.GetRealmFromTargetUrl(spUri), new Uri("https://myapp.app-host.cevanapps.com/Pages/Default.aspx")).AccessToken;
            //Response.Write(token);
            //ClientContext clientContext = TokenHelper.GetClientContextWithAuthorizationCode(spUri.ToString(), code, new Uri("https://myapp.app-host.cevanapps.com/Pages/Default.aspx"));

            //clientContext.Load(clientContext.Web, web => web.Title);

            //clientContext.ExecuteQuery();
            //Response.Write(clientContext.Web.Title);
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using LowTrustWeb;

////using Microsoft.SharePoint.Samples;
//using Microsoft.SharePoint.Client;

//namespace LowTrustWeb
//{
//    public partial class Default : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            Uri sharePointSiteUrl = new Uri("https://sharepoint.cevanapps.com/");

//            if (Request.QueryString["code"] != null)
//            {
//                TokenCache.UpdateCacheWithCode(Request, Response, sharePointSiteUrl);
//            }

//            if (!TokenCache.IsTokenInCache(Request.Cookies))
//            {
//                Response.Redirect(TokenHelper.GetAuthorizationUrl(sharePointSiteUrl.ToString(), "Web.Write"));
//            }
//            else
//            {
//                string refreshToken = TokenCache.GetCachedRefreshToken(Request.Cookies);
//                string accessToken =
//                TokenHelper.GetAccessToken(
//                            refreshToken,
//                            "00000003-0000-0ff1-ce00-000000000000",
//                            sharePointSiteUrl.Authority,
//                            TokenHelper.GetRealmFromTargetUrl(sharePointSiteUrl)).AccessToken;

//                using (ClientContext context =
//                        TokenHelper.GetClientContextWithAccessToken(sharePointSiteUrl.ToString(),
//                                                                    accessToken))
//                {
//                    context.Load(context.Web);
//                    context.ExecuteQuery();
//                    TokenHelper.GetAppOnlyAccessToken();
//                    Response.Write("<p>" + context.Web.Title + "</p>");
//                }
//            }
//        }
//    }
//}