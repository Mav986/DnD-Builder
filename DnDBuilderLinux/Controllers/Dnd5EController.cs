using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilderLinux.Handlers;
using DnDBuilderLinux.Exceptions;
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
        /// <returns>
        ///     200 OK if races were retrieved
        ///     500 Internal Server Error otherwise
        /// </returns>
        [HttpGet]
        [Route("races")]
        public HttpResponseMessage GetRaces()
        {
            try
            {
                JToken allRaces = _dndHandler.GetAllRaces();
                return Request.CreateResponse(HttpStatusCode.OK, allRaces);
            }
            catch (DndException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message + " If the problem persists, contact a server administrator");
            }
        }

        /// <summary>
        ///     Get the names and DnD5eapi urls for all races
        /// </summary>
        /// <returns>
        ///    200 OK if all classes were retrieved
        ///    500 Internal Server Error otherwise
        /// </returns>
        [HttpGet]
        [Route("classes")]
        public HttpResponseMessage GetClasses()
        {
            try
            {
                JToken allClasses = _dndHandler.GetAllClasses();
                return Request.CreateResponse(HttpStatusCode.OK, allClasses);
            }
            catch (DndException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message + " If the problem persists, contact a server administrator");
            }
        }

        /// <summary>
        ///     get a boolean value indicating whether the class is a caster or not
        /// </summary>
        /// <param name="classType">A valid DND 5E class</param>
        /// <returns>
        ///    200 OK if spellcaster was calculated
        ///    400 Bad Request otherwise
        /// </returns>
        [HttpGet]
        [Route("spellcaster/{classType}")]
        public HttpResponseMessage GetSpellcaster(string classType)
        {
            try
            {
                bool isCaster = _dndHandler.IsCaster(classType);
                return Request.CreateResponse(HttpStatusCode.OK, isCaster);
            }
            catch (DndException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    e.Message + " If the problem persists, contact a server administrator");
            }
        }
    }
}