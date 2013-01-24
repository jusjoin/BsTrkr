<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhoneSim.aspx.cs" Inherits="PhoneSimulator._PhoneSim" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Phone Simulator</title>
    <meta http-equiv="refresh" content="60">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table class="style1">
        <tr>
            <td>
                &nbsp;</td>
            <td style="text-align: center">
                <asp:Label ID="lblTitle" runat="server" Text="Phone Sim"></asp:Label>
                <br />
                <asp:TextBox ID="txtMessage" runat="server" Height="215px" TextMode="MultiLine" 
                    Width="351px"></asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td style="text-align: center">
                <asp:Button ID="btnGetMsg" runat="server" onclick="btnGetMsg_Click" 
                    Text="Get Messages" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td rowspan="2">
                &nbsp;</td>
            <td style="text-align: center">
                <asp:Button ID="btnSendMessage" runat="server" onclick="btnSendMessage_Click" 
                    Text="Send Message" />
            </td>
            <td rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:TextBox ID="txtSendMessage" runat="server" Columns="140" Height="45px" 
                    style="text-align: center" Width="406px"></asp:TextBox>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
