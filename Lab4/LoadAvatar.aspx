<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LoadAvatar.aspx.cs" Inherits="Lab4.LoadAvatar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
    SelectCommand="SELECT * FROM [Employee]"></asp:SqlDataSource>
<asp:DropDownList ID="DropDownList1" runat="server" 
    DataSourceID="SqlDataSource1" DataTextField="FirstName" DataValueField="Id">
</asp:DropDownList>
    <br />
    <asp:FileUpload ID="FileUpload1" runat="server"  />
    <br />
    <asp:Button ID="Button1" runat="server"
        Text="Загрузить" onclick="Button1_Click" />
</asp:Content>
