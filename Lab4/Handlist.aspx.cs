using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lab4
{
    public partial class Handlist : System.Web.UI.Page
    {
        char[] chars = "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮЁ".ToArray().OrderBy(a => a).ToArray();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            startDDL.DataSource = chars;
            startDDL.DataBind();
            endDDL.DataSource = chars;
            endDDL.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var db = new DCDataContext();
            var arr = chars.Where(q => q >= startDDL.SelectedValue.ToCharArray()[0] && q <= endDDL.SelectedValue.ToCharArray()[0]).ToArray();
            var res = db.Employee.Where(q => arr.Contains(q.FirstName.ToUpper()[0])).OrderBy(a => a.FirstName).ToArray();
            GridView1.DataSource = res;
            GridView1.DataBind();
        }

    }
}