using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Klyuchnikov.Seti.OneSemestr.Lab4
{
    public partial class MinSalary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var query = @"SELECT     Project.Name, EmployeeProject.OfficialSalary
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
ORDER BY EmployeeProject.OfficialSalary";
            var db = new DCDataContext();
            db.ExecuteQuery<string>(query);
            GridView1.DataBind();
        }
    }
}