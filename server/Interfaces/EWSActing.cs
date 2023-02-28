using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Interfaces
{
    interface IEWSActing
    {
            WebCredentials getWebCreds();
            ExchangeService CrEwsService();
            JsonResult GetApposInfo(string startD);       
            string PostOrEditAppo(ExchangeService service, JsonElement JSPullAppo);
            string DelAppo(string id);
    }
}
