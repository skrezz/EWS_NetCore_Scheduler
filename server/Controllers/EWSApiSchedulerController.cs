using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.Models;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EWSApiSchedulerController : ControllerBase
    {     
        [HttpGet("GetAppos")]
        public JsonResult GetAppos(string startD)
        {
            ISchedulingService ApposInfo = new SchedulingService();            
            return ApposInfo.GetAppos(startD);
        }
        [HttpPost("PostAppos")]
        public string PostAppos(JsonElement JSPullAppo)
        {
            ISchedulingService PostAppo = new SchedulingService();            
            return PostAppo.PostAppo(JSPullAppo);
        }
        [HttpGet("DelAppo")]
        public string DelAppo(string id)
        {
            ISchedulingService appo= new SchedulingService();            
            return appo.DelAppo(id);
        }


    }
}