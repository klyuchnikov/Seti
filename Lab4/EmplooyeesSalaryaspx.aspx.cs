using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lab4
{
    public partial class EmplooyeesSalaryaspx : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public class EmpRes
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronymic { get; set; }
            public int OfficialSalary { get; set; }
            public string ProjectName { get; set; }
            public EmpRes()
            {
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            var query =
                @"SELECT     Employee.FirstName, Employee.LastName, Employee.Patronymic, EmployeeProject.OfficialSalary, Project.Name
FROM         EmployeeProject INNER JOIN
                      Employee ON EmployeeProject.EmployeeId = Employee.Id INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
WHERE     (EmployeeProject.OfficialSalary > {0})";
            var db = new DCDataContext();
            db.ExecuteQuery<EmpRes>(query, TextBox1.Text);
            GridView1.DataBind();
        }
    }
}