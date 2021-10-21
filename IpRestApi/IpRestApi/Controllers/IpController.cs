using IpRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using NetTools;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Text;

namespace IpRestApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class IpController : ControllerBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost("create")]
        public IActionResult CreateIpAddresses([FromForm] IpAddress req)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                IPNetwork ipnetwork = IPNetwork.Parse(req.ip);
                var start = ipnetwork.FirstUsable;
                var end = ipnetwork.LastUsable;
                var range = new IPAddressRange(start, end);

                sb.Append("BEGIN TRANSACTION;");

                foreach (var ip in range)
                {
                    sb.Append($"INSERT INTO IP_Management (ip_address, is_available) VALUES ('{ip}','1');");
                }

                sb.Append("COMMIT;");
            }
            catch (Exception ex)
            {
                var message = $"Error while parsing create IP request. IP and CIDR: {req.ip}";
                logger.Error(ex, message);
                return BadRequest(message);
            }

            SQLiteConnection sqlite_conn;
            sqlite_conn = SQLiteHandler.CreateConnection();

            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = sb.ToString();
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var message = "Error while creating new IP address records";
                logger.Error(ex, message);
                return BadRequest(message);
            }
            finally
            {
                sqlite_conn.Close();
            }

            return Ok();
        }

        [HttpGet("list")]
        public IActionResult ListIpAddresses()
        {
            SQLiteConnection conn;
            conn = SQLiteHandler.CreateConnection();
            List<IpAddress> ips = new List<IpAddress>();

            try
            {
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM IP_Management";

                sqlite_datareader = cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    var ip = sqlite_datareader.GetString(0);
                    var isAvail = sqlite_datareader.GetInt32(1);
                    ips.Add(new IpAddress(ip, ((isAvail == 1 ? true : false))));
                }
            }
            catch (Exception ex)
            {
                var message = "Error while querying new IP address records";
                logger.Error(ex, message);
                return BadRequest(message);
            }
            finally
            {
                conn.Close();
            }

            return Content(JsonConvert.SerializeObject(ips), "application/json");
        }

        [HttpPost("acquire")]
        public IActionResult AcquireIp([FromForm] IpAddress req)
        {
            SQLiteConnection conn;
            conn = SQLiteHandler.CreateConnection();

            try
            {
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = $"UPDATE IP_Management SET is_available = false WHERE ip_address = '{req.ip}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var message = "Error while acquiring ip";
                logger.Error(ex, message);
                return BadRequest(message);
            }
            finally
            {
                conn.Close();
            }

            return Ok();
        }


        [HttpPost("release")]
        public IActionResult ReleaseIp([FromForm] IpAddress req)
        {
            SQLiteConnection conn;
            conn = SQLiteHandler.CreateConnection();

            try
            {
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = $"UPDATE IP_Management SET is_available = true WHERE ip_address = '{req.ip}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var message = "Error while acquiring ip";
                logger.Error(ex, message);
                return BadRequest(message);
            }
            finally
            {
                conn.Close();
            }

            return Ok();
        }
    }
}
