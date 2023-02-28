using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Service
{
    public class EWSs:IEWSActing
    {
        public WebCredentials getWebCreds()
        {

            string? ews_user = Environment.GetEnvironmentVariable("EWS_USER");
            string? ews_pwd = Environment.GetEnvironmentVariable("EWS_PWD");

            if (ews_user == null || ews_pwd == null) throw new ArgumentNullException("User or password is not provided");
            return new WebCredentials(ews_user, ews_pwd);

        }
        public ExchangeService CrEwsService()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2016,TimeZoneInfo.Utc);
            service.Credentials = getWebCreds();
            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            return service;
        }

        public JsonResult GetApposInfo(string startD)
        {
            IEWSActing EWSAct = new EWSs();
            ExchangeService service = EWSAct.CrEwsService();
            ISchedulingService GetAppoServ = new SchedulingService();
            Appo[] ApposArray = GetAppoServ.GetApposLogic(service, startD);
            return new JsonResult(ApposArray);

        }
        

        public string PostOrEditAppo(ExchangeService service, JsonElement JSPullAppo)
        {
            ISchedulingService ssPost = new SchedulingService();
            Appointment[] newAppos = ssPost.PostAppoLogic(service, JSPullAppo);
            foreach (Appointment newAppo in newAppos)
            {
                if(newAppo != null)
                try
                {
                    newAppo.Save(SendInvitationsMode.SendToNone);
                    Item item = Item.Bind(service, newAppo.Id, new PropertySet(ItemSchema.Subject));
                }
                catch (System.InvalidOperationException ex)
                {

                }
            }
            return "ok";
        }

        public string DelAppo(string id)
        {
            IEWSActing EWSAct = new EWSs();
            ExchangeService service = EWSAct.CrEwsService();
            Appointment delAppo = Appointment.Bind(service,id,new PropertySet(BasePropertySet.IdOnly));
            delAppo.Delete(DeleteMode.SoftDelete);
            return "deleted_";
        }
    }
}
