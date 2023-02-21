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
        [HttpGet(Name = "GetAppos")]
        public JsonResult GetAppos(string startD)
        {
            ISchedulingService ApposInfo = new SchedulingService();
            JsonResult js = ApposInfo.GetApposInfo(startD);
            return ApposInfo.GetApposInfo(startD);
        }
        [HttpPost(Name = "PostAppos")]
        public string PostAppos(JsonResult JSPullAppo)
        {
            IEWSActing EWSAct = new EWSs();
            ExchangeService service = EWSAct.CrEwsService();
            return EWSAct.PostOrEditAppo(service, JSPullAppo);
            /*string fileName = @"..\server\tests\PostAppos\PostAppoIn.json";
            string jsonString = System.IO.File.ReadAllText(fileName);             
            Appo[] app = JsonSerializer.Deserialize<Appo[]>(jsonString);*/


        }


    }
}