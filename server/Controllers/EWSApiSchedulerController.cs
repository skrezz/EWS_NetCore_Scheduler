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
        public void PostAppos(string startD)
        {

            string fileName = @"..\server\tests\PostAppos\PostAppoIn.json";
            string jsonString = System.IO.File.ReadAllText(fileName);             
            Appo[] app = JsonSerializer.Deserialize<Appo[]>(jsonString);


        }


    }
}