using System;
using System.Data.SqlClient;

namespace I_Facility
{
    public  class MySqlconnectionstring : IDisposable
    {
        //static String ServerName = @"NB0021V10\NB0021SQLEXP2012"; //MCA39
        //static String ServerName = @"DESKTOP-LGCPDTC\NB0022SQLEXP2012"; //MCA27
        //static String ServerName = @"NB0018V10\NB0018SQLEXP2012"; //MCA21
        //static String ServerName = @"NB0020V10\NB0020SQLEXP2012"; //MCA20
        //static String ServerName = @"NB0016V10\NB0016SQLEXP2012"; //MCA19
        //static String ServerName = @"NB0019V10\NB0019SQLEXP2012"; //MCA18
        //static String ServerName = @"NB072V10\NB072SQLEXP2012"; //MCA01
        //static String ServerName = @"NB073V10\NB073SQLEXP2012"; //MCA02
        //static String ServerName = @"NB074V10\NB074SQLEXP2012"; //MCA03
        //static String ServerName = @"NB075V10\NB075SQLEXP2012"; //MCA04
        //static String ServerName = @"NB076V10\NB076SQLEXP2012"; //MCA05
        //static String ServerName = @"NB077V10\NB077SQLEXP2012"; //MCA06
        //static String ServerName = @"NB078V10\NB078SQLEXP2012"; //MCA08
        //static String ServerName = @"NB079V10\NB079SQLEXP2012"; //MCA09
        //static String ServerName = @"NB081V10\NB081SQLEXP2012"; //MCA24
        //static String ServerName = @"NB082V10\NB082SQLEXP2012"; //MCA29
        //static String ServerName = @"NB083V10\NB083SQLEXP2012"; //MCA30
        //static String ServerName = @"NB084V10\NB084SQLEXP2012"; //MCA40
        //static String ServerName = @"NB065V10\NB065SQLEXP2012"; //MCA11
        //static String ServerName = @"NB071V10\NB071SQLEXP2012"; //MCA10
        //static String ServerName = @"NB066V10\NB066SQLEXP2012"; //MCA12
        //static String ServerName = @"NB067V10\NB067SQLEXP2012"; //MCA13
        //static String ServerName = @"NB068V10\NB068SQLEXP2012"; //MCA14
        //static String ServerName = @"NB069V10\NB069SQLEXP2012"; //MCA15
        //static String ServerName = @"NB080V10\NB080SQLEXP2012"; //MCA16
        //static String ServerName = @"NB070V10\NB070SQLEXP2012"; //MCA17
        //static String ServerName = @"NB050V10\NB050SQLEXP2012"; //MCA22
        //static String ServerName = @"NB051V10\NB051SQLEXP2012"; //MCA23
        //static String ServerName = @"NB052V10\NB052SQLEXP2012"; //MCA25
        //static String ServerName = @"NB053V10\NB053SQLEXP2012"; //MCA26
        //static String ServerName = @"NB054V10\NB054SQLEXP2012"; //MCA27
        //static String ServerName = @"NB055V10\NB055SQLEXP2012"; //MCA28
        //static String ServerName = @"NB062V10\NB062SQLEXP2012"; //MCA37
        //static String ServerName = @"NB061V10\NB061SQLEXP2012"; //MCA36
        //static String ServerName = @"NB064V10\NB064SQLEXP2012"; //MCA39
        //static String ServerName = @"NB056V10\NB056SQLEXP2012"; //MCA31
        //static String ServerName = @"NB057V10\NB057SQLEXP2012"; //MCA32
        //static String ServerName = @"NB058V10\NB058SQLEXP2012"; //MCA33
        //static String ServerName = @"NB059V10\NB059SQLEXP2012"; //MCA34
        //static String ServerName = @"NB060V10\NB060SQLEXP2012"; //MCA35
        //static String ServerName = @"NB063V10\NB063SQLEXP2012"; //MCA38
        //static String ServerName = @"SRKSDEV007-PC\MSSQLSERVER12"; //"localhost";
        //static String ServerName = @"SRKSDEV007-PC\MSSQLSERVER12,1234"; //"mine";
        static String CentralServerName = @"localhost";
        //static String CentralServerName = @"TCP:SRKS_TECH-1,19040";
        //static String CentralServerName = @"SRKSDEV007-PC\SQLEXPRESS"; //"mine 16";
        //static String CentralServerName = @"tcp:PAVANKUMARV013\CCSSQLEXP2012013,1234";
        static String username = "root";
        static String password = "srks4$";
        static String port = "3306";
        static String DB = "i_facility";

        //public MySqlConnection msqlConnection = new MySqlConnection("server = " + ServerName + ";userid = " + username + ";Password = " + password + ";database = " + DB + ";port = " + port + ";persist security info=False");

        //public SqlConnection sqlConnection = new SqlConnection(@"Data Source = " + ServerName + ";User ID = " + username + ";Password = " + password + ";Initial Catalog = " + DB + ";Persist Security Info=True");
        public SqlConnection sqlConnection = new SqlConnection(@"Data Source = " + CentralServerName + ";User ID = " + username + ";Password = " + password + ";Initial Catalog = " + DB + ";Persist Security Info=True");

        public void open()
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
        }

        public void close()
        {
            sqlConnection.Close();
        }

        //public void open()
        //{
        //    if (sqlConnection.State != System.Data.ConnectionState.Open)
        //        sqlConnection.Open();
        //}

        //public void close()
        //{
        //    sqlConnection.Close();
        //}

        void IDisposable.Dispose()
        { }

    }
}