using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.Models;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EWSApiSchedulerController : ControllerBase
    {
        private readonly IEWSActing _EWSActing = new EWSs();
        
        [HttpGet("GetAppos")]
        public JsonResult GetAppos(string CalendarId, string startD)
        {
            ISchedulingService ApposInfo = new SchedulingService(_EWSActing);
            JsonResult test = ApposInfo.GetAppos(CalendarId, startD);
            return ApposInfo.GetAppos(CalendarId, startD);
        }
        [HttpPost("PostAppos")]
        public string PostAppos(JsonElement JSPullAppo)
        {
            ISchedulingService PostAppo = new SchedulingService(_EWSActing);            
            return PostAppo.PostAppo(JSPullAppo);
        }
        [HttpGet("DelAppo")]
        public string DelAppo(string id)
        {
            ISchedulingService appo= new SchedulingService(_EWSActing);            
            return appo.DelAppo(id);
        }

        [HttpGet("GetCalendars")]
        public JsonResult GetCalendars()
        {          
            ISchedulingService Cals = new SchedulingService(_EWSActing);
            return new JsonResult(Cals.GetCals());
        }


    }
}