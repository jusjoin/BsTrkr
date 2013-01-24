using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Subgurim.Controles;
using System.Drawing;
using System.Xml.Linq;



namespace RFIDReaderSimulator
{
    public partial class _Reader : System.Web.UI.Page
    {
        localhost.BusEvents busEvents = new localhost.BusEvents();
        JUTCDataContext JUTCLinq = new JUTCDataContext();
        GLatLng hwttc = new GLatLng(18.012075, -76.798570);
        string tabledTime = "";
        string routeNo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            MarkerManager mManager = new MarkerManager();
            double lat = 0.0;
            double longi = 0.0;


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


            //Add all stops to GMAP
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
                                             + "var hsn = document.getElementById('hidStopNo');"
                                             + "hsn.value = '" + s.StopNo + "';"
                                             + "}"));
            }

            GMap1.markerManager = mManager;

            if (!IsPostBack)
            {
                GMap1.setCenter(hwttc, 18);
            }
        }

        protected void btnSendSignal_Click(object sender, EventArgs e)
        {
            string busNo = ddlBusList.SelectedValue;
            string stopNo = hidStopNo.Value;
            float speed = (float)Convert.ToDouble(txtSpeed.Text);

            var busRout =
                from br in JUTCLinq.BusRoutes
                where br.BusLicPlate == busNo
                select br;

            foreach (var br in busRout)
            {
                tabledTime = br.TabledTime;
                routeNo = br.RouteNo;
            }


            string response = busEvents.SignalStop(busNo, System.DateTime.Now, System.DateTime.Now,
                                    tabledTime, stopNo, routeNo, speed);


            txtMessage.Text += response + "\r\n";
        }

        protected void ddlBusList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var busInfo =
                from bi in JUTCLinq.BusRoutes
                where bi.BusLicPlate == ddlBusList.SelectedValue
                select bi;

            foreach (var bi in busInfo)
            {
                routeNo = bi.RouteNo;
                tabledTime = bi.TabledTime;
            }
        }

        protected void Test_Click(object sender, EventArgs e)
        {
            string response = busEvents.test();
            txtMessage.Text = response += "\r\n";
        }
    }
}

//add show other buses on route at all times showing origin and destination