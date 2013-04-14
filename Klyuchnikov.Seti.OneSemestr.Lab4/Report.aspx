<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="Klyuchnikov.Seti.OneSemestr.Lab4.Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="SELECT     Employee.FirstName, SUM(EmployeeProject.CountDaysWork) AS SumTotal
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId
GROUP BY Employee.FirstName, EmployeeProject.ProjectId
HAVING      (EmployeeProject.ProjectId = @id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="id" 
                PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="ProjectsD" 
        DataTextField="Name" DataValueField="Id" AutoPostBack="True">
    </asp:DropDownList>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="LightGoldenrodYellow"
        BorderColor="Tan" BorderWidth="1px" CellPadding="2" DataSourceID="SqlDataSource1"
        ForeColor="Black" GridLines="None">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="FirstName" HeaderText="Фамилия сотрудника" 
                SortExpression="FirstName" />
            <asp:BoundField DataField="SumTotal" HeaderText="Количество отработанных дней" 
                SortExpression="SumTotal" ReadOnly="True" />
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
    <asp:SqlDataSource ID="ProjectsD" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
        SelectCommand="SELECT * FROM [Project]"></asp:SqlDataSource>
    <br />
</asp:Content>
