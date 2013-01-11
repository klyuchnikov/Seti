using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lab4
{
    public partial class LoadAvatar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if(!FileUpload1.HasFile)
                return;
            var db = new DCDataContext();
            db.Employee.Single(a => a.Id == int.Parse(DropDownList1.SelectedValue)).AvatarName = DropDownList1.SelectedValue + "_" + FileUpload1.FileName;
            db.SubmitChanges();
            FileUpload1.SaveAs(Directory.GetParent(HttpContext.Current.Request.PhysicalPath).FullName +  "/Avatars/" + DropDownList1.SelectedValue + "_" + FileUpload1.FileName);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "key", "alert('Файл загружен!');", true);
        }
    }
}