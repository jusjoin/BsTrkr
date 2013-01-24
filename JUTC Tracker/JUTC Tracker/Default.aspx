<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JUTC_Tracker.Default" %>

<%@ Register assembly="GMaps" namespace="Subgurim.Controles" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml">
<head id="Head1" runat="server">
    <title>Welcome to the JUTC Bus Tracker Prototype</title>
    
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 251px;
        }
        .style3
        {
            width: 657px;
            text-align: center;
        }
        #divDirections
        {
            width: 701px;
        }
        .style4
        {
            text-decoration: underline;
        }
    </style>
</head>
<body onload="initialize()">
    <form id="form1" runat="server">
    <table class="style1">
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/jutc_banner.gif" 
                    style="text-align: center" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Label ID="lblStops" runat="server" Text="Stops For:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlStopList" runat="server" AppendDataBoundItems="True" 
                    AutoPostBack="True" 
                    onselectedindexchanged="ddlStopList_SelectedIndexChanged" 
                    DataSourceID="RouteNoDataSource" DataTextField="RouteNo" 
                    DataValueField="RouteNo">
                    <asp:ListItem Selected="True">No Stops</asp:ListItem>
                    <asp:ListItem>All Stops</asp:ListItem>
                </asp:DropDownList>
                <br />
                <asp:SqlDataSource ID="RouteNoDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                    SelectCommand="SELECT DISTINCT [RouteNo] FROM [BusRoute]"></asp:SqlDataSource>
                <br />
                <br />
                <asp:Label ID="lblRouteNos" runat="server" Text="Route Numbers:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlRouteNoList" runat="server" 
                    DataSourceID="RouteNoDataSource" DataTextField="RouteNo" 
                    DataValueField="RouteNo" AutoPostBack="True">
                </asp:DropDownList>
                <br />
                <br />
                <asp:Label ID="lblTime" runat="server" Text="Tabled Time:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlTabledTime" runat="server" 
                    DataSourceID="TTimeDataSource" DataTextField="RouteNo" DataValueField="RouteNo">
                </asp:DropDownList>
                <br />
                <br />
                <asp:SqlDataSource ID="TTimeDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                    
                    SelectCommand="SELECT [RouteNo], [Time], [Day] FROM [TimeTable] WHERE (([RouteNo] = @RouteNo) AND ([Day] = @Day))">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlRouteNoList" Name="RouteNo" 
                            PropertyName="SelectedValue" Type="String" />
                        <asp:SessionParameter Name="Day" SessionField="Today" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <br />
                <br />
                <asp:DropDownList ID="ddlShowBuses" runat="server" 
                    onselectedindexchanged="ddlShowBuses_SelectedIndexChanged" 
                    AutoPostBack="True">
                    <asp:ListItem>Don&#39;t show buses</asp:ListItem>
                    <asp:ListItem>Show all buses</asp:ListItem>
                    <asp:ListItem>Show buses for selected route</asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />
            </td>
            <td style="text-align: center" class="style3">
                <cc1:GMap ID="GMap1" runat="server" Height="500px" Width="700px" />
            </td>
            <td style="text-align: center">
                <asp:Button ID="btnHWTTC" runat="server" onclick="btnHWTTC_Click" 
                    Text="Go To Transport Centre" />
                <br />
                <asp:Label ID="lblLatLong" runat="server" 
                    Text="Coordinates (Seperated by comma (,))"></asp:Label>
                <br />
                <asp:TextBox ID="txtLatLong" runat="server" ValidationGroup="1"></asp:TextBox>
                <br />
                <asp:Button ID="btnGo" runat="server" onclick="btnGo_Click" Text="Go" 
                    ValidationGroup="1" />
                <br />
                <asp:RequiredFieldValidator ID="latlongRequired" runat="server" 
                    ControlToValidate="txtLatLong" 
                    ErrorMessage="Lattitude &amp; Longitude are required" ValidationGroup="1" 
                    Display="Dynamic"></asp:RequiredFieldValidator>
                <br />
                </td>
        </tr>
        <tr>
            <td class="style2" rowspan="2">
                &nbsp;</td>
            <td class="style3">
&nbsp;
                </td>
            <td rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style3">
                <asp:LoginView ID="LoginView1" runat="server">
                    <LoggedInTemplate>
                        Welcome
                        <asp:LoginName ID="LoginName1" runat="server" />
                        ! Click here to
                        <asp:LoginStatus ID="LoginStatus2" runat="server" />
                        <br />
                        <br />
                        _________<span class="style4">Create Notification</span>_________<br />
                        Choose a stop on the map<br />
                        <asp:Label ID="lblRouteNo" runat="server" Text="Route #: "></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:DropDownList ID="ddlRouteNo" runat="server" AutoPostBack="True" 
                            DataSourceID="RouteNoDataSource" DataTextField="RouteNo" 
                            DataValueField="RouteNo">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="lblDay" runat="server" Text="Day: "></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:DropDownList ID="ddlDay" runat="server" AutoPostBack="True" 
                            DataSourceID="DayDataSource" DataTextField="Day" DataValueField="Day">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="lblTabledTime" runat="server" Text="Tabled Time: "></asp:Label>
                        <asp:DropDownList ID="DropDownList3" runat="server" 
                            DataSourceID="TTimeDataSource2" DataTextField="Time" DataValueField="Time">
                        </asp:DropDownList>
                        <br />
                        <asp:Button ID="btnCreateNotification" runat="server" 
                            Text="Create Notification" />
                        <br />
                        <asp:SqlDataSource ID="DayDataSource" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                            SelectCommand="SELECT DISTINCT [Day] FROM [TimeTable] WHERE ([RouteNo] = @RouteNo) ORDER BY [Day]">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlRouteNo" Name="RouteNo" 
                                    PropertyName="SelectedValue" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <br />
                        <asp:SqlDataSource ID="TTimeDataSource2" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                            SelectCommand="SELECT [Time] FROM [TimeTable] WHERE (([RouteNo] = @RouteNo) AND ([Day] = @Day)) ORDER BY [Time]">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlRouteNo" Name="RouteNo" 
                                    PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="ddlDay" Name="Day" 
                                    PropertyName="SelectedValue" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </LoggedInTemplate>
                    <AnonymousTemplate>
                        You are not lpgged. To use notifications feature you must be a member.<br />
                        <asp:LoginStatus ID="LoginStatus1" runat="server" 
                            LoginText="Click here to login." />
                    </AnonymousTemplate>
                </asp:LoginView>
            </td>
        </tr>
        <tr>
            <td class="style2" rowspan="2">
                &nbsp;</td>
            <td class="style3">
                    &nbsp;</td>
            <td rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style3">
                &nbsp;</tr>
        </table>
    </form>
</body>
</html>
