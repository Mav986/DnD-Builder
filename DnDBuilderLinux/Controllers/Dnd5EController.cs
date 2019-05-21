using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilderLinux.Handlers;
using DnDBuilderLinux.Web;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Controllers
{
    [RoutePrefix("dnd")]
    public class Dnd5EController : ApiController
    {
        private readonly Dnd5EHandler _dndHandler;

        /// <inheritdoc />
        /// <summary>
        ///     Store and retrieve DnD race data
        /// </summary>
        public Dnd5EController()
        {
            RequestHandler reqHandler = new RequestHandler("http://www.dnd5eapi.co/api/", new CacheHandler());
            _dndHandler = new Dnd5EHandler(reqHandler);
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
                return _dndHandler.GetRaces();
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
                return _dndHandler.GetRace(name);
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
                return _dndHandler.GetClasses();
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
                return _dndHandler.GetClass(name);
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Error: " + e.Message));
            }
        }
    }
}