using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Subgurim.Controles;
using System.Drawing;
using System.Configuration;

namespace JUTC_Tracker
{
    public partial class Tracker : System.Web.UI.Page
    {
        
        GLatLng hwttc = new GLatLng(18.01209505590662, -76.7985588312149);
        GLatLng lastclick = new GLatLng();
        JUTCDataContext JUTCLinq = new JUTCDataContext();
        MarkerManager mManager = new MarkerManager();
        string latlong;
        GIcon stopIcon = new GIcon(); //create bus stop marker
        GIcon busIcon = new GIcon(); //create bus marker
        GIcon depotIcon = new GIcon(); //create depot  marker
            

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Today"] = DateTime.Now.DayOfWeek.ToString();

            //reset listeners, necessary for marker listener to work between postback
            GMap1.resetListeners();

            //hide objects to be hidden
            lblDistancesError.Visible = false;
            lblStopsError.Visible = false;

            //Set bus stop marker properties
            stopIcon.image = "/images/busstop.png";
            stopIcon.iconSize = new GSize(32, 37);
            stopIcon.iconAnchor = new GPoint(15, 36);
            stopIcon.infoWindowAnchor = new GPoint(5, 1);
            
            //Set bus marker properties
            busIcon.image = "/images/bus.png";
            busIcon.iconSize = new GSize(32, 37);
            busIcon.iconAnchor = new GPoint(15, 36);
            busIcon.infoWindowAnchor = new GPoint(5, 1);

            //Set depot marker properties
            depotIcon.image = "/images/depot.png";
            depotIcon.iconSize = new GSize(32, 37);
            depotIcon.iconAnchor = new GPoint(15, 36);
            depotIcon.infoWindowAnchor = new GPoint(5, 1);

            GMap1.mapType = GMapType.GTypes.Hybrid;

            GMarkerOptions mOpts = new GMarkerOptions();
            mOpts.clickable = false;
            mOpts.icon = stopIcon;

            GMarker marker = new GMarker(hwttc, mOpts);
            //End create default marker
            
            /*//Begin Listener for click map
            marker.javascript_GLatLng = "point";
            GListener listener = new GListener(marker.ID, GListener.Event.dragend, "alertame");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("function(overlay, point) {");
            sb.Append("if (overlay){");
            sb.Append("alert(overlay.id);");
            sb.Append("}");
            sb.Append("else{");
            sb.Append(marker.ToString(GMap1.GMap_Id));
            sb.Append(listener.ToString());
            sb.Append("lastclick.lat = marker.point.lat;");
            sb.Append("lastclick.lng = marker.point.lng;");
            sb.Append("txtLat.text = Convert.ToToString(lastclick.lat);");
            sb.Append("txtLong.text = Convert.ToString(lastclick.lng);");
            sb.Append("}");
            sb.Append("}");

            GListener listener2 = new GListener(GMap1.GMap_Id, GListener.Event.click, sb.ToString());
            GMap1.addListener(listener2);
            //End Listener*/

            
            
            //GMap1.GZoom = 3; //set zoom level
            GMap1.Key = ConfigurationSettings.AppSettings["GoogleMapKey"];
            GMap1.enableDragging = true;
            GMap1.enableGoogleBar = false;
            GMap1.Language = "en";
            GMap1.BackColor = Color.White;
            //GMap1.enableHookMouseWheelToZoom = true;
            GMap1.enableGKeyboardHandler = true;
            
            //Add built-in control for selecting map type
            GMap1.addControl(new GControl(GControl.preBuilt.MapTypeControl));

            //Add built-in control for zoom and pan
            //GMap1.addControl(new GControl(GControl.preBuilt.SmallZoomControl3D));
            GMap1.addControl(new GControl(GControl.preBuilt.LargeMapControl3D));

            //Custom cursor
            GCustomCursor customCursor = new GCustomCursor(cursor.crosshair, cursor.text);
            GMap1.addCustomCursor(customCursor);

            //Mark centre with a "+"
            GControl control = new GControl(GControl.extraBuilt.MarkCenter);

            //Adds a textbox which gives the coordinates of the last click
            GMap1.addControl(new GControl(GControl.extraBuilt.TextualOnClickCoordinatesControl,
                                new GControlPosition(GControlPosition.position.Bottom_Right)));

            if (!IsPostBack)
            {
                //Set centre position of map
                GMap1.setCenter(hwttc, 18);
                //Add InfoWindow greeting
                //GInfoWindow window = new GInfoWindow(hwttc, "<b>Welcome to the HWT Transport Centre</b>");
                //GMap1.addInfoWindow(window);

                //Set up google direction capabalities
                GDirection direction = new GDirection();
                direction.autoGenerate = false;
                direction.buttonElementId = "btnPlot";
                direction.fromElementId = txtStopA.ClientID;
                direction.toElementId = txtStopB.ClientID;
                direction.divElementId = "divDirections"; direction.clearMap = true;

                // Optional
                // direction.locale = "en-En;

                GMap1.Add(direction);
            }

