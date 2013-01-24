<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tracker.aspx.cs" Inherits="JUTC_Tracker.Tracker" %>
<%@ Register assembly="GMaps" namespace="Subgurim.Controles" tagprefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml">
<head runat="server">
    <title>Bus Tracker</title>
    <script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyB1w1PPE_Neth5tE1CunyNm02OxaysoG00&sensor=true" type="text/javascript"></script>
    <script type="text/javascript" src="/js/tracker.js"></script>
    <link rel="stylesheet" type="text/css" href="/css/tracker.css"/>
    <style type="text/css">
        #Button1
        {
            height: 25px;
            width: 29px;
        }
    </style>
</head>
<body onload="initialize()" class="adminTracker">
    <form id="form1" runat="server">
    <input id="hidCoords" type="hidden" value="" runat="server"/>
    <input id="hidStopNo" type="hidden" value="" runat="server"/>
    <input id="hidStopsList" type="hidden" value="" runat="server"/>
    <input id="hidDistList" type="hidden" value="" runat="server"/>
    <input id="hidCoordsList" type="hidden" value="" runat="server"/>
    <div id="hiddenData" style="display: none;"></div>
    <table>
        <tr>
            <td >
                &nbsp;</td>
            <td>
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/jutc_banner.gif" 
                    style="text-align: center" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td >
                <asp:Label ID="lblStops" runat="server" Text="Show Stops For:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlStopList" runat="server" AppendDataBoundItems="True" 
                    onchange="ddlStopList_SelectedIndexChanged()" 
                    DataSourceID="RouteNoDataSource" DataTextField="RouteNo" 
                    DataValueField="RouteNo">
                    <asp:ListItem Selected="True">No Stops</asp:ListItem>
                    <asp:ListItem>All Stops</asp:ListItem>
                </asp:DropDownList>

                <asp:SqlDataSource ID="RouteNoDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                    SelectCommand="SELECT [RouteNo] FROM [TravelRoutes]"></asp:SqlDataSource>
                <br />
                <br />
                <br />
                <asp:Label ID="lblRouteNos" runat="server" Text="Route Numbers:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlRouteNoList" runat="server" 
                    DataSourceID="RouteNoDataSource" DataTextField="RouteNo" 
                    DataValueField="RouteNo" AutoPostBack="True">
                </asp:DropDownList>
                <br />
                <asp:Label ID="lblBusWaiting" runat="server" Text="Waiting Buses:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlBusWaiting" runat="server" 
                    DataSourceID="BusDataSource" DataTextField="LicensePlate" 
                    DataValueField="LicensePlate" AutoPostBack="True">
                </asp:DropDownList>
                <br />
                <asp:Label ID="lblTime" runat="server" Text="Tabled Time:"></asp:Label>
                <br />
                <asp:DropDownList ID="ddlTabledTime" runat="server" 
                    DataSourceID="TTimeDataSource" DataTextField="Time" DataValueField="Time">
                </asp:DropDownList>
                <br />
                <asp:Button ID="btnAddBus" runat="server" onclick="btnAddBus_Click" 
                    Text="Put Bus on Route" />
                <br />
                <asp:SqlDataSource ID="BusDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                    
                    SelectCommand="SELECT [LicensePlate], [Type], [BusStatus] FROM [Buses] WHERE ([BusStatus] = @Status)">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="Waiting" Name="Status" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="TTimeDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:JUTCConnectionString %>" 
                    SelectCommand="SELECT [Time], [Day], [RouteNo] FROM [TimeTable] WHERE (([RouteNo] = @RouteNo) AND ([Day] = @Day))">
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
            <td style="text-align: center" >
                <div id="theMap"></div>
                <br />
                Click stop markers on the map to see stop information
                <cc1:GMap ID="GMap1" runat="server" Height="500px" Width="700px" 
                    Visible="False" />
            </td>
            <td style="text-align: center">
                <asp:Button ID="btnHWTTC" runat="server" onclick="btnHWTTC_Click" 
                    Text="Go To Transport Centre" />
                <br />
                <input id="btnHWTTC1" type="button" value="Go To Transport Centre" onclick="go(hwttc)" /><br />
                <asp:Label ID="lblLatLong" runat="server" 
                    Text="Coordinates (Seperated by comma (,))"></asp:Label>
                <br />
                <asp:TextBox ID="txtLatLong" runat="server" ValidationGroup="1"></asp:TextBox>
                <br />
                <asp:Button ID="btnGo" runat="server" onclick="btnGo_Click" Text="Go" 
                    ValidationGroup="1" />
                <br />
                <input id="btnGo1" type="button" value="Go" onclick="go(pointToLatLng(txtLatLong.value))"/><br />
                <asp:RequiredFieldValidator ID="latlongRequired" runat="server" 
                    ControlToValidate="txtLatLong" 
                    ErrorMessage="Lattitude &amp; Longitude are required" ValidationGroup="1" 
                    Display="Dynamic"></asp:RequiredFieldValidator>
                <br />
                </td>
        </tr>
        <tr>
            <td  rowspan="2">
                &nbsp;</td>
            <td >
                <asp:Label ID="lblStopA" runat="server" Text="Stop A: "></asp:Label>
                <asp:TextBox ID="txtStopA" runat="server"></asp:TextBox>
