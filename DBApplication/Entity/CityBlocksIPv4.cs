using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBApplication.Entity
{
  public class CityBlocksIPv4
    {
        public string ContinentCode { get; set; }
        public string ContinentName { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string TimeZone { get; set; }      
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }      
    }
}
