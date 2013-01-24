using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using System.ComponentModel;
using System.Xml;

namespace JUTC_Tracker
{
    public class Global : System.Web.HttpApplication
    {
        protected static JUTCDataContext JUTCLinq = new JUTCDataContext();

        protected void Application_Start(object sender, EventArgs e)
        {
            loadStopsXFile();
            loadRoutesXFile();
            //BackgroundWorker Code
            
            // Code that runs on application startup
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted +=
                   new RunWorkerCompletedEventHandler(WorkerCompleted);

            // Calling the DoWork Method Asynchronously
            worker.RunWorkerAsync(); //we can also pass parameters to the async method....

            //BackgroundWorker Code
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Background worker code that runs on application shutdown
            //If background worker process is running then clean up that object.

            //Background worker end

        }

        private static void DoWork(object sender, DoWorkEventArgs e)
        {

            //Background code


        }

        private static void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /*BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                // sleep for 20 secs and again call DoWork
                System.Threading.Thread.Sleep(20000);
                worker.RunWorkerAsync();
            }*/
        }

        protected static void loadStopsXFile()
        {
            string xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\stopsgps.xml";
            XmlDocument stopsXml = new XmlDocument();
            stopsXml.Load(xmlFilePath);
            XmlNode root = stopsXml.DocumentElement;

            //Remove all attribute and child nodes.
            root.RemoveAll();
            stopsXml.Save(xmlFilePath);

            XmlNode stopsgps = stopsXml.SelectSingleNode("stopsgps");
            var stops = JUTCLinq.SelectAllStops();

            foreach (var s in stops)
            {
                XmlNode newstop = stopsXml.CreateNode(XmlNodeType.Element, "stop", null);
                //XmlAttribute stopNo = stopsXml.CreateAttribute("StopNo");
                XmlNode stopNo = stopsXml.CreateNode(XmlNodeType.Element, "stopNo", null);
                stopNo.InnerText = s.StopNo.ToString();
                newstop.AppendChild(stopNo);

                //XmlAttribute lattitude = stopsXml.CreateAttribute("lattitude");
                //lattitude.Value = s.lattitude.ToString();
                XmlNode lattitude = stopsXml.CreateNode(XmlNodeType.Element, "latitude", null);
                lattitude.InnerText = s.Lattitude.ToString();
                newstop.AppendChild(lattitude);

                //XmlAttribute longitude = stopsXml.CreateAttribute("Longitude");
                //longitude.Value = s.Longitude.ToString();
                XmlNode longitude = stopsXml.CreateNode(XmlNodeType.Element, "Longitude", null);
                longitude.InnerText = s.Longitude.ToString();
                newstop.AppendChild(longitude);

                //XmlAttribute location = stopsXml.CreateAttribute("Location");
                //location.Value = s.Location.ToString();
                XmlNode location = stopsXml.CreateNode(XmlNodeType.Element, "location", null);
                location.InnerText = s.Location.ToString();
                newstop.AppendChild(location);

                //XmlAttribute type = stopsXml.CreateAttribute("Type");
                //type.Value = s.Type.ToString();
                XmlNode type = stopsXml.CreateNode(XmlNodeType.Element, "type", null);
                type.InnerText = s.Type.ToString();
                newstop.AppendChild(type);

                /*newstop.Attributes.Append(stopNo);
                newstop.Attributes.Append(lattitude);
                 * 
                new;stop.Attributes.Append(longitude);
                newstop.Attributes.Append(location);
                newstop.Attributes.Append(type);*/

                //System.Diagnostics.Debug.Write(stopsXml.OuterXml.ToString());
                //System.Diagnostics.Debug.Write(Environment.NewLine);
                //System.Diagnostics.Debug.Write(newstop.OuterXml.ToString());
                //System.Diagnostics.Debug.Write(Environment.NewLine);
                stopsgps.AppendChild(newstop);
                //System.Diagnostics.Debug.Write(stopsgps.OuterXml.ToString());
                //System.Diagnostics.Debug.Write(Environment.NewLine);
                //stopsXml.AppendChild(newstop);
                stopsXml.GetElementsByTagName("stopsgps")[0].InsertAfter(newstop,
                    stopsXml.GetElementsByTagName("stopsgps")[0].LastChild);
            }
            stopsXml.Save(xmlFilePath);
        }

        protected static void loadRoutesXFile()
        {
            string xmlFilePath = HttpContext.Current.Server.MapPath("~/App_Data") + "\\routes.xml";
            XmlDocument routesXml = new XmlDocument();
            routesXml.Load(xmlFilePath);
            XmlNode root = routesXml.DocumentElement;

            //Remove all attribute and child nodes.
            root.RemoveAll();
            routesXml.Save(xmlFilePath);

            XmlNode routes = routesXml.SelectSingleNode("routes");
            var route = JUTCLinq.SelectAllRoutes();

            foreach (var r in route)
            {
                XmlNode newRoute = routesXml.CreateNode(XmlNodeType.Element, "route", null);
                //XmlAttribute stopNo = stopsXml.CreateAttribute("StopNo");
                XmlNode routeNo = routesXml.CreateNode(XmlNodeType.Attribute, "routeNo", null);
                routeNo.Value = r.RouteNo.ToString();
                newRoute.Attributes.SetNamedItem(routeNo);
                
                XmlNode totDist = routesXml.CreateNode(XmlNodeType.Attribute, "totalDistance", null);
                totDist.Value = r.TotalDistance.ToString();
                newRoute.Attributes.SetNamedItem(totDist);
                
                XmlNode tripTime = routesXml.CreateNode(XmlNodeType.Attribute, "tripTime", null);
                tripTime.Value = r.TripTime.ToString();
                newRoute.Attributes.SetNamedItem(tripTime);

                string stops = r.RouteStops.ToString();
                string[] stopsArr = stops.Split(',');
                string distance = r.RouteDistances.ToString();
                string[] distancesArr = distance.Split(',');

                for (int cnt = 0; cnt < stopsArr.Length; cnt++)
                {
                    int digit = cnt + 1;
                    XmlNode newStop = routesXml.CreateNode(XmlNodeType.Element, "stop"+ digit, null);
                    newStop.InnerText = stopsArr[cnt];
                    newRoute.AppendChild(newStop);

                    XmlNode newDist = routesXml.CreateNode(XmlNodeType.Element, "distance" + digit, null);
                    newDist.InnerText = distancesArr[cnt];
                    newRoute.AppendChild(newDist);
                }
            
                routesXml.GetElementsByTagName("routes")[0].InsertAfter(newRoute,
                    routesXml.GetElementsByTagName("routes")[0].LastChild);
            }
            routesXml.Save(xmlFilePath);
        }

    }
}