&nbsp;
                <asp:Label ID="lblStopB" runat="server" Text="Stop B: "></asp:Label>
                <asp:TextBox ID="txtStopB" runat="server"></asp:TextBox>
                <input id="btnPlot" type="button" value="Plot Course" /><br />
                Plots the course by road on the map between two stops given.</td>
            <td rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td >
                /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\</td>
        </tr>
        <tr>
            <td  rowspan="2">
                &nbsp;</td>
            <td >
                    <input id="btnAddStop" type="button" value="Add Stop" onclick="javascript: ('1')"/><br />
                    Add a stop marker to the map. Describe below - give stop#, location, and enter 
                    coordinates<br />
                    <asp:Label ID="lblStopNo" runat="server" Text="Stop #"></asp:Label>
                    &nbsp;
                    <asp:Label ID="lblLong" runat="server" Text="Location"></asp:Label>
                    <br />
                    <input id="txtStopNo" type="text" />&nbsp;
                    <input id="txtLocation" type="text" /><br />

                    <input type="radio" name="type" id="rbtype" value="Depot" />Depot
                    <input type="radio" name="type" value="BusStop" checked="checked" />Bus Stop<br />
                    <div id="addStopResponse" runat="server"></div>
                    <br />
                    <asp:DropDownList ID="ddlStopsList" runat="server" 
                        DataSourceID="StopsDataSource" DataTextField="StopNo" DataValueField="StopNo">
                    </asp:DropDownList>
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
            <td >
                /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                </td>
        </tr>
        <tr>
            <td >
                &nbsp;</td>
            <td >
                Create a trip for a certain route at a certain time. Use get stops button below 
                to get list of stops collected.<br />
                <asp:Button ID="btnAddRoute" runat="server" onclick="btnAddRoute_Click" 
                    Text="Create Route-Trip" ValidationGroup="3" />
                <br />
                <asp:Label ID="lblRouteNo" runat="server" Text="Route #: "></asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="lblTripTime" runat="server" Text="Trip Time: "></asp:Label>
                <br />
                <asp:TextBox ID="txtRouteNo" runat="server" ValidationGroup="3" Width="71px"></asp:TextBox>
                &nbsp;&nbsp;
                <asp:TextBox ID="txttripTime" runat="server" ValidationGroup="3" Width="71px"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RouteNoRequired" runat="server" 
                    ControlToValidate="txtRouteNo" ErrorMessage="Route Number Required" 
                    ValidationGroup="3"></asp:RequiredFieldValidator>
&nbsp;&nbsp;
                <asp:RequiredFieldValidator ID="RouteNoRequired0" runat="server" 
                    ControlToValidate="txttripTime" ErrorMessage="Trip Time Required" 
                    ValidationGroup="3"></asp:RequiredFieldValidator>
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
                    Text="Retrieved Coordinates"></asp:Label><br />
                <asp:TextBox ID="txtCoordinates" runat="server" Columns="100" Rows="3" 
                    ValidationGroup="3"></asp:TextBox>
                <br />
                <br />
                <br />
                <asp:Label ID="lblRouteDistances" runat="server" 
                    Text="Route Distances (Distances between each stop seperated by (,))"></asp:Label><br />
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
                Collect stops by clicking them on the map, click this button get the list of 
                stops and their details.<br />
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
