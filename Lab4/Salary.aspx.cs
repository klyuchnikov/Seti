using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Klyuchnikov.Seti.OneSemestr.Lab4
{
    public partial class Salary : System.Web.UI.Page
    {
        public class EmpRes
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronymic { get; set; }
            public int Result { get; set; }
            public string ProjectName { get; set; }
            public EmpRes()
            {
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var query = @"SELECT     Employee.FirstName, Employee.LastName, Employee.Patronymic, Employee.Id, Project.Name, EmployeeProject.OfficialSalary * EmployeeProject.CountDaysWork /22 as Result
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id";
            var db = new DCDataContext();
            db.ExecuteQuery<EmpRes>(query);
            GridView1.DataBind();
        }
    }
}