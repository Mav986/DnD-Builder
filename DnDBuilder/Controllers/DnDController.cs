using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilder.Handlers;
using DnDBuilder.Web;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class DnDController : ApiController
    {
        private readonly Race _raceHandler;
        
        /// <summary>
        /// Store and retrieve DnD character data
        /// </summary>
        public DnDController()
        {
            _raceHandler = new Race(
                new RequestHandler("http://www.dnd5eapi.co/api/"), 
                new CacheHandler()
                );
        }
        
        /// <summary>
        /// GET all races
        /// </summary>
        /// <returns>A JObject containing all DnD 5e races</returns>
        /// <exception cref="HttpResponseException">
        /// Thrown anytime there is a problem retrieving race data from origin server
        /// </exception>
        [HttpGet]
        [Route("races/all")]
        public JObject Get()
        {
            try
            {
                return _raceHandler.GetRaces();
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Error: " + e.Message));
            }
        }

        /// <summary>
        /// GET a specific race
        /// </summary>
        /// <param name="name">The name of the race</param>
        /// <returns>A JObject containing data for the specified race</returns>
        /// <exception cref="HttpResponseException">
        /// Thrown if the named race cannot be found
        /// </exception>
        [HttpGet]
        [Route("races/{name}")]
        public JObject Get(string name)
        {
            try
            {
                return _raceHandler.GetRace(name);
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound, 
                    "Error: " + e.Message));
            }
        }
        
    }
}