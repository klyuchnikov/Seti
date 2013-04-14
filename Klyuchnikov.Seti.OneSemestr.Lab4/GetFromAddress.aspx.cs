using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Klyuchnikov.Seti.OneSemestr.Lab4
{
    public partial class GetFromAddress : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var db = new DCDataContext();
            var res = db.Employee.Where(a => a.Address.Contains(TextBox1.Text));
            GridView1.DataSource = res;
            GridView1.DataBind();
        }
    }
}