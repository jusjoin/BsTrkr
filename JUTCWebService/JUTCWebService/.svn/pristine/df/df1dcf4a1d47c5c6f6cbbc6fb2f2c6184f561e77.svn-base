using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace JUTCWebService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BusEvents : System.Web.Services.WebService
    {
        JUTCDataContext JUTCLinq = new JUTCDataContext();

        [WebMethod]
        public string test()
        {
            return "Web Service Operational";
        }

        
        
        [WebMethod]
        public string SignalStop(string busNo, DateTime datetimeOfStop, string tabledTime,
                                    string stopNo, string routeNo, float speed)
        {
            //check if stop is on route and get route data
            bool check = new bool();
            string[] routeStopsArr = null;
            string[] routeDistances = null;
            double totalDistance = 0.0;
            int totStops = 0;
            try
            {
                var route =
                    from r in JUTCLinq.Routes
                    where r.RouteNo == routeNo
                    select r;

                foreach (var r in route)
                {
                    routeStopsArr = r.RouteStops.Split(',');
                    
                    //is stop on route
                    if(routeStopsArr.Contains(stopNo))
                    {
                        totalDistance = r.TotalDistance;
                        routeDistances = r.RouteDistances.Split(',');
                        totStops = r.RouteDistances.Length;
                        check = true;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return "Sending Signal Failed. Reason: " + ex.Message;
            }

            //if stop on route action stop for bus
            if(check == true)
            {
                try{
                JUTCLinq.InsertStopLog(routeNo, stopNo, datetimeOfStop, busNo, tabledTime);
                }
                catch(Exception ex)
                {
                    return "Sending Signal Failed. Reason: " + ex.Message;
                }
                //calculations for ETA and Distances
                //Calculate DistToFinalStop
                double distTravelled = 0.0;
                double distToGo = 0.0;
                double distToNext = 0.0;
                for(int cnt=0; cnt < routeStopsArr.Length - 1; cnt++)
                {
                    
                    if(cnt == 0 && routeStopsArr[cnt] == stopNo || routeStopsArr[cnt] == stopNo)
                    {
                        cnt = routeStopsArr.Length + 1;
                        break;
                    }
                    if(routeStopsArr[cnt+1] != stopNo)
                    {
                        distTravelled += Convert.ToDouble(routeDistances[cnt]);
                        distToNext = Convert.ToDouble(routeDistances[cnt+1]);
                    }
                }
                distToGo = totalDistance - distTravelled;

                //get speed at each stop
                string[] spdAtStops = null;
                double avgSpeed = 0.0;
                int stopsToGo = 0;
                var busroute =
                    from br in JUTCLinq.BusRoutes
                    where br.BusLicPlate == busNo
                    select br;

                foreach(var br in busroute)
                {
                    spdAtStops = br.SpdAtStops.Split(',');
                    avgSpeed = br.AvgSpeed;
                }

                if(spdAtStops.Length == 0)
                {
                    spdAtStops = new string[1];
                    spdAtStops[0] = Convert.ToString(speed);
                }
                else
                {
                    //add new element to speed at stops by reinitializing array
                    string[] temp = spdAtStops;
                    spdAtStops = new string[temp.Length + 1];
                    temp.CopyTo(spdAtStops,0);
                    spdAtStops[temp.Length] = Convert.ToString(speed);
                }
                
                double sum = 0.0;
                foreach(string spd in spdAtStops)
                {
                    sum = Convert.ToDouble(spd);
                }
                avgSpeed = sum/spdAtStops.Length;
                stopsToGo = totStops - spdAtStops.Length;

                //combine speed at stops to one string
                string spdAtStopsString = "";
                
                foreach(string s in spdAtStops)
                {
                    spdAtStopsString += s + ",";
                }
                spdAtStopsString.Remove(spdAtStopsString.Length - 1);
                
                //ETA
                //distances are in metres speed is in km/h
                //double speedMS = (avgSpeed * 1000) / 3600;
                double speedMS = (speed * 1000) / 3600;
                double ETANextStop = ((double)distToNext/(double)speedMS)/60;
                double ETAFinalStop = ((double)distToGo/(double)speedMS)/60;
                //deletebusroute record
                JUTCLinq.DeleteBusRoute(busNo);
                //insertbusroute info
                JUTCLinq.InsertBusRoute(routeNo,busNo,tabledTime,distToGo,distToNext,spdAtStopsString,
                                        avgSpeed,stopsToGo,routeStopsArr[spdAtStops.Length],routeStopsArr[spdAtStops.Length-1],Convert.ToInt32(ETANextStop),Convert.ToInt32(ETAFinalStop));
                
                
                //call function to read passenger notice and send messages
                PassengerNotice(stopNo, routeNo, busNo);
            }
            else
            {
                //Log any stop bus passes but do not add any other data to database
                //JUTCLinq.InsertStopLog(routeNo, stopNo, dateOfStop, timeOfStop, busNo, tabledTime);
                return "Stop NOT on Route, Signal Held";
            }

            return "Stop IS on Route, Signal Sent";
        }

        [WebMethod]
        public string PassengerNotice(string stopNo, string routeNo, string busNo)
        {
            int numUsers = 0;

            try
            {
                var passNotice =
                        from pn in JUTCLinq.PassengerNotices
                        where pn.StopNo == stopNo && pn.RouteNo == routeNo
                        select pn;

                foreach (var pn in passNotice)
                {
                    numUsers++;
                    if (pn.CellNo.Length == 7)
                    {
                        string Message = "Bus number " + busNo + " got to stop " + stopNo
                                            + " at " + DateTime.Now.ToString() + ". Message from JUTC.";

                        JUTCLinq.InsertPhoneMessage(pn.UserName, DateTime.Now, Message);
                    }

                    if (pn.Email != "")
                    {
                        //send email
                    }

                }
            }
            catch (Exception ex)
            {
                return "Error in retrieving notice event information. Reason: " + ex.Message;
            }

            

            return "Notices sent to " + numUsers + ".";
        }
    }
}
