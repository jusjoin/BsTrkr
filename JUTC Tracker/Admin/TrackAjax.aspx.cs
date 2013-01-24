using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace JUTC_Tracker.Admin
{
    public partial class TrackAjax : System.Web.UI.Page
    {
        JUTCDataContext JUTCLinq = new JUTCDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            //serverResponse.InnerHtml = "Invalid access to page";

            if (Request.QueryString["query"] != null)
            {
                runQuery(Convert.ToInt32(Request.QueryString["query"]));
            }
        }

        protected void runQuery(int q)
        {
            switch (q)
            {
                case 1:
                    ajaxAddStop();
                    break;

                case 2:
                    ajaxSelectStops();
                    break;
            }
        }

        protected void ajaxAddStop()
        {
            string latlong = Request.QueryString["latlong"];
            string[] latlongArr = latlong.Split(',');
            latlongArr[0] = latlongArr[0].Trim('(');
            latlongArr[1] = latlongArr[1].Trim(' ', ')');
            //handle exception in case stop exists

            try
            {
                JUTCLinq.InsertStop(Request.QueryString["stopNo"].ToUpper(), Convert.ToDouble(latlongArr[0]), Convert.ToDouble(latlongArr[1]),
                                        Request.QueryString["location"], Request.QueryString["type"]);
                serverResponse.InnerHtml = "<font color='red'>";
                serverResponse.InnerHtml += "Stop '" + Request.QueryString["stopNo"] + "' added successfully. <img src='../images/green_tick.png' height='30' width='30'/>";
                serverResponse.InnerHtml += "</font>";
            }
            catch (Exception ex)
            {
                serverResponse.InnerHtml = "<font color='red'>";
                serverResponse.InnerHtml += "An error was encountered. <br />" + ex.GetBaseException().ToString() + "<br />" + ex.Message;
                serverResponse.InnerHtml += "</font>";
            }
            
        }

        protected void ajaxSelectStops()
        {
            if (Request.QueryString["route"].ToString()  == "All Stops")
            {
                string xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\stopsgps.xml";
                XDocument stopsXml = XDocument.Load(xmlFilePath);
                //XmlNodeList xnList = stopsXml.SelectNodes("/stopsgps/stop
                var stops = from item in stopsXml.Descendants("stop")
                            select item.Element("stopNo").Value;
                var latitude = from item in stopsXml.Descendants("stop")
                               select item.Element("latitude").Value;
                var longitude = from item in stopsXml.Descendants("stop")
                                select item.Element("Longitude").Value;
                var location = from item in stopsXml.Descendants("stop")
                               select item.Element("location").Value;
                var type = from item in stopsXml.Descendants("stop")
                           select item.Element("type").Value;


                /*latitude = item.Element("latitude").Value,
                longitude = item.Element("longitude").Value,
                location = item.Element("location").Value,
                type = item.Element("type").Value*/
                int cnt = 0;
                serverResponse.InnerHtml = "";
                foreach (var s in stops)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='stopNo' value='" + s.ToString() + "'/>";
                    cnt++;
                }
                foreach (var la in latitude)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='latitude' value='" + la.ToString() + "'/>";
                }
                foreach (var lo in longitude)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='longitude' value='" + lo.ToString() + "'/>";
                }
                foreach (var loc in location)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='location' value='" + loc.ToString() + "'/>";
                }
                foreach (var t in type)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='type' value='" + t.ToString() + "'/>";
                }
                serverResponse.InnerHtml += "<input type='text' id='total' value='" + cnt + "'/>";
            }
            else
            {
                string xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\routes.xml";
                XDocument routesXml = XDocument.Load(xmlFilePath);
                //XmlNodeList xnList = stopsXml.SelectNodes("/stopsgps/stop
                var routeAttribs = from item in routesXml.Descendants("routes")
                            from r in item.Elements("route")
                            where r.Attribute("routeNo").Value == Request.QueryString["route"]
                            select new
                            {
                                routeNo = r.Attribute("tripTime").Value,
                                totDist = r.Attribute("totalDistance").Value
                            };
                var stops = from item in routesXml.Descendants("routes")
                            from r in item.Elements("route")
                            where r.Attribute("routeNo").Value == Request.QueryString["route"]
                            select item.Element("stop");

                var distances  = from item in routesXml.Descendants("route")
                            select item.Element("distance").Value;
                var tripTime = from item in routesXml.Descendants("route")
                            select item.Element("stop").Value;
                //var totDist = from item in routesXml.Descendants("route")
                  //          select item.Element("stop").Value;

            }
        }
    }
}