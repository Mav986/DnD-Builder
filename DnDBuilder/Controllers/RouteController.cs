using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilder.Handlers;
using DnDBuilder.Web;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class RouteController : ApiController
    {
        private readonly CharacterHandler _charHandler;

        /// <inheritdoc />
        /// <summary>
        ///     Store and retrieve DnD race data
        /// </summary>
        public RouteController()
        {
            var reqHandler = new RequestHandler("http://www.dnd5eapi.co/api/", new CacheHandler());
            _charHandler = new CharacterHandler(reqHandler);
        }

        /// <summary>
        ///     GET all races
        /// </summary>
        /// <returns>A JObject containing all DnD 5e races</returns>
        /// <exception cref="HttpResponseException">
        ///     Thrown anytime there is a problem retrieving race data from origin server
        /// </exception>
        [HttpGet]
        [Route("races/all")]
        public JObject GetRaces()
        {
            try
            {
                return _charHandler.GetRaces();
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Error: " + e.Message));
            }
        }

        /// <summary>
        ///     GET a specific race
        /// </summary>
        /// <param name="name">The name of the race</param>
        /// <returns>A JObject containing data for the specified race</returns>
        /// <exception cref="HttpResponseException">
        ///     Thrown if the named race cannot be found
        /// </exception>
        [HttpGet]
        [Route("races/{name}")]
        public JObject GetRace(string name)
        {
            try
            {
                return _charHandler.GetRace(name);
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Error: " + e.Message));
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("classes/all")]
        public JObject GetClasses()
        {
            try
            {
                return _charHandler.GetClasses();
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Error: " + e.Message));
            }
        }

        [HttpGet]
        [Route("classes/{name}")]
        public JObject GetClass(string name)
        {
            try
            {
                return _charHandler.GetClass(name);
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Error: " + e.Message));
            }
        }
    }
}