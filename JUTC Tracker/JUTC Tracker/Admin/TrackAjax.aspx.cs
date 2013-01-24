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
            string xmlFilePath = "";
            if ((Convert.ToString(Request.QueryString["option"]) == "All Stops"))
            {

                xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\stopsgps.xml";
                XDocument stopsXml = XDocument.Load(xmlFilePath);
                //XmlNodeList xnList = stopsXml.SelectNodes("/stopsgps/stop
                int cnt = 0;
                serverResponse.InnerHtml = "";
                var stops = from item in stopsXml.Descendants("stop")
                            select new
                            {
                                stopNo = item.Element("stopNo").Value,
                                latitude = item.Element("latitude").Value,
                                longitude = item.Element("longitude").Value,
                                location = item.Element("location").Value,
                                type = item.Element("type").Value
                            };
                foreach (var s in stops)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='stopNo' value='" + s.stopNo.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='latitude' value='" + s.latitude.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='longitude' value='" + s.longitude.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='location' value='" + s.location.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='type' value='" + s.type.ToString() + "'/>";
                    cnt++;
                }

                serverResponse.InnerHtml += "<input type='hidden' id='total' name='total' value='" + cnt + "'/>";
            }

            else
            {
                xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\stopsgps.xml";
                XDocument stopsXml = XDocument.Load(xmlFilePath);
                xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\routes.xml";
                XDocument routesXml = XDocument.Load(xmlFilePath);
                //XmlNodeList xnList = stopsXml.SelectNodes("/stopsgps/stop
                int cnt = 0;
                int i = 0;
                serverResponse.InnerHtml = "";
                var routes = from item in routesXml.Descendants("route" + (Convert.ToString(Request.QueryString["option"])))
                            select new
                            {
                                stopNo = item.Element("stop" + i).Element("stopNo").Value,
                                dist = item.Element("dist" + i).Value,
                                numStops = item.Element("numStops").Value,
                                totDist = item.Element("totDist").Value,
                                tripTime = item.Element("tripTime").Value,
                                latitude = item.Element("stop" + i).Element("lattitude").Value,
                                longitude = item.Element("stop" + i).Element("longitude").Value,
                                location = item.Element("stop" + i).Element("location").Value,
                                type = item.Element("stop" + i).Element("type").Value
                            };

                foreach (var r in routes)
                {
                    serverResponse.InnerHtml += "<input type='hidden' name='stopNo' value='" + r.stopNo.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='dist' value='" + r.dist.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='numStops' value='" + r.numStops.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='totDist' value='" + r.totDist.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='tripTime' value='" + r.tripTime.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='latitude' value='" + r.latitude.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='longitude' value='" + r.longitude.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='location' value='" + r.location.ToString() + "'/>";
                    serverResponse.InnerHtml += "<input type='hidden' name='type' value='" + r.type.ToString() + "'/>";
                    cnt++;
                }

                serverResponse.InnerHtml += "<input type='hidden' id='total' name='total' value='" + cnt + "'/>";
            }
            
        }
    }
}