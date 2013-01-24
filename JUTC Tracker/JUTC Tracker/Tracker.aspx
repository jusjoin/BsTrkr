<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tracker.aspx.cs" Inherits="JUTC_Tracker.Tracker" %>

<%@ Register assembly="GMaps" namespace="Subgurim.Controles" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml">
<head runat="server">
    <title>Bus Tracker</title>
    <script src="http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAN3fwc6GxwerSzTl7BJKTnBT2yXp_ZAY8_ufC3CFXhHIE1NvwkxQ2itl5_rVpK_5nE5mwaSQ2Pb9hzg&sensor=false"
        type="text/javascript"></script>
    <script type="text/javascript">

        var coordsList, distList, coordsListArr, gDir, cnt;


        function initialize() {
            gDir = new GDirections();
            GEvent.addListener(gDir, "load", function() {
                var drivingDistanceMeters = gDir.getDistance().meters;
                alert(drivingDistanceMeters);
            });
        }
        function getDirections() {
            coordsList = document.getElementById('hidCoordsList').value;
            distList = document.getElementById('hidDistList').value;
            coordsListArr = coordsList.split('|');
            for (cnt = 0; cnt < coordsListArr.length-1; cnt++) 
            {
                setTimeout("gDir.load('from: ' + coordsListArr[cnt] + ' to: ' + coordsListArr[cnt + 1])", 100);
                setTimeout("var temp = gDir.getDistance().meters",100);
                setTimeout("distList += temp + ','",100);
            }
            var delay = cnt * 110;
            setTimeout("document.getElementById('hidDistList').value = distList", delay);
            setTimeout("alert('Click Get Distances')", delay);
        }
    </script>
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
    </style>
</head>
<body onload="initialize()">
    <form id="form1" runat="server">
    <input id="hidCoords" type="hidden" value="" runat="server"/>
    <input id="hidStopNo" type="hidden" value="" runat="server"/>
    <input id="hidStopsList" type="hidden" value="" runat="server"/>
    <input id="hidDistList" type="hidden" value="" runat="server"/>
    <input id="hidCoordsList" type="hidden" value="" runat="server"/>
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
                    SelectCommand="SELECT DISTINCT [RouteNo] FROM [Route]"></asp:SqlDataSource>
                <br />
                <br />
                <asp:Label ID="lblRouteNos" runat="server" Text="Route Numbers:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlRouteNoList" runat="server" 
                    DataSourceID="RouteNoDataSource" DataTextField="RouteNo" 
                    DataValueField="RouteNo">
                </asp:DropDownList>
                <br />
                <asp:DropDownList ID="ddlShowBuses" runat="server" 
                    onselectedindexchanged="ddlShowBuses_SelectedIndexChanged">
                    <asp:ListItem>Don&#39;t show buses</asp:ListItem>
                    <asp:ListItem>Show all buses</asp:ListItem>
                    <asp:ListItem>Show buses for selected route</asp:ListItem>
                </asp:DropDownList>
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
                <asp:Label ID="lblStopA" runat="server" Text="Stop A: "></asp:Label>
                <asp:TextBox ID="txtStopA" runat="server"></asp:TextBox>
&nbsp;
                <asp:Label ID="lblStopB" runat="server" Text="Stop B: "></asp:Label>
                <asp:TextBox ID="txtStopB" runat="server"></asp:TextBox>
                <input id="btnPlot" type="button" value="Plot Course" /></td>
            <td rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style3">
                /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\</td>
        </tr>
        <tr>
            <td class="style2" rowspan="2">
                &nbsp;</td>
            <td class="style3">
                    <asp:Button ID="btnAddStop" runat="server" Text="Add Stop" 
                    onclick="btnAddStop_Click" ValidationGroup="2" />
                    <br />
                    <asp:Label ID="lblStopNo" runat="server" Text="Stop #"></asp:Label>
                    &nbsp;
                    <asp:Label ID="lblLong" runat="server" Text="Location"></asp:Label>
                    <br />
                    <asp:RequiredFieldValidator ID="stopnoRequired" runat="server" 
                    ControlToValidate="txtStopNo" ErrorMessage="Stop # is required" 
                    ValidationGroup="2"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="txtStopNo" runat="server" ValidationGroup="2"></asp:TextBox>
                    &nbsp;
                    <asp:TextBox ID="txtLocation" runat="server" 
    ValidationGroup="2"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="locationRequired" runat="server" 
                    ControlToValidate="txtLocation" ErrorMessage="Location is required" 
                    ValidationGroup="2"></asp:RequiredFieldValidator>
                    <br />
                    <asp:Label ID="lblLatLong2" runat="server" 
                    Text="Coordinates (Seperated by comma (,))"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtLatLong2" runat="server" 
    ValidationGroup="2"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="latlongRequired2" runat="server" 
                    ControlToValidate="txtLatLong2" 
                    ErrorMessage="Lattitude &amp; Longitude are required" 
    ValidationGroup="2"></asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <asp:DropDownList ID="ddlStopsList" runat="server" 
                        DataSourceID="StopsDataSource" DataTextField="StopNo" DataValueField="StopNo">
                    </asp:DropDownList>
