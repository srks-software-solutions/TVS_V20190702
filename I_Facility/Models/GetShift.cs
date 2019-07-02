using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using MySql.Data.MySqlClient;
using I_Facility;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace I_Facility.ServerModel
{
    public class GetShift
    {
        i_facilityEntities1 db = new i_facilityEntities1();

        public bool IsThisPlanInAction(int id)
        {
            bool status = false;
            DataTable dataHolder = new DataTable();

            string CorrectedDate = null;
            tbldaytiming StartTime = db.tbldaytimings.Where(m => m.IsDeleted == 0).SingleOrDefault();
            TimeSpan Start = StartTime.StartTime;
            if (Start <= DateTime.Now.TimeOfDay)
            {
                CorrectedDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                CorrectedDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            I_Facility.MsqlConnection mc = new I_Facility.MsqlConnection();
            mc.open();
            String sql = "SELECT * FROM i_facility.tblshiftplanner WHERE StartDate <='" + CorrectedDate + "' AND EndDate >='" + CorrectedDate + "'AND ShiftPlannerID = " + id + " ORDER BY ShiftPlannerID ASC";

            

            MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
            da.Fill(dataHolder);
            mc.close();

            if (dataHolder.Rows.Count > 0)
            {
                status = true;
            }
            return status;
        }

        private class MsqlConnection
        {
        }
    }
}