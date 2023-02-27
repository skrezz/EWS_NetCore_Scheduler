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
            IEWSActing ApposInfo = new EWSs();
            JsonResult js = ApposInfo.GetApposInfo(startD);
            return ApposInfo.GetApposInfo(startD);
        }
        [HttpPost("PostAppos")]
        public string PostAppos(JsonElement JSPullAppo)
        {
            IEWSActing EWSAct = new EWSs();
            ExchangeService service = EWSAct.CrEwsService();
            return EWSAct.PostOrEditAppo(service, JSPullAppo);
        }
        [HttpGet("DelAppo")]
        public string DelAppo(string id)
        {
            IEWSActing appo= new EWSs();            
            return appo.DelAppo(id);
        }


    }
}