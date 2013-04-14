using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Klyuchnikov.Seti.OneSemestr.Lab4
{
    public partial class SalaryUSD : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var query = @"SELECT     Employee.FirstName, Employee.LastName, Employee.Patronymic, Employee.Id, Project.Name, 
                      CAST(EmployeeProject.OfficialSalary * EmployeeProject.CountDaysWork AS float(2)) / 22 / {0} AS Result, Project.Id AS Expr1
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
WHERE     (Project.Id = {1})";
            var db = new DCDataContext();
            db.ExecuteQuery<Salary.EmpRes>(query, DropDownList1.SelectedValue, TextBox1.Text);
            GridView1.DataBind();
        }
    }
}