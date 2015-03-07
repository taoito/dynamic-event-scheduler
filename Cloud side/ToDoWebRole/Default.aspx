<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="ToDoWebRole._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to ASP.NET!
    </h2>
    <p>
        To learn more about ASP.NET visit <a href="http://www.asp.net" title="ASP.NET Website">www.asp.net</a>.
    </p>
    <p>
        You can also find <a href="http://go.microsoft.com/fwlink/?LinkID=152368&amp;clcid=0x409"
            title="MSDN ASP.NET Docs">documentation on ASP.NET at MSDN</a>.
    </p>


    
    <asp:GridView 
        ID="GridView1"
        DataSourceID="todoData"
        DataKeyNames="PartitionKey"
        AllowPaging="false"
        AutoGenerateColumns="true"
        GridLines = "Vertical"
        Runat="server"
        BackColor="AliceBlue"
        ForeColor="Black">
        <Columns>
            <asp:CommandField ShowDeleteButton="true"  />
        </Columns>
        <RowStyle BackColor="Aquamarine" />
        <FooterStyle BackColor="CadetBlue" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
    </asp:GridView>    

    <asp:ObjectDataSource runat="server" ID="todoData" TypeName="ToDoWebRole.ToDoDataSource"
        DataObjectTypeName="ToDoWebRole.ToDoDataModel" 
        SelectMethod="Select" DeleteMethod="Delete" InsertMethod="Insert">    
    </asp:ObjectDataSource>
    
     
    <asp:GridView 
        ID="GridView2"
        DataSourceID="locationData"
        DataKeyNames="PartitionKey"
        AllowPaging="false"
        AutoGenerateColumns="true"
        GridLines = "Vertical"
        Runat="server"
        BackColor="AliceBlue"
        ForeColor="Black">
        <Columns>
            <asp:CommandField ShowDeleteButton="true" />
        </Columns>
        <RowStyle BackColor="Aquamarine" />
        <FooterStyle BackColor="CadetBlue" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
    </asp:GridView>    

    <asp:ObjectDataSource runat="server" ID="locationData" TypeName="ToDoWebRole.LocationDataSource"
        DataObjectTypeName="ToDoWebRole.LocationDataModel" 
        SelectMethod="Select" DeleteMethod="Delete" InsertMethod="Insert">    
    </asp:ObjectDataSource>  
    
    
    
     <asp:GridView 
        ID="GridView3"
        DataSourceID="identityData"
        DataKeyNames="PartitionKey"
        AllowPaging="false"
        AutoGenerateColumns="true"
        GridLines = "Vertical"
        Runat="server"
        BackColor="AliceBlue"
        ForeColor="Black">
        <Columns>
            <asp:CommandField ShowDeleteButton="true"  />
        </Columns>
        <RowStyle BackColor="Aquamarine" />
        <FooterStyle BackColor="CadetBlue" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
    </asp:GridView>    

    <asp:ObjectDataSource runat="server" ID="identityData" TypeName="ToDoWebRole.IdentityDataSource"
        DataObjectTypeName="ToDoWebRole.IdentityDataModel" 
        SelectMethod="Select" DeleteMethod="Delete" InsertMethod="Insert">    
    </asp:ObjectDataSource>  
    
    
     

   
</asp:Content>
