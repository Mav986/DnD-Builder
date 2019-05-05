using System;
using System.Net.Http;
using System.Web.Http;
using DnDBuilder.Handlers;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class DnDController : ApiController
    {
        private const string BaseUri = "http://www.dnd5eapi.co/api/";
        
        private readonly Race _raceHandler;
        
        /// <summary>
        /// Store and retrieve DnD character data
        /// </summary>
        public DnDController()
        {
            _raceHandler = new Race(BaseUri);
        }
        
        /// <summary>
        /// GET all races
        /// </summary>
        /// <returns>A JObject containing all DnD 5e races</returns>
        [HttpGet]
        [Route("races/all")]
        public JObject Get()
        { 
            return _raceHandler.GetRaces(); ;
        }
    }
}