using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Klyuchnikov.Seti.OneSemestr.Lab4
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Request.Url.AbsolutePath.Contains("Account") &&!this.Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Account/Login.aspx");
            }
        }
    }
}
