<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="PhoneSimulator.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Login to Phone Simulator</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Login ID="Login1" runat="server" DestinationPageUrl="PhoneSim.aspx">
    </asp:Login>
    </form>
</body>
</html>
