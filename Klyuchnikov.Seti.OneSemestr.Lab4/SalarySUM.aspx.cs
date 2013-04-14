using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Klyuchnikov.Seti.OneSemestr.Lab4
{
    public partial class SalarySUM : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var query =
                @"SELECT   Employee.FirstName, Employee.LastName, Employee.Patronymic, SUM(EmployeeProject.OfficialSalary)AS SumTotal 
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId
group BY Employee.FirstName, Employee.LastName, Employee.Patronymic";
            var db = new DCDataContext();
            db.ExecuteQuery<string>(query);
            GridView1.DataBind();
        }
    }
}