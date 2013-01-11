<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SalarySUM.aspx.cs" Inherits="Lab4.SalarySUM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="SELECT   Employee.FirstName, Employee.LastName, Employee.Patronymic, SUM(EmployeeProject.OfficialSalary)AS SumTotal 
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId
group BY Employee.FirstName, Employee.LastName, Employee.Patronymic
"></asp:SqlDataSource>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="LightGoldenrodYellow"
        BorderColor="Tan" BorderWidth="1px" CellPadding="2" DataSourceID="SqlDataSource1"
        ForeColor="Black" GridLines="None">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="FirstName" HeaderText="Фамилия" SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="Имя" SortExpression="LastName" />
            <asp:BoundField DataField="Patronymic" HeaderText="Отчество" SortExpression="Patronymic" />
            <asp:BoundField DataField="SumTotal" HeaderText="Суммарная оклад" ReadOnly="True"
                SortExpression="SumTotal" />
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
</asp:Content>
