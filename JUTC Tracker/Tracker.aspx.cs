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
        
        GLatLng hwttc = new GLatLng(18.012075, -76.798570);
        GLatLng lastclick = new GLatLng();
        JUTCDataContext JUTCLinq = new JUTCDataContext();
        string latlong;

        protected void Page_Load(object sender, EventArgs e)
        {
            //reset listeners, necessary for marker listener to work between postback
            GMap1.resetListeners();

            //hide objects to be hidden
            lblDistancesError.Visible = false;
            lblStopsError.Visible = false;

            //Create default marker
            GIcon icon = new GIcon();
            icon.image = "http://labs.google.com/ridefinder/images/mm_20_red.png";
            icon.shadow = "http://labs.google.com/ridefinder/images/mm_20_shadow.png";
            icon.iconSize = new GSize(20, 28);
            icon.shadowSize = new GSize(30, 28);
            icon.iconAnchor = new GPoint(10, 18);
            icon.infoWindowAnchor = new GPoint(5, 1);
            
            GMap1.mapType = GMapType.GTypes.Hybrid;

            GMarkerOptions mOpts = new GMarkerOptions();
            mOpts.clickable = false;
            mOpts.icon = icon;

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
            latlong = txtLatLong2.Text;
            //do some validation to see if this is in correct format
            string[] latlongArr = latlong.Split(',');
            //handle exception in case stop exists
            JUTCLinq.InsertStop(txtStopNo.Text.ToUpper(), Convert.ToDouble(latlongArr[0]), Convert.ToDouble(latlongArr[1]), txtLocation.Text);
            
            //fire event to event to show newly added stop on postback
            ddlStopList_SelectedIndexChanged(null, null);
        }

        protected void ddlStopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MarkerManager mManager = new MarkerManager();
            double lat = 0.0;
            double longi = 0.0;

            if (ddlStopList.SelectedValue == "All Stops")
            {
                //select all stops create the marker and event
                //to show its info
                var stops =
                from s in JUTCLinq.Stops
                select s;
                
                foreach (var s in stops)
                {
                    lat = s.Lattitude;
                    longi = s.Longitude;
                    GMarker mkr = new GMarker(new GLatLng(lat, longi));
                    //GMap1.addGMarker(mkr); 
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
                var stops =
                from s in JUTCLinq.Routes
                where s.RouteNo == ddlStopList.SelectedValue
                select s;

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
                        GMarker mkr = new GMarker(new GLatLng(lat, longi));
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

            JUTCLinq.InsertRoute(txtRouteNo.Text, txtRouteStops.Text, txtStopDistances.Text, totalDistance);
        }


        protected void btnConvertStops_Click(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);
            txtRouteStops.Text = hidStopsList.Value.Trim(',');
            string[] routeStopsArr = txtRouteStops.Text.Split(',');

            foreach (string stp in routeStopsArr)
            {
                var stop =
                    from s in JUTCLinq.Stops
                    where s.StopNo == stp
                    select s;

                foreach (var s in stop)
                {
                    string temp = Convert.ToString(s.Lattitude) + ',' + Convert.ToString(s.Longitude) + '|';
                    txtCoordinates.Text += temp;
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

            RouteNoDataSource.DeleteCommand = "DELETE FROM [Route] WHERE [RouteNo] = " + ddlRouteList.SelectedValue;
            RouteNoDataSource.Delete();
        }

        protected void ddlShowBuses_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);

            if (ddlShowBuses.SelectedValue == "Show all buses")
            {
                var buses =
                from b in JUTCLinq.BusRoutes
                select b;

                foreach (var b in buses)
                {
                    //create corresponding markers and click handler
                }
            }

            if (ddlShowBuses.SelectedValue == "Show buses for selected route")
            {
                var buses =
                from b in JUTCLinq.BusRoutes
                where b.RouteNo == ddlRouteNoList.SelectedValue
                select b;

                foreach (var b in buses)
                {
                    //create corresponding markers and click handler
                }
            }
            
        }


        /*protected void btnGetDistances_Click(object sender, EventArgs e)
        {
            ddlStopList_SelectedIndexChanged(null, null);
            txtStopDistances.Text = hidDistList.Value;
        }*/
    }
}