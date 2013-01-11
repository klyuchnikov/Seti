<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MinSalary.aspx.cs" Inherits="Lab4.MinSalary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" SelectCommand="SELECT     Project.Name, EmployeeProject.OfficialSalary
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
ORDER BY EmployeeProject.OfficialSalary"></asp:SqlDataSource>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" 
        CellPadding="2" DataSourceID="SqlDataSource1" ForeColor="Black" 
        GridLines="None">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Название проекта" SortExpression="Name" />
            <asp:BoundField DataField="OfficialSalary" HeaderText="Оклад" 
                SortExpression="OfficialSalary" />
        </Columns>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
            HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <SortedAscendingCellStyle BackColor="#FAFAE7" />
        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
        <SortedDescendingCellStyle BackColor="#E1DB9C" />
        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
    </asp:GridView>
</asp:Content>
