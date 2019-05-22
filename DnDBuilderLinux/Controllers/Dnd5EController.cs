using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilderLinux.Handlers;
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
            _dndHandler = new Dnd5EHandler();
        }

        /// <summary>
        ///     GET all races
        /// </summary>
        /// <returns>A JObject containing all DnD 5e races</returns>
        /// <exception cref="HttpResponseException">
        ///     If there is a problem retrieving race data from origin server
        /// </exception>
        [HttpGet]
        [Route("races")]
        public JObject GetRaces()
        {
            try
            {
                return _dndHandler.GetRaces();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong. If the problem persists, contact a server administrator"));
            }
        }

        /// <summary>
        ///     GET all classes
        /// </summary>
        /// <returns>A JObject containing all DnD 5e classes</returns>
        /// <exception cref="HttpResponseException">
        ///    If there is a problem retreiving class data from origin server
        /// </exception>
        [HttpGet]
        [Route("classes")]
        public JObject GetClasses()
        {
            try
            {
                return _dndHandler.GetClasses();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong. If the problem persists, contact a server administrator"));
            }
        }
    }
}