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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetAppos(string startD, string currentViewState, JsonElement JSGetAppos)
        {
            var data=JSGetAppos[0];  
            string[] CalendarIds = new string[data.GetArrayLength()];
            string userLogin = "";
            for(int i=0;i< data.GetArrayLength();i++)
            {
                CalendarIds[i] = data[i].ToString();
            }
            //CalendarIds = data[0];
            userLogin = JSGetAppos[1].ToString();
            ISchedulingService ApposInfo = new SchedulingService(_EWSActing);            
            //JsonResult test = ApposInfo.GetAppos(CalendarIds, startD);
            return ApposInfo.GetAppos(CalendarIds, startD, currentViewState,userLogin);
        }
        [HttpPost("PostAppos")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public string PostAppos(JsonElement JSPullAppo)
        {
            ISchedulingService PostAppo = new SchedulingService(_EWSActing);
            
            JsonElement Appo = JSPullAppo[0];            
            string userLogin = JSPullAppo[1].ToString();
            ISchedulingService ApposInfo = new SchedulingService(_EWSActing);
            //return null;
            return PostAppo.PostAppo(JSPullAppo[0], JSPullAppo[1].ToString());
        }
        [HttpGet("DelAppo")]
        public string DelAppo(string id, string userLogin)
        {
            ISchedulingService appo= new SchedulingService(_EWSActing);            
            return appo.DelAppo(id,userLogin);
        }

        [HttpPost("GetCalendars")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetCalendars(JsonElement userData)
        {   
            ISchedulingService Cals = new SchedulingService(_EWSActing);
            AuthService au = new AuthService();
            //au.refreshTokenCheck(userData);
            return new JsonResult(Cals.GetCals(userData));
        }
        [HttpPost("RegUser")]
        public JsonResult RegUser(JsonElement userData)
        {
            AuthService Auth = new AuthService();
            
            return Auth.RegUser(userData);
        }
        [HttpPost("LogUser")] 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public string LogUser(JsonElement userData)
        {
            //AuthService Auth = new AuthService();
            return "congrats";
        }
        [HttpPost("RefToken")]
        public string RefToken(JsonElement userData)
        {
            AuthService Auth = new AuthService();
            if (Auth.RefreshTokenCheck(userData[0].ToString(), userData[1].ToString()) == ""|| !Auth.RegCheck(userData[0].ToString()))
                return "refTokenFail";
            else
            {
                var res = Auth.TokenCreate(userData, 2, 60);               
                dynamic data = res.Value;              
                
                return data.access_token;

                //return res[1];
            }
        }


    }
}