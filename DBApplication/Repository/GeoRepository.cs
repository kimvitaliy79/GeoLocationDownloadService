using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBApplication.Entity;

namespace DBApplication.Repository
{
    public class GeoRepository
    {
        string conString=string.Empty;
        public GeoRepository()
        {          
            if (ConfigurationManager.ConnectionStrings["GeoLocation"] != null)
                conString = ConfigurationManager.ConnectionStrings["GeoLocation"].ToString();
        }

        public CityBlocksIPv4 GetItemByNetworkID(string NetworkID)
        {
            if (string.IsNullOrEmpty(conString))
                throw new Exception("Настройте соединение с базой, ключ GeoLocation в конфиг файле.");

            CityBlocksIPv4 item = new CityBlocksIPv4();

            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand("Stp_GetGeoLocationByID", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NetworkID", NetworkID);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {                       
                        item = new CityBlocksIPv4()
                        {
                            ContinentCode = rdr["continent_code"] != null ? rdr["continent_code"].ToString() : string.Empty,
                            ContinentName = rdr["continent_name"] != null ? rdr["continent_name"].ToString() : string.Empty,
                            CountryName = rdr["country_name"] != null ? rdr["country_name"].ToString() : string.Empty,
                            CityName = rdr["city_name"] != null ? rdr["city_name"].ToString() : string.Empty,
                            TimeZone = rdr["time_zone"] != null ? rdr["time_zone"].ToString() : string.Empty,
                            Latitude = rdr["Latitude"] != null ? Convert.ToDecimal(rdr["Latitude"]) : (decimal?)null,
                            Longitude = rdr["Longitude"] != null ? Convert.ToDecimal(rdr["Longitude"]) : (decimal?)null    
                        };
                    }
                }
            }
            return item;
        }      

        public void DeleteData(string stpName)
        {
            if (string.IsNullOrEmpty(conString))
                throw new Exception("Настройте соединение с базой, ключ GeoLocation в конфиг файле.");

            using (SqlConnection dbConnection = new SqlConnection(conString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(stpName, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        // Copy the DataTable to SQL Server using SqlBulkCopy
        public void InsertDataUsingSQLBulkCopy(DataTable csvFileData, string destTableName)
        {
            if (string.IsNullOrEmpty(conString))
                throw new Exception("Настройте соединение с базой, ключ GeoLocation в конфиг файле.");

            using (SqlConnection dbConnection = new SqlConnection(conString))
            {
                dbConnection.Open();               

                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = destTableName;

                    foreach (var column in csvFileData.Columns)
                    {
                        //string dbColName = DefineColName(column.ToString());
                        // if (!string.IsNullOrEmpty(dbColName))
                        if (column.ToString() == "is_anonymous_proxy" || column.ToString() == "is_satellite_provider")
                            continue;
                        else
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                    }                      

                    s.WriteToServer(csvFileData);
                }
            }
        }

        private string DefineColName(string column)
        {
            string result = string.Empty;
            switch (column)
            {
                case "network":
                    result= "Network";
                    break;
                case "geoname_id":
                    result= "GeonameId";
                    break;
                case "registered_country_geoname_id":
                    result= "RegisteredCountryGeonameId";
                    break;
                case "represented_country_geoname_id":
                    result= "RepresentedCountryGeonameId";
                    break;
                case "postal_code":
                    result= "PostalCode";
                    break;
                case "latitude":
                    result= "Latitude";
                    break;
                case "longitude":
                    result= "Longitude";
                    break;
                case "accuracy_radius":
                    result= "AccuracyRadius";
                    break;

            }
            return result;
        }
    }
}
