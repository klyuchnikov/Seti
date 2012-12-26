<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Projects.aspx.cs" Inherits="Lab4.Projects" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" CellPadding="2"
        DataKeyNames="Id" ForeColor="Black" GridLines="None">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Имя Проекта" SortExpression="Name" />
            <asp:BoundField DataField="DataStart" HeaderText="Дата начала" SortExpression="DataStart" />
            <asp:BoundField DataField="DataEnd" HeaderText="Дата окончания" SortExpression="DataEnd" />
            <asp:BoundField DataField="Description" HeaderText="Описание" SortExpression="Description" />
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
    <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataKeyNames="Id"
        DataSourceID="SqlDataSource2" Height="50px" Width="398px" 
        BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" 
        CellPadding="2" ForeColor="Black" GridLines="None" 
        onitemdeleted="DetailsView1_ItemDeleted" 
        oniteminserted="DetailsView1_ItemInserted" 
        onitemupdated="DetailsView1_ItemUpdated" Visible="False">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <EditRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <Fields>
            <asp:BoundField DataField="Id" HeaderText="Имя Проекта" SortExpression="Id" Visible="False" />
            <asp:BoundField DataField="Name" HeaderText="Имя Проекта" SortExpression="Name" />
            <asp:BoundField DataField="DataStart" HeaderText="Дата начала" SortExpression="DataStart" />
            <asp:BoundField DataField="DataEnd" HeaderText="Дата окончания" SortExpression="DataEnd" />
            <asp:BoundField DataField="Description" HeaderText="Описание" SortExpression="Description" />
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
        </Fields>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
            HorizontalAlign="Center" />
    </asp:DetailsView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:dbConnectionString %>"
        SelectCommand="SELECT * FROM [Project]" DeleteCommand="DELETE FROM [Project] WHERE [Id] = @Id"
        InsertCommand="INSERT INTO [Project] ([Name], [DataStart], [DataEnd], [Description]) VALUES (@Name, @DataStart, @DataEnd, @Description)"
        UpdateCommand="UPDATE [Project] SET [Name] = @Name, [DataStart] = @DataStart, [DataEnd] = @DataEnd, [Description] = @Description WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="DataStart" Type="String" />
            <asp:Parameter Name="DataEnd" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="DataStart" Type="String" />
            <asp:Parameter Name="DataEnd" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        DeleteCommand="DELETE FROM [Project] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Project] ([Name], [DataStart], [DataEnd], [Description]) VALUES (@Name, @DataStart, @DataEnd, @Description)"
        SelectCommand="SELECT * FROM [Project] WHERE ([Id] = @Id)" 
        UpdateCommand="UPDATE [Project] SET [Name] = @Name, [DataStart] = @DataStart, [DataEnd] = @DataEnd, [Description] = @Description WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="DataStart" Type="String" />
            <asp:Parameter Name="DataEnd" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="Id" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="DataStart" Type="String" />
            <asp:Parameter Name="DataEnd" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
</asp:Content>
