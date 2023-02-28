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
        public FindItemsResults<Item> appointments(ExchangeService service)
        {
            CalendarFolder calendar = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet());
            ItemView iView = new ItemView(20);
            // Limit the properties returned to the appointment's subject, start time, and end time.
            iView.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);

            // Retrieve a collection of appointments by using the calendar view.
            return service.FindItems(WellKnownFolderName.Calendar, iView);
        }           


        public string DelAppo(Appointment appointment)
        {
            appointment.Delete(DeleteMode.SoftDelete);
            return "deleted_";
        }

        public Appointment EWSAppoBind(ExchangeService service, string id, PropertySet PSet)
        {
            return Appointment.Bind(service, id, PSet);
        }

        public void EWSAppoUpdate(Appointment appo,ConflictResolutionMode conflictResolutionMode, SendInvitationsOrCancellationsMode mode)
        {
            appo.Update(conflictResolutionMode, mode);
        }

        public Appointment EWSBindToRecurringMaster(ExchangeService service, string id, PropertySet props)
        {
            Appointment test= Appointment.BindToRecurringMaster(service, id, props);
            return Appointment.BindToRecurringMaster(service, id, props);
        }
    }
}
