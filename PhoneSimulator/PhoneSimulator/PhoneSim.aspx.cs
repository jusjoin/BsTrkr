using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace PhoneSimulator
{
    public partial class _PhoneSim : System.Web.UI.Page
    {
        JUTCDataContext JUTCLinq = new JUTCDataContext();
        MembershipUser user;

        protected void Page_Load(object sender, EventArgs e)
        {
             user = Membership.GetUser();

             var msgs =
                 from m in JUTCLinq.PhoneMessages
                 where m.UserName == user.UserName
                 select m;

             foreach (var m in msgs)
             {
                 txtMessage.Text += "Time of Message: " + m.Time + "\r\n" + m.Message + "\r\n";

                 JUTCLinq.PhoneMessages.DeleteOnSubmit(m);
             }

             try
             {
                 JUTCLinq.SubmitChanges();
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex);
             }
            
        }

        protected void btnGetMsg_Click(object sender, EventArgs e)
        {
            var msgs =
                from m in JUTCLinq.PhoneMessages
                where m.UserName == user.UserName
                select m;

            foreach (var m in msgs)
            {
                txtMessage.Text += "Time of Message: " +  m.Time + "\r\n" +  m.Message + "\r\n";

                JUTCLinq.PhoneMessages.DeleteOnSubmit(m);
            }

            try
            {
                JUTCLinq.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            //call webservice method to parse text message and give info based on this
        }
    }
}
