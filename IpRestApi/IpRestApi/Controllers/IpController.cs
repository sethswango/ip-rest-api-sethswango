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
            try
            {
                IPNetwork ipnetwork = IPNetwork.Parse(req.ip);
                var start = ipnetwork.FirstUsable;
                var end = ipnetwork.LastUsable;
                var range = new IPAddressRange(start, end);

                using (var connection = SQLiteHandler.CreateConnection())
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        INSERT INTO IP_Management (ip_address, is_available) VALUES ($value, 1);
                    ";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "$value";
                    command.Parameters.Add(parameter);

                    foreach (var ip in range)
                    {
                        parameter.Value = ip;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }             
            }
            catch (Exception ex)
            {
                var message = $"Error handling create IP request. IP and CIDR: {req.ip} Error: {ex}";
                logger.Error(ex, message);
                return BadRequest(message);
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
