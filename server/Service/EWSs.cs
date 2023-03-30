using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Globalization;
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
        public Appointment[] FindAppointments(ExchangeService service, string[] CalendarIds, string startDate)
        {
            IEWSActing EWS = new EWSs();
            //CalendarFolder calendar = CalendarFolder.Bind(service, CalendarId, new PropertySet());          
            ItemView iView = new ItemView(200);
            SearchFilter searchFilter = new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, DateTime.Parse(startDate));

            //View.date
            // Limit the properties returned to the appointment's subject, start time, and end time.
            iView.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);
            Appointment[] apps = new Appointment[100];
            int i = 0;
            foreach (string CalendarId in CalendarIds)
            {
                FindItemsResults<Item> appointments = service.FindItems(CalendarId, searchFilter, iView);
                //FindItemsResults<Item> appointments = service.FindItems(CalendarId, iView);
                // Retrieve a collection of appointments by using the calendar view. ];                
                foreach (Appointment a in appointments)
                {
                    apps[i] = a;
                    i++;
                }
            }
            int j = 0;
            Appointment[] appsFin= new Appointment[Array.IndexOf(apps, null)];
            while (apps[j] != null)
            {
                appsFin[j] = apps[j];                
                j++;
            }            
            return appsFin;
        }


        public string EWSDelAppo(Appointment appointment)
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

        public Cal[] GetCals()
        {
            
            IEWSActing EWS = new EWSs();
            ExchangeService service = CrEwsService();
            FindFoldersResults ffr = service.FindFolders(WellKnownFolderName.Calendar,new FolderView(1000));
            Cal[] Calendars = new Cal[ffr.Folders.Count + 1];
            for (int i=1;i< Calendars.Length;i++)
            {
                Calendars[i] = new Cal
                {
                    title = ffr.Folders[i-1].DisplayName,
                    CalId = ffr.Folders[i-1].Id.UniqueId
                };                    
            }
            CalendarFolder DefaultCal = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet(FolderSchema.DisplayName));
            Calendars[0] = new Cal
            {
                title = DefaultCal.DisplayName,
                CalId = DefaultCal.Id.UniqueId
            };

            return Calendars;
        }
    }
}
