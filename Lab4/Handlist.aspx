<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Handlist.aspx.cs" Inherits="Lab4.Handlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Начальная буква:<asp:DropDownList ID="startDDL" runat="server" 
            AutoPostBack="True" >
        </asp:DropDownList>
    </p>
    <p>
        Конечная буква:<asp:DropDownList ID="endDDL" runat="server" >
        </asp:DropDownList>
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" Text="Найти" onclick="Button1_Click" />
    </p>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
    BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" 
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
