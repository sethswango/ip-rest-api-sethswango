using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SQLite;
using NLog;
using NLog.Extensions.Logging;

namespace IpRestApi
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

            try
            {
                // Create SQL Connection
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();

                // Recreate IP tracking table
                RecreateTable(sqlite_conn);
                
                // Run Web App
                logger.Debug("Init main");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception during execution of Main method");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

        static SQLiteConnection CreateConnection()
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

        static void RecreateTable(SQLiteConnection conn)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                string Dropsql = "DROP TABLE IF EXISTS IpManagement";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = Dropsql;
                sqlite_cmd.ExecuteNonQuery();

                string Createsql = "CREATE TABLE SampleTable (id INTEGER PRIMARY KEY, ip_address ,)";
                string Createsql1 = "CREATE TABLE SampleTable1 (Col1 VARCHAR(20), Col2 INT)";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while recreating the IP table");
            }
        }
    }
}
