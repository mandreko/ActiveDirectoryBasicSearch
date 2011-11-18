<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ActiveDirectoryBasicSearch.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" defaultbutton="submitButton">
        <div id="search">
            Name: <asp:TextBox runat="server" ID="nameTextBox" ></asp:TextBox>
            
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                        ErrorMessage="Name is required." ControlToValidate="nameTextBox"></asp:RequiredFieldValidator>

            <asp:Button runat="server" ID="submitButton" Text="Submit" onclick="submitButton_Click"/>
        </div>
        <div id="searchResults">
            <asp:GridView ID="searchResultsGrid" runat="server" AutoGenerateColumns="False" 
                          EmptyDataText="No Contacts Found">
                <Columns>
                    <asp:BoundField DataField="DisplayName" HeaderText="Display Name" />
                    <asp:BoundField DataField="Phone" HeaderText="Phone" />
                    <asp:BoundField DataField="Office" HeaderText="Office" />
                </Columns>
            </asp:GridView>
        </div>
        
    </form>
    
</body>
</html>
