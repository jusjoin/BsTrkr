using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace JUTC_Tracker
{
    /// <summary>
    /// Summary description for GPS_Collector
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class GPS_Collector : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string collect(string licplate, string coords, string timestamp)
        {
            //6/15/2009 1:45:30 PM //standard datetime format
            //write data collected to xml file server reads from this xml file using ajax to show bus locations
            //data can be written to server at regular intervals and can even be compared at the end of the day
            //or purge xml file once it gets too big


            return "Collected successfully";
        }
    }
}
