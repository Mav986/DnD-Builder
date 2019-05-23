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
        ///     Get the names and DnD5eapi urls for all races
        /// </summary>
        /// <returns>JSON containing all DnD 5e race names</returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        [Route("races")]
        public JToken GetRaces()
        {
            try
            {
                return _dndHandler.GetAllRaces();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong. If the problem persists, contact a server administrator"));
            }
        }

        /// <summary>
        ///     Get the names and DnD5eapi urls for all races
        /// </summary>
        /// <returns>JSON containing all DnD 5e class names and urls</returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        [Route("classes")]
        public JToken GetClasses()
        {
            try
            {
                return _dndHandler.GetAllClasses();
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