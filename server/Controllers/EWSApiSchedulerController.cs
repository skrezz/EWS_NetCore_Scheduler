using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.Models;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EWSApiSchedulerController : ControllerBase
    {
        private readonly IEWSActing _EWSActing = new EWSs();
        private readonly IHttpContextAccessor httpC;
        public EWSApiSchedulerController(IHttpContextAccessor _httpC)
        {
            httpC = _httpC;
        }

        [HttpPost("GetAppos")]
        public JsonResult GetAppos(string startD, string currentViewState, string[] CalendarIds)
        {
            ISchedulingService ApposInfo = new SchedulingService(_EWSActing);
            //JsonResult test = ApposInfo.GetAppos(CalendarIds, startD);
            return ApposInfo.GetAppos(CalendarIds, startD, currentViewState);
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
            ISchedulingService appo = new SchedulingService(_EWSActing);
            return appo.DelAppo(id);
        }

        [HttpGet("GetCalendars")]
        public JsonResult GetCalendars()
        {
            var test3 = httpC.HttpContext;
            var test2 = httpC.HttpContext.User.Identity.Name;
            ISchedulingService Cals = new SchedulingService(_EWSActing);
            JsonResult test = new JsonResult(Cals.GetCals());
            return new JsonResult(Cals.GetCals());
        }
    }

}