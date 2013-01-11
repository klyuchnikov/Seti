<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Salary.aspx.cs" Inherits="Lab4.Salary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
         BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
        CellPadding="2" ForeColor="Black" GridLines="None" 
        DataSourceID="SqlDataSource1">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="FirstName" HeaderText="Фамилия" 
                SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="Имя" 
                SortExpression="LastName" />
            <asp:BoundField DataField="Patronymic" HeaderText="Отчество" 
                SortExpression="Patronymic" />
            <asp:BoundField DataField="Result" HeaderText="Сумма к выплате" DataFormatString="{0:C2}"
                SortExpression="Result" />
            <asp:BoundField DataField="Name" HeaderText="Проект" 
                SortExpression="ProjectName" />
        </Columns>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <SortedAscendingCellStyle BackColor="#FAFAE7" />
        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
        <SortedDescendingCellStyle BackColor="#E1DB9C" />
        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
        SelectCommand="SELECT Employee.FirstName, Employee.LastName, Employee.Patronymic, Employee.Id, Project.Name, CAST(EmployeeProject.OfficialSalary * EmployeeProject.CountDaysWork AS float(2)) / 22 AS Result FROM Employee INNER JOIN EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN Project ON EmployeeProject.ProjectId = Project.Id">
    </asp:SqlDataSource>
</asp:Content>
