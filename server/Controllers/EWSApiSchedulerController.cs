using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.Models;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        
        [HttpPost("GetAppos")]
        public JsonResult GetAppos(string startD, string currentViewState, string[] CalendarIds, string userLogin)
        {
            ISchedulingService ApposInfo = new SchedulingService(_EWSActing);            
            //JsonResult test = ApposInfo.GetAppos(CalendarIds, startD);
            return ApposInfo.GetAppos(CalendarIds, startD, currentViewState,userLogin);
        }
        [HttpPost("PostAppos")]
        public string PostAppos(JsonElement JSPullAppo, string userLogin)
        {
            ISchedulingService PostAppo = new SchedulingService(_EWSActing);            
            return PostAppo.PostAppo(JSPullAppo,userLogin);
        }
        [HttpGet("DelAppo")]
        public string DelAppo(string id, string userLogin)
        {
            ISchedulingService appo= new SchedulingService(_EWSActing);            
            return appo.DelAppo(id,userLogin);
        }

        [HttpPost("GetCalendars")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetCalendars(JsonElement userLogin)
        {          
            ISchedulingService Cals = new SchedulingService(_EWSActing);
            string test = userLogin.ToString();
            return new JsonResult(Cals.GetCals(userLogin.ToString()));
        }
        [HttpPost("RegUser")]
        public JsonResult RegUser(JsonElement JSPullAppo)
        {
            AuthService Auth = new AuthService();
            return Auth.RegUser(JSPullAppo);
        }
        [HttpPost("LogUser")] 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public string LogUser(JsonElement JSPullAppo)
        {
            //AuthService Auth = new AuthService();
            return "congrats";
        }


    }
}