            //Page.RegisterClientScriptBlock("GoogleDistance", "<script language=javascript src='googleDistance.js'>");

        }

        protected void btnHWTTC_Click(object sender, EventArgs e)
        {
            GMap1.setCenter(hwttc, 18);
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            latlong = txtLatLong.Text;
            string[] latlongArr = latlong.Split(',');
            GMap1.setCenter(new GLatLng(Convert.ToDouble(latlongArr[0]), Convert.ToDouble(latlongArr[1])), 18);
            ddlStopList_SelectedIndexChanged(null, null);
        }

        protected void btnAddStop_Click(object sender, EventArgs e)
        {
            //latlong = txtLatLong2.Text;
            //do some validation to see if this is in correct format
            string[] latlongArr = latlong.Split(',');
            //handle exception in case stop exists
            //JUTCLinq.InsertStop(txtStopNo.Text.ToUpper(), Convert.ToDouble(latlongArr[0]), Convert.ToDouble(latlongArr[1]),
                                    //txtLocation.Text, Convert.ToString(rbStopType.SelectedValue));
            
            //fire event to event to show newly added stop on postback
            ddlStopList_SelectedIndexChanged(null, null);
        }

        protected void ddlStopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            double lat = 0.0;
            double longi = 0.0;

            if (ddlStopList.SelectedValue == "All Stops")
            {
                //select all stops create the marker and event
                //to show its info
                var stops = JUTCLinq.SelectAllStops();
                //from s in JUTCLinq.Stops
                //select s;
                
                foreach (var s in stops)
                {
                    lat = s.Lattitude;
                    longi = s.Longitude;
                    GMarker mkr = null;
                    if (s.Type == "BusStop")
                    {
                        mkr = new GMarker(new GLatLng(lat, longi), stopIcon);
                    }
                    else if (s.Type == "Depot")
                    {
                        mkr = new GMarker(new GLatLng(lat, longi), depotIcon);
                    }
                    //GMap1.addGMarker(mkr);  makes marker permanent
                    mManager.Add(mkr,10);
                    GMap1.Add(new GListener(mkr.ID, GListener.Event.click,
                                             "function(){prompt('Stop #: " + s.StopNo
                                             + ",  Location: " + s.Location + ",  Coordinates:"
                                             + "','" + s.Lattitude + "," + s.Longitude
                                             + "');"
                                             + "var hc = document.getElementById('hidCoords');"
                                             + "hc.value = '" + s.Lattitude + "," + s.Longitude + "';"
                                             + "var hsn = document.getElementById('hidStopNo');"
                                             + "hsn.value = '" + s.Lattitude + "," + s.Longitude + "';"
                                             + "var hsl = document.getElementById('hidStopsList');"
                                             + "hsl.value += '" + s.StopNo + ",';"
                                             + "}"));
                }

            }
            else
            {
                //select stoplist for specified route
                var stops = JUTCLinq.SelectStopsByRoute(ddlStopList.SelectedValue);
                //from s in JUTCLinq.Routes
                //where s.RouteNo == ddlStopList.SelectedValue
                //select s;

                string routelist = " ";

                //get routestops
                foreach (var s in stops)
                {
                    routelist = s.RouteStops;
                }

                //parse route stops list
                string[] stopsArr =  routelist.Split(',');

                //travers array and select each stop
                foreach(string stp in stopsArr)
                {
                    var stops2 =
                    from s2 in JUTCLinq.Stops
                    where s2.StopNo == stp
                    select s2;

                    //for each stop select the row and create the marker 
                    //and event to show its info
                    foreach (var s2 in stops2)
                    {
                        lat = s2.Lattitude;
                        longi = s2.Longitude;
                        GMarker mkr = null;
                        if (s2.Type == "BusStop")
                        {
                            mkr = new GMarker(new GLatLng(lat, longi), stopIcon);
                        }
                        else if (s2.Type == "Depot")
                        {
                            mkr = new GMarker(new GLatLng(lat, longi), depotIcon);
                        }
                        mManager.Add(mkr, 10);
                        GMap1.Add(new GListener(mkr.ID, GListener.Event.click,
                                             "function(){prompt('Stop #: " + s2.StopNo
                                             + ",  Location: " + s2.Location + ",  Coordinates:"
                                             + "','" + s2.Lattitude + "," + s2.Longitude
                                             + "');"
                                             + "var hc = document.getElementById('hidCoords');"
                                             + "hc.value = '" + s2.Lattitude + "," + s2.Longitude + "';"
                                             + "var hsn = document.getElementById('hidStopNo');"
                                             + "hsn.value = '" + s2.Lattitude + "," + s2.Longitude + "';"
                                             + "var hsl = document.getElementById('hidStopsList');"
                                             + "hsl.value += '" + s2.StopNo + ",';"
                                             + "}")); 
                    }
                }
            }
            GMap1.markerManager = mManager;

        }

        protected void btnAddRoute_Click(object sender, EventArgs e)
        {
            string stops = txtRouteStops.Text.ToUpper();
            string[] stopsArr = stops.Split(',');
            int stopsInDB = 0;
            string offendingStop = "";

            //check if all stop numbers given exist
            foreach(string stp in stopsArr)
            {
                var stop =
                from s in JUTCLinq.Stops
                where s.StopNo == stp
                select s;

                int sid = stopsInDB;
                foreach (var s in stop)
                {
                    //inccrement to check if something was returned as in s
                    //if not no stop exist with a stop number given
                    stopsInDB++;
                }

                //if stopsInDB was not incremented
                //record offending stopNo
                if(sid == stopsInDB)
                {
                    offendingStop = stp;
                }
                
            }

            string stopDistances = txtStopDistances.Text;
            string[] distArr = stopDistances.Split(',');
            double[] distValueArr = new double[distArr.Length];
            string offendingDistance = "";
            double totalDistance = 0.0;
            
            if (stopsArr.Length == stopsInDB)
            {
                //check that all distances given are valid numbers
                try
                {
                    for (int cnt = 0; cnt < distArr.Length; cnt++)
                    {
                        offendingDistance = distArr[cnt];
                        distValueArr[cnt] = Convert.ToDouble(distArr[cnt]);
                        //throw exception for negative number or 0 value
                        if (distValueArr[cnt] < 1)
                        {
                            throw new Exception("Value less than or equal to zero.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    lblDistancesError.Visible = true;
                    lblDistancesError.Text = "The distance '" + offendingDistance + "' is not valid. "
                                            + ex.ToString();
                }
                
                //check if number of distances = numstops-1
                //if yes sum distances
                if (distValueArr.Length == (stopsArr.Length - 1))
                {
                    totalDistance = distValueArr.Sum();
                }
                else
                {
                    lblDistancesError.Visible = true;
                    lblDistancesError.Text = "There should be 1 less distances than stops: Stops = " + stopsArr.Length
                                            + " Distances = " + distValueArr.Length;
                }
            }
            else
            {
                lblStopsError.Text = "The stop '" + offendingStop + "' does not exist in the database.";
            }

            JUTCLinq.InsertRoute(txtRouteNo.Text, txtRouteStops.Text, txtStopDistances.Text, totalDistance, Convert.ToInt32(txttripTime.Text));
        }


        protected void btnConvertStops_Click(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);
            txtRouteStops.Text = hidStopsList.Value.Trim(','); //collect stops from all markers
            string[] routeStopsArr = txtRouteStops.Text.Split(',');

            foreach (string stp in routeStopsArr)
            {
                var stop =
                    from s in JUTCLinq.Stops
                    where s.StopNo == stp
                    select s; //select information for  each stop in the array

                foreach (var s in stop)
                {
                    string temp = Convert.ToString(s.Lattitude) + ',' + Convert.ToString(s.Longitude) + '|';
                    txtCoordinates.Text += temp; // store all coordinates for stops in the array each set seperated by '|'
                }
            }

            txtCoordinates.Text = txtCoordinates.Text.Trim('|');
            hidCoordsList.Value = txtCoordinates.Text;
            string[] coordsListArr = hidCoordsList.Value.Split('|');

            string[] distList = new string[coordsListArr.Length - 1];

            for (int cnt = 0; cnt < coordsListArr.Length-1; cnt++ )
            {
                string[] singleCoord = coordsListArr[cnt].Split(',');
                double lat = Convert.ToDouble(singleCoord[0]);
                double lng = Convert.ToDouble(singleCoord[1]);
                GLatLng glatlang = new GLatLng(lat, lng);

                string[] singleCoord2 = coordsListArr[cnt+1].Split(',');
                double lat2 = Convert.ToDouble(singleCoord2[0]);
                double lng2 = Convert.ToDouble(singleCoord2[1]);
                GLatLng glatlang2 = new GLatLng(lat2, lng2);

                distList[cnt] = Convert.ToString(glatlang.distanceFrom(glatlang2));
            }

            txtStopDistances.Text = string.Join(",",distList);
        }

        protected void btnDeleteStop_Click(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);

            StopsDataSource.DeleteCommand = "DELETE FROM [Stops] WHERE [StopNo] = " + ddlStopList.SelectedValue;
            StopsDataSource.Delete();
        }

        protected void btnDeleteRoute_Click(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);

            RouteNoDataSource.DeleteCommand = "DELETE FROM [TravelRoutes] WHERE [RouteNo] = " + ddlRouteList.SelectedValue;
            RouteNoDataSource.Delete();
        }

        protected void ddlShowBuses_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);
            double lat = 0.0, longi = 0.0;
            string laststop = "";
            string busNum = "";
            string routeNum = "";

            if (ddlShowBuses.SelectedValue == "Show all buses")
            {
                   var busloc =
                   from bl in JUTCLinq.BusRoutes
                   select bl;
                    
                    //get coords for stop
                    foreach (var bl in busloc)
                    {
                        
                        //Get last stop
                        laststop = bl.LastStop;
                        busNum = bl.BusLicPlate;
                        routeNum = bl.RouteNo;

                        var stopCoords =
                            from sc in JUTCLinq.Stops
                            where sc.StopNo == laststop
                            select sc;

                        //create corresponding markers and click handler
                        foreach(var sc in stopCoords)
                        {
                            lat = sc.Lattitude;
                            longi = sc.Longitude;
                            GMarker mkr = new GMarker(new GLatLng(lat, longi),busIcon);
                            mManager.Add(mkr, 10);
                            GMap1.Add(new GListener(mkr.ID, GListener.Event.click,
                                                 "function(){alert('Bus #: " + busNum
                                                 + ",  Location: " + sc.StopNo + ", " + sc.Location
                                                 + " on route number " + routeNum + "');}"));
                        }
                        
                    }
                    
            }

            if (ddlShowBuses.SelectedValue == "Show buses for selected route")
            {
                   var busloc =
                   from bl in JUTCLinq.BusRoutes
                   where bl.RouteNo == ddlRouteNoList.SelectedValue
                   select bl;
                    
                    
                    foreach (var bl in busloc)
                    {
                        
                        //Get last stop
                        laststop = bl.LastStop;
                        busNum = bl.BusLicPlate;
                        routeNum = bl.RouteNo;

                        var stopCoords =
                            from sc in JUTCLinq.Stops
                            where sc.StopNo == laststop
                            select sc;

                        //create corresponding markers and click handler
                        foreach(var sc in stopCoords)
                        {
                            lat = sc.Lattitude;
                            longi = sc.Longitude;
                            GMarker mkr = new GMarker(new GLatLng(lat, longi), busIcon);
                            mManager.Add(mkr, 10);
                            GMap1.Add(new GListener(mkr.ID, GListener.Event.click,
                                                 "function(){alert('Bus #: " + busNum
                                                 + ",  Location: " + sc.StopNo + ", " + sc.Location
                                                 + " on route number " + routeNum + "');}"));
                        }
                        
                    }
            }
            
        }

        protected void btnAddBus_Click(object sender, EventArgs e)
        {
            double distToFinal = 0.0;
            string[] distances = null;
            string[] stops = null;
            var route =
                from r in JUTCLinq.TravelRoutes
                where r.RouteNo == ddlRouteNoList.SelectedValue
                select r;

                foreach (var r in route)
                {
                    distToFinal = r.TotalDistance;
                    distances = r.RouteDistances.Split(',');
                    stops = r.RouteStops.Split(',');
                }
            double speedMS = (50 * 1000) / 3600;
            int etaNext = Convert.ToInt32((Convert.ToInt32(distances[0]) / speedMS) / 60);
            int etaFinal = Convert.ToInt32((Convert.ToInt32(distToFinal) / speedMS) / 60);

            JUTCLinq.InsertBusRoute(ddlRouteNoList.SelectedValue,ddlBusWaiting.SelectedValue,ddlTabledTime.SelectedValue,
                                     distToFinal,Convert.ToDouble(distances[0]),"50",50,distances.Length,stops[1],stops[0],etaNext,etaFinal);

            var bus =
                from b in JUTCLinq.Buses
                where b.LicensePlate == ddlBusWaiting.SelectedValue
                select b;

            foreach (var b in bus)
            {
                b.BusStatus = "InTransit";
            }

            JUTCLinq.InsertStopLog(ddlRouteNoList.SelectedValue, stops[0], DateTime.Now,
                                    ddlBusWaiting.SelectedValue, ddlTabledTime.SelectedValue);

            JUTCLinq.SubmitChanges();
        }


        /*protected void btnGetDistances_Click(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);
            txtStopDistances.Text = hidDistList.Value;
        }*/
    }
}