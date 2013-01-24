﻿using System;
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
    public partial class Default : System.Web.UI.Page
    {

        GLatLng hwttc = new GLatLng(18.012075, -76.798570);
        GLatLng lastclick = new GLatLng();
        JUTCDataContext JUTCLinq = new JUTCDataContext();
        MarkerManager mManager = new MarkerManager();
        string latlong;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Today"] = DateTime.Now.DayOfWeek.ToString();

            //reset listeners, necessary for marker listener to work between postback
            GMap1.resetListeners();


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


        protected void ddlStopList_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                    mManager.Add(mkr, 10);
                    GMap1.Add(new GListener(mkr.ID, GListener.Event.click,
                                             "function(){prompt('Stop #: " + s.StopNo
                                             + ",  Location: " + s.Location + ",  Coordinates:"
                                             + "','" + s.Lattitude + "," + s.Longitude
                                             + "');"
                                             + "var hc = document.getElementById('hidCoords');"
                                             + "hc.value = '" + s.Lattitude + "," + s.Longitude + "';"
                                             + "var hsn = document.getElementById('hidStopNo');"
                                             + "hsn.value = '" + s.StopNo +"';"
                                             + "}"));
                }

            }
            else
            {
                //select stoplist for specified route
                var stops =
                from s in JUTCLinq.TravelRoutes
                where s.RouteNo == ddlStopList.SelectedValue
                select s;

                string routelist = " ";

                //get routestops
                foreach (var s in stops)
                {
                    routelist = s.RouteStops;
                }

                //parse route stops list
                string[] stopsArr = routelist.Split(',');

                //travers array and select each stop
                foreach (string stp in stopsArr)
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
                                             + "hsn.value = '" + s2.StopNo + "';"
                                             + "}"));
                    }
                }
            }
            GMap1.markerManager = mManager;
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
                    foreach (var sc in stopCoords)
                    {
                        lat = sc.Lattitude;
                        longi = sc.Longitude;
                        GMarker mkr = new GMarker(new GLatLng(lat, longi));
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
                    foreach (var sc in stopCoords)
                    {
                        lat = sc.Lattitude;
                        longi = sc.Longitude;
                        GMarker mkr = new GMarker(new GLatLng(lat, longi));
                        mManager.Add(mkr, 10);
                        GMap1.Add(new GListener(mkr.ID, GListener.Event.click,
                                             "function(){alert('Bus #: " + busNum
                                             + ",  Location: " + sc.StopNo + ", " + sc.Location
                                             + " on route number " + routeNum + "');}"));
                    }

                }
            }
        }

    }
}