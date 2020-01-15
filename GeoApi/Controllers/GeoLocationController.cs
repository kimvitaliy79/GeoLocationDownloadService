using DBApplication.Repository;
using GeoApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoApi.Controllers
{
    public class GeoLocationController : ApiController
    {
        GeoRepository geoRepository = new GeoRepository();       

        // POST api/values
        public HttpResponseMessage Post([FromBody]GeoLocation item)
        {           
            var selectedItem= geoRepository.GetItemByNetworkID(item.NetworkIP);

            return Request.CreateResponse(HttpStatusCode.OK, selectedItem);
        }
    }
}