&nbsp;&nbsp;
                    <asp:Button ID="btnDeleteStop" runat="server" onclick="btnDeleteStop_Click" 
                        Text="Delete Stop" />
                    <br />
                    <asp:SqlDataSource ID="StopsDataSource" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                        SelectCommand="SELECT [StopNo] FROM [Stops]"></asp:SqlDataSource>
            </td>
            <td rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style3">
                /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\</tr>
        <tr>
            <td class="style2">
                &nbsp;</td>
            <td class="style3">
                <asp:Button ID="btnAddRoute" runat="server" onclick="btnAddRoute_Click" 
                    Text="Add Route" ValidationGroup="3" />
                <br />
                <asp:Label ID="lblRouteNo" runat="server" Text="Route #: "></asp:Label>
                <br />
                <asp:TextBox ID="txtRouteNo" runat="server" ValidationGroup="3"></asp:TextBox>
                <br />
                <asp:Label ID="lblRouteStops" runat="server" 
                    Text="Route Stops (Stop Numbers seperated by (,))"></asp:Label>
                <br />
                <asp:TextBox ID="txtRouteStops" runat="server" Columns="100" Rows="3" 
                    ValidationGroup="3"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="StopsRequired" runat="server" 
                    ErrorMessage="List of stops required" ControlToValidate="txtRouteStops" 
                    ValidationGroup="3"></asp:RequiredFieldValidator>
&nbsp;
                <asp:Label ID="lblStopsError" runat="server" ForeColor="Red" Text="Stops Error" 
                    Visible="False"></asp:Label>
                <br />
                <br />
                <asp:Label ID="lblCoords" runat="server" 
                    Text="Retrieved Coordinates"></asp:Label>
                <asp:TextBox ID="txtCoordinates" runat="server" Columns="100" Rows="3" 
                    ValidationGroup="3"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="lblRouteDistances" runat="server" 
                    Text="Route Distances (Distances between each stop seperated by (,))"></asp:Label>
                <asp:TextBox ID="txtStopDistances" runat="server" Columns="100" Rows="3" 
                    ValidationGroup="3"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="DistancesRequired" runat="server" 
                    ErrorMessage="List of distances required" 
                    ControlToValidate="txtStopDistances" ValidationGroup="3"></asp:RequiredFieldValidator>
                <asp:Label ID="lblDistancesError" runat="server" ForeColor="Red" 
                    Text="Distances Error" Visible="False"></asp:Label>
                <br />
                <br />
                <asp:Button ID="btnConvertStops" runat="server" onclick="btnConvertStops_Click" 
                    Text="Get Stops, Coordinates &amp; Distances" />
                <br />
                <br />
                <asp:DropDownList ID="ddlRouteList" runat="server" 
                    datasourceid="RouteNoDataSource" DataTextField="RouteNo" 
                    DataValueField="RouteNo">
                </asp:DropDownList>
&nbsp;
                <asp:Button ID="btnDeleteRoute" runat="server" onclick="btnDeleteRoute_Click" 
                    Text="Delete Route" />
                <div id="divDirections">
                </div>
            </td>
            <td>
                </td>
        </tr>
    </table>
    </form>
</body>
</html>
