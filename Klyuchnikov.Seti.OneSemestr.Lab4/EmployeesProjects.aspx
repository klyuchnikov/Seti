<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EmployeesProjects.aspx.cs" Inherits="Klyuchnikov.Seti.OneSemestr.Lab4.EmployeesProjects" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="EmployeeD" DataTextField="FirstName"
        DataValueField="Id" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
        AutoPostBack="True">
    </asp:DropDownList>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        DataKeyNames="Id" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
        CellPadding="2" ForeColor="Black" GridLines="None" SelectedIndex="0">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
            <asp:BoundField DataField="Id" HeaderText="Должность" SortExpression="Id" Visible="False" />
            <asp:BoundField DataField="Name" HeaderText="Должность" SortExpression="Name" />
            <asp:BoundField DataField="Expr1" HeaderText="Проект" SortExpression="Expr1" />
            <asp:BoundField DataField="CountDaysWork" HeaderText="Количество отработанных дней"
                SortExpression="CountDaysWork" />
            <asp:BoundField DataField="OfficialSalary" HeaderText="Оклад" SortExpression="OfficialSalary" />
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
    <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" BackColor="LightGoldenrodYellow"
        BorderColor="Tan" BorderWidth="1px" CellPadding="2" DataSourceID="SqlDataSource2"
        ForeColor="Black" GridLines="None" Height="50px" Width="125px" ViewStateMode="Enabled"
        DataKeyNames="Id">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <EditRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <Fields>
            <asp:BoundField DataField="Id" HeaderText="Должность" SortExpression="Id" Visible="False" />
            <asp:BoundField DataField="Name" HeaderText="Должность" SortExpression="Name" />
            <asp:BoundField DataField="Expr1" HeaderText="Проект" SortExpression="Expr1" />
            <asp:BoundField DataField="CountDaysWork" HeaderText="Количество отработанных дней"
                SortExpression="CountDaysWork" />
            <asp:BoundField DataField="OfficialSalary" HeaderText="Оклад" SortExpression="OfficialSalary" />
        </Fields>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
    </asp:DetailsView>
    <asp:Panel ID="Panel1" runat="server" Visible="False">
        Привязка сотрудника к проекту:<br />
        Сотрудник:
        <asp:DropDownList ID="EmployeesDDL" runat="server" 
            DataSourceID="EmployeeD" DataTextField="FirstName"
            DataValueField="Id" 
            OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        Проект:
        <asp:DropDownList ID="ProjectsDDL" runat="server" DataSourceID="SqlDataSourceProjects"
            DataTextField="Name" DataValueField="Id">
        </asp:DropDownList>
        <br />
        Должность:
        <asp:DropDownList ID="PositionsDDL" runat="server" DataSourceID="SqlDataSourcePositions"
            DataTextField="Name" DataValueField="Id">
        </asp:DropDownList>
        <br />
        Оклад:
        <asp:TextBox ID="CountDaysWorkTB" runat="server"></asp:TextBox>
        <br />
        Количество отработанных часов:
        <asp:TextBox ID="OfficialSalaryTB" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="CreateB" runat="server" Text="Привязать" 
            onclick="Button1_Click" />
            <asp:Button ID="UpdateB" runat="server" Text="Изменить" 
            onclick="UpdateB_Click" />
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <asp:LinkButton ID="UpdateLB" runat="server" onclick="UpdateLB_Click">Изменить</asp:LinkButton>
        <asp:LinkButton ID="DeleteLB" runat="server" onclick="DeleteLB_Click">Удалить</asp:LinkButton>
    </asp:Panel>
    <br />
    <asp:Button ID="Button2" runat="server" Text="Привязать нового сотрудника" 
        onclick="Button2_Click" />
    <br />
    <br />
    <asp:SqlDataSource ID="EmployeeD" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="SELECT * FROM [Employee]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceProjects" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="SELECT * FROM [Project]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourcePositions" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="SELECT * FROM [Position]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" SelectCommand="SELECT     Project.Name, Position.Name AS Expr1, EmployeeProject.CountDaysWork, EmployeeProject.OfficialSalary, Employee.Id
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN
                      Position ON EmployeeProject.PositionId = Position.Id INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
WHERE     (Employee.Id = @id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="id" 
                PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" SelectCommand="SELECT     Project.Name, Position.Name AS Expr1, EmployeeProject.CountDaysWork, EmployeeProject.OfficialSalary, Employee.Id, EmployeeProject.Id AS Expr2
FROM         Employee INNER JOIN
                      EmployeeProject ON Employee.Id = EmployeeProject.EmployeeId INNER JOIN
                      Position ON EmployeeProject.PositionId = Position.Id INNER JOIN
                      Project ON EmployeeProject.ProjectId = Project.Id
WHERE     (Employee.Id =@id) AND (EmployeeProject.Id = @idEmployeeProject)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="id" 
                PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="GridView1" Name="idEmployeeProject" 
                PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <br />
</asp:Content>
