using Microsoft.Extensions.Options;
using Mugs.Models;
using Mugs.Services.Interfaces;
using Mugs.Services.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mugs.Services
{
    public class MySqlQuerier : IMySqlQuerier
    {
        MySqlQuerierOptions Options { get; }

        public MySqlQuerier(IOptions<MySqlQuerierOptions> options)
        {
            Options = options.Value;
        }

        public async Task<List<Inmate>> GetFiveInmates()
        {
            return await Task.Run(async () =>
            {
                var values = new List<Inmate>();
                string sql = "SELECT * FROM ocala ORDER BY DateOfBooking DESC LIMIT 5";
                using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            Inmate inmate = new Inmate
                            {
                                BookingNumber = Convert.ToUInt32(reader.GetValue(0)),
                                Name = reader.GetString(1),
                                DateOfBooking = reader.GetDateTime(2),
                                County = reader.GetString(3),
                                DateOfBirth = reader.GetDateTime(4),
                                Age = Convert.ToUInt32(reader.GetValue(5)),
                                Gender = reader.GetString(6),
                                Race = reader.GetString(7),
                                Charges = JsonConvert.DeserializeObject<List<Charge>>(reader.GetString(8)),
                                ImageUrl = reader.GetString(9),
                                Display = reader.GetBoolean(10)
                            };
                            values.Add(inmate);
                        }
                        reader.Close();
                        connection.Close();
                        return values;
                    }
                }
            });
        }

        public async Task<bool> InmateExistsAsync(uint bookingNumber)
        {
            return await Task.Run(async () =>
            {
                string sql = "SELECT count(*) FROM ocala WHERE BookingNumber = @bookingNumber";
                using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        var paramBookingNumber = new MySqlParameter("bookingNumber", MySqlDbType.UInt32)
                        {
                            Value = bookingNumber
                        };
                        cmd.Parameters.Add(paramBookingNumber);
                        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        connection.Close();
                        return result > 0;
                    }
                }
            });
        }

        public async Task InsertInmateAsync(Inmate inmate)
        {
            await Task.Run(async () =>
            {
                var charges = JsonConvert.SerializeObject(inmate.Charges);
                string sql = @"INSERT INTO ocala (BookingNumber, Name, DateOfBooking, County, DateOfBirth, Age, Gender, Race, Charges, ImageUrl) 
                                VALUES (@bookingNumber, @name, @dateOfBooking, @county, @dateOfBirth, @age, @gender, @race, @charges, @imageUrl)";
                using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        var paramBookingNumber = new MySqlParameter("bookingNumber", MySqlDbType.UInt32)
                        {
                            Value = inmate.BookingNumber
                        };
                        var paramName = new MySqlParameter("name", MySqlDbType.VarChar)
                        {
                            Value = inmate.Name
                        };
                        var paramDateOfBooking = new MySqlParameter("dateOfBooking", MySqlDbType.DateTime)
                        {
                            Value = inmate.DateOfBooking
                        };
                        var paramCounty = new MySqlParameter("county", MySqlDbType.VarChar)
                        {
                            Value = inmate.County
                        };
                        var paramDateOfBirth = new MySqlParameter("dateOfBirth", MySqlDbType.DateTime)
                        {
                            Value = inmate.DateOfBirth
                        };
                        var paramAge = new MySqlParameter("age", MySqlDbType.UInt32)
                        {
                            Value = inmate.Age
                        };
                        var paramGender = new MySqlParameter("gender", MySqlDbType.VarChar)
                        {
                            Value = inmate.Gender
                        };
                        var paramRace = new MySqlParameter("race", MySqlDbType.VarChar)
                        {
                            Value = inmate.Race
                        };
                        var paramCharges = new MySqlParameter("charges", MySqlDbType.JSON)
                        {
                            Value = charges
                        };
                        var paramImageUrl = new MySqlParameter("imageUrl", MySqlDbType.VarChar)
                        {
                            Value = inmate.ImageUrl
                        };
                        cmd.Parameters.Add(paramBookingNumber);
                        cmd.Parameters.Add(paramName);
                        cmd.Parameters.Add(paramDateOfBooking);
                        cmd.Parameters.Add(paramCounty);
                        cmd.Parameters.Add(paramDateOfBirth);
                        cmd.Parameters.Add(paramAge);
                        cmd.Parameters.Add(paramGender);
                        cmd.Parameters.Add(paramRace);
                        cmd.Parameters.Add(paramCharges);
                        cmd.Parameters.Add(paramImageUrl);
                        await cmd.ExecuteNonQueryAsync();
                        connection.Close();
                    }
                }
            });
        }

        public bool TableExists(string database, string table)
        {
            string sql = "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = @database AND TABLE_NAME = @table";
            using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    var paramDatabase = new MySqlParameter("database", MySqlDbType.VarChar)
                    {
                        Value = database.Normalize()
                    };
                    var paramTable = new MySqlParameter("table", MySqlDbType.VarChar)
                    {
                        Value = table.Normalize()
                    };
                    cmd.Parameters.Add(paramDatabase);
                    cmd.Parameters.Add(paramTable);
                    var result = Convert.ToInt32(cmd.ExecuteScalar());
                    connection.Close();
                    return result == 1;
                }
            }
        }

        public async Task<bool> TableExistsAsync(string database, string table)
        {
            return await Task.Run(async () =>
            {
                string sql = "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = @database AND TABLE_NAME = @table";
                using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        var paramDatabase = new MySqlParameter("database", MySqlDbType.VarChar)
                        {
                            Value = database.Normalize()
                        };
                        var paramTable = new MySqlParameter("table", MySqlDbType.VarChar)
                        {
                            Value = table.Normalize()
                        };
                        cmd.Parameters.Add(paramDatabase);
                        cmd.Parameters.Add(paramTable);
                        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        connection.Close();
                        return result == 1;
                    }
                }
            });
        }

        public void TryCreateTable(string table, Dictionary<string, string> columns)
        {
            string tmp = string.Empty;
            foreach (var column in columns)
            {
                if (column.Key != columns.LastOrDefault().Key)
                    tmp += $"{column.Key} {column.Value}, ";
                else
                    tmp += $"{column.Key} {column.Value}";
            }
            string sql = $"CREATE TABLE IF NOT EXISTS {table} ({tmp})";
            using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns">Key = Column Name, Value = Column Type</param>
        /// <returns></returns>
        public async Task TryCreateTableAsync(string table, Dictionary<string, string> columns)
        {
            string tmp = string.Empty;
            foreach (var column in columns)
            {
                if (column.Key != columns.LastOrDefault().Key)
                    tmp += $"{column.Key} {column.Value}, ";
                else
                    tmp += $"{column.Key} {column.Value}";
            }
            string sql = $"CREATE TABLE IF NOT EXISTS {table} ({tmp})";
            using (MySqlConnection connection = new MySqlConnection(Options.DefaultConnection))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }
        }
    }
}
