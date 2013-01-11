using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lab4
{
    public partial class FirstNameInitials : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DCDataContext();
            var res = db.ExecuteQuery(typeof(string), "SELECT  FirstName + ' ' + SUBSTRING(LastName,1,1) + '. ' + SUBSTRING(Patronymic,1,1) + '.' FROM Employee", new object[0]);
            ListBox1.DataSource = res;
            ListBox1.DataBind();
        }
    }
}