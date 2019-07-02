using System;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using I_Facility.Models;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace I_Facility
{
    public class GetMode
    {

        ServerModel.i_facilityEntities1 Serverdb = new ServerModel.i_facilityEntities1();

        //Geting latest Connection value for PC
        public string GetConnection(int MachineID)
        {
            //getting machine ipaddress
            int status = 2;
            string IPAddress = null;

            string Status = "Disconnected";
            int Statustable = 0;

            bool State = false;
            Ping ping = new Ping();
            try
            {
                PingReply pingresult = ping.Send(IPAddress);
                if (pingresult.Status.ToString() == "Success")
                {
                    State = true;
                    Status = "Connected";
                }
            }
            catch { }

            return Status;
        }

        //Geting latest Connection value for server
        public string GetConnectionForServer(int MachineID)
        {
            //getting server ipaddress
            int status = 2;
            string IPAddress = null;

            string Status = "Disconnected";
            int Statustable = 0;

            bool State = false;
            Ping ping = new Ping();
            try
            {
                PingReply pingresult = ping.Send(IPAddress);
                if (pingresult.Status.ToString() == "Success")
                {
                    State = true;
                    Status = "Connected";
                }
            }
            catch { }
            return Status;
        }

        public String GetIPAddressofAndon()
        {
            String IPAddress = null;
            string line;
            string curFile = @"c:\users\oeeuser\desktop\IPAddress.txt";
            // Read the file and display it line by line.
            if (System.IO.File.Exists(curFile))
            {
                System.IO.StreamReader file =
                    new System.IO.StreamReader(@"c:\users\oeeuser\desktop\IPAddress.txt");
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("IP"))
                    {
                        string[] linesplit = line.Split(':');
                        IPAddress = linesplit[1];
                    }
                }

                file.Close();
            }
            return IPAddress;
        }

        public string GetIPAddressofTabSystem()
        {
            //string IP_Address = null;
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            //string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //if (!string.IsNullOrEmpty(ipAddress))
            //{
            //    string[] addresses = ipAddress.Split(',');
            //    if (addresses.Length != 0)
            //    {
            //        IP_Address = addresses[0];
            //    }
            //}
            ////Use this for client IP Address
            //IP_Address = context.Request.ServerVariables["HOST"];
            //string userIpAddress = context.Request.UserHostAddress;

            string ipAdd = "";
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAdd = Convert.ToString(ipHostInfo.AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork));

            return ipAdd;
        }

        //GetCorrectedDate(out correctedDate);
        public void GetCorrectedDate(out String correctedDate)
        {
            correctedDate = null;
            MsqlConnection MC = new MsqlConnection();
            MC.open();
            String GetDayStartQuery = "SELECT StartTime from unitworksccs.unitworkccs.tbldaytiming where IsDeleted = 0 Limit 1";
            MySqlDataAdapter daGDS = new MySqlDataAdapter(GetDayStartQuery, MC.sqlConnection);
            DataTable dtGDS = new DataTable();
            daGDS.Fill(dtGDS);
            MC.close();
            TimeSpan Start = Convert.ToDateTime(dtGDS.Rows[0][0]).TimeOfDay;
            if (Start <= DateTime.Now.TimeOfDay)
            {
                correctedDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                correctedDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
        }


        public void UpdateOperatorHeader(int MacID)
        {
            var MacDet = Serverdb.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.MachineID).FirstOrDefault();

            String ServerConnected = "NOT CONNECTED";
            String MachineConnected = "NOT CONNECTED";
            //Ping ServerPing = new Ping();
            //PingReply ServerReply;
            //String TabIPAddress = GetIPAddressofTabSystem();
            //var MachineDetails = Serverdb.tblmachinedetails.Where(m => m.TabIPAddress == TabIPAddress && m.IsDeleted == 0).FirstOrDefault();
            var MachineDetails = Serverdb.tblmachinedetails.Where(m => m.MachineID == MacDet).FirstOrDefault();
            //try
            //{
            //    ServerReply = ServerPing.Send(MachineDetails.IPAddress, 500);
            //    if (ServerReply.Status == IPStatus.Success)
            //    {
            //        ServerConnected = "CONNECTED";
            //    }
            //}
            //catch (Exception e)
            //{

            //}

            //String MacIP = MacDet.IPAddress;
            //Ping MacPing = new Ping();
            //PingReply MacReply;
            //MacReply = MacPing.Send(MacIP, 500);

            //if (MacReply.Status == IPStatus.Success)
            //{
            //    MachineConnected = "CONNECTED";
            //}
            #region Update tblOperatorHeader 
            var operatorHeader = Serverdb.tbloperatorheaders.Where(m => m.MachineID == MacID).OrderByDescending(m => m.InsertedOn).FirstOrDefault();

            if (operatorHeader != null)
            {
                operatorHeader.TabConnecStatus = MachineConnected;

                operatorHeader.ServerConnecStatus = ServerConnected;
                operatorHeader.ModifiedOn = DateTime.Now;
                operatorHeader.ModifiedBy = 1;// get from session once these screens are integrated....


                Serverdb.Entry(operatorHeader).State = System.Data.Entity.EntityState.Modified;
                Serverdb.SaveChanges();
            }

            #endregion
        }


        //checking dual idle entry
        public bool CheckIdleEntry(int machnid)
        {
            bool tick = false;
            string mode = null;
            int IdleLock = 0;
            var IdleEntry = Serverdb.tbllivemodes.Where(m => m.IsDeleted == 0 && m.MachineID == machnid && m.IsCompleted == 0 && m.StartIdle == 1 && m.ColorCode == "YELLOW").OrderByDescending(m => m.ModeID).FirstOrDefault();
            if (IdleEntry != null)
            {
                mode = IdleEntry.MacMode;
                IdleLock = IdleEntry.StartIdle;
                if (mode == "IDLE" && IdleLock == 1)
                {
                    tick = true;
                }
            }
            //true if last entry is idle
            return tick;
        }

        public class Shift
        {
            public int shiftid { get; set; }
            public string shiftname { get; set; }
        }

        public class Hold
        {
            public int holdid { get; set; }
            public string holdname { get; set; }
        }
    }
}