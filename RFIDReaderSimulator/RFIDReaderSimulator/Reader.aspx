<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reader.aspx.cs" Inherits="RFIDReaderSimulator._Reader" %>

<%@ Register assembly="GMaps" namespace="Subgurim.Controles" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>RFID Reader Simulator</title>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <input id="hidStopNo" type="hidden" value="" runat="server"/>
    <table class="style1" align="center">
        <tr>
            <td>
                &nbsp;</td>
            <td class="style2">
                <cc1:GMap ID="GMap1" runat="server" Height="500px" style="text-align: center" 
                    Width="700px" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td id=" " class="style2">
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                    SelectCommand="SELECT [LicensePlate], [Status] FROM [Buses] WHERE ([Status] = @Status)">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="InTransit" Name="Status" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:Label ID="lblBus" runat="server" Text="Bus: "></asp:Label>
                <asp:DropDownList ID="ddlBusList" runat="server" DataSourceID="SqlDataSource1" 
                    DataTextField="LicensePlate" DataValueField="LicensePlate" 
                    onselectedindexchanged="ddlBusList_SelectedIndexChanged">
                </asp:DropDownList>
&nbsp;
                <asp:Label ID="lblSpeed" runat="server" Text="Speed: "></asp:Label>
&nbsp;
                <asp:TextBox ID="txtSpeed" runat="server"></asp:TextBox>
&nbsp;
                <asp:Button ID="btnSendSignal" runat="server" Text="Send Signal" 
                    onclick="btnSendSignal_Click" />
            &nbsp;
                <asp:Button ID="Test" runat="server" onclick="Test_Click" Text="Test" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td class="style2">
                <asp:Label ID="lblMessages" runat="server" Text="Messages"></asp:Label>
                <br />
                <asp:TextBox ID="txtMessage" runat="server" Height="183px" Width="468px" 
                    ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    </form>
</body>
</html>
