<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Employees.aspx.cs" Inherits="Lab4.Employees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        DataKeyNames="Id" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px"
        CellPadding="2" ForeColor="Black" GridLines="None">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False"
                ReadOnly="True" Visible="False" />
            <asp:BoundField DataField="FirstName" HeaderText="Фамилия" 
                SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="Имя" 
                SortExpression="LastName" />
            <asp:BoundField DataField="Patronymic" HeaderText="Отчество" 
                SortExpression="Patronymic" />
            <asp:BoundField DataField="Address" HeaderText="Адрес" 
                SortExpression="Address" />
            <asp:BoundField DataField="Characterization" HeaderText="Характеристика" 
                SortExpression="Characterization" />
            <asp:ImageField DataImageUrlField="AvatarName"   DataImageUrlFormatString="/Avatars/{0}" HeaderText="Фотография">
            </asp:ImageField>
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
    <asp:DetailsView ID="DetailsView1" Visible="False" runat="server" AutoGenerateRows="False"
        BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" CellPadding="2"
        DataKeyNames="Id" DataSourceID="SqlDataSource2" ForeColor="Black" GridLines="None"
        Height="50px" Width="125px" onitemdeleted="DetailsView1_ItemDeleted" 
        oniteminserted="DetailsView1_ItemInserted" 
        onitemupdated="DetailsView1_ItemUpdated">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <EditRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <Fields>
            <asp:BoundField DataField="FIO" HeaderText="ФИО" SortExpression="FIO" />
            <asp:BoundField DataField="Address" HeaderText="Адрес" SortExpression="Address" />
            <asp:BoundField DataField="Characterization" HeaderText="Характеристика" SortExpression="Characterization" />
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" 
                ShowInsertButton="True" />
        </Fields>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
    </asp:DetailsView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="SELECT * FROM [Employee]" DeleteCommand="DELETE FROM [Employee] WHERE [Id] = @Id"
        InsertCommand="INSERT INTO [Employee] ([FIO], [Address], [Characterization]) VALUES (@FIO, @Address, @Characterization)"
        UpdateCommand="UPDATE [Employee] SET [FIO] = @FIO, [Address] = @Address, [Characterization] = @Characterization WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="FIO" Type="String" />
            <asp:Parameter Name="Address" Type="String" />
            <asp:Parameter Name="Characterization" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="FIO" Type="String" />
            <asp:Parameter Name="Address" Type="String" />
            <asp:Parameter Name="Characterization" Type="String" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        DeleteCommand="DELETE FROM [Employee] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Employee] ([FIO], [Address], [Characterization]) VALUES (@FIO, @Address, @Characterization)"
        SelectCommand="SELECT * FROM [Employee] WHERE ([Id] = @Id)" UpdateCommand="UPDATE [Employee] SET [FIO] = @FIO, [Address] = @Address, [Characterization] = @Characterization WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="FIO" Type="String" />
            <asp:Parameter Name="Address" Type="String" />
            <asp:Parameter Name="Characterization" Type="String" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="Id" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="FIO" Type="String" />
            <asp:Parameter Name="Address" Type="String" />
            <asp:Parameter Name="Characterization" Type="String" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>
