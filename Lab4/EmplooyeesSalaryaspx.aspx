<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmplooyeesSalaryaspx.aspx.cs" Inherits="Lab4.EmplooyeesSalaryaspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Оклад:
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" Text="Найти" onclick="Button1_Click" />
    </p>
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
            <asp:BoundField DataField="OfficialSalary" HeaderText="Оклад" 
                SortExpression="OfficialSalary" />
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
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" SelectCommand="SELECT     Employee.FirstName, Employee.LastName, Employee.Patronymic, EmployeeProject.OfficialSalary, Project.Name
FROM         EmployeeProject INNER JOIN
                      Employee ON EmployeeProject.EmployeeId = Employee.Id INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
WHERE     (EmployeeProject.OfficialSalary &gt; @p)">
        <SelectParameters>
            <asp:ControlParameter ControlID="TextBox1" Name="p" PropertyName="Text" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    </asp:Content>
