using NLog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace IpRestApi
{
    public class SQLiteHandler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;

            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True;");

            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while creating SQLite connection");
            }
            return sqlite_conn;
        }

        public static void RecreateTable()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();

            try
            {
                SQLiteCommand sqlite_cmd;
                string Dropsql = "DROP TABLE IF EXISTS IP_Management";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = Dropsql;
                sqlite_cmd.ExecuteNonQuery();

                string Createsql = "CREATE TABLE IP_Management (ip_address TEXT NOT NULL, is_available INT NOT NULL CHECK(is_available < 2))";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while recreating the IP table");
            }
            finally
            {
                sqlite_conn.Close();
            }
        }
    }
}
