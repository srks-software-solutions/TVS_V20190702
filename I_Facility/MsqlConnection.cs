using System;
//using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace I_Facility
{
    public class MsqlConnection : IDisposable
    {
        
        static String CentralServerName = @"localhost";
        static String username = "root";
        //static String password = "srks4$"; //local
        static String password = "srks4$tvsm"; //server
        static String port = "3306";
        static String DB = "i_facility";

        public MySqlConnection sqlConnection = new MySqlConnection("server = " + CentralServerName + ";userid = " + username + ";Password = " + password + ";database = " + DB + ";port = " + port + ";persist security info=False");

        public void open()
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
        }

        public void close()
        {
            sqlConnection.Close();
        }


        public void Dispose()
        {
            sqlConnection.Dispose();
            GC.SuppressFinalize(this);
        } 

        void IDisposable.Dispose()
        {


        }

    }
}
