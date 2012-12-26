<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EmployeesProjects.aspx.cs" Inherits="Lab4.EmployeesProjects" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="EmployeeD" DataTextField="FIO"
        DataValueField="Id" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
        AutoPostBack="True">
    </asp:DropDownList>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        DataKeyNames="Id" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
        CellPadding="2" ForeColor="Black" GridLines="None" SelectedIndex="0">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
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
        BorderColor="Tan" BorderWidth="1px" CellPadding="2" DataKeyNames="Id" DataSourceID="SqlDataSource2"
        ForeColor="Black" GridLines="None" Height="50px" Width="125px" 
        ViewStateMode="Enabled">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <EditRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False"
                ReadOnly="True" />
            <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" SortExpression="EmployeeId" />
            <asp:BoundField DataField="CountDaysWork" HeaderText="CountDaysWork" SortExpression="CountDaysWork" />
            <asp:BoundField DataField="ProjectId" HeaderText="ProjectId" SortExpression="ProjectId" />
            <asp:BoundField DataField="OfficialSalary" HeaderText="OfficialSalary" SortExpression="OfficialSalary" />
            <asp:BoundField DataField="PositionId" HeaderText="PositionId" SortExpression="PositionId" />
        </Fields>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
    </asp:DetailsView>
    <br />
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:dbConnectionString %>"
        SelectCommand="SELECT * FROM [View_1] WHERE ([EmployeeId] = @EmployeeId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="EmployeeId" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        DeleteCommand="DELETE FROM [EmployeeProject] WHERE [Id] = @Id" InsertCommand="INSERT INTO [EmployeeProject] ([EmployeeId], [CountDaysWork], [ProjectId], [OfficialSalary], [PositionId]) VALUES (@EmployeeId, @CountDaysWork, @ProjectId, @OfficialSalary, @PositionId)"
        SelectCommand="SELECT * FROM [EmployeeProject] WHERE ([Id] = @Id)" UpdateCommand="UPDATE [EmployeeProject] SET [EmployeeId] = @EmployeeId, [CountDaysWork] = @CountDaysWork, [ProjectId] = @ProjectId, [OfficialSalary] = @OfficialSalary, [PositionId] = @PositionId WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="EmployeeId" Type="Int32" />
            <asp:Parameter Name="CountDaysWork" Type="String" />
            <asp:Parameter Name="ProjectId" Type="Int32" />
            <asp:Parameter Name="OfficialSalary" Type="String" />
            <asp:Parameter Name="PositionId" Type="Int32" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="Id" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="EmployeeId" Type="Int32" />
            <asp:Parameter Name="CountDaysWork" Type="String" />
            <asp:Parameter Name="ProjectId" Type="Int32" />
            <asp:Parameter Name="OfficialSalary" Type="String" />
            <asp:Parameter Name="PositionId" Type="Int32" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
    <br />
</asp:Content>
