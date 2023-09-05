using Castle.Components.DictionaryAdapter.Xml;
using EWS_NetCore_Scheduler.data;
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
        public WebCredentials checkWebCreds(ExchangeService service)
        {
            
            //проверка подлинности кредов
            
            //string? ews_user = Environment.GetEnvironmentVariable("EWS_USER");
            //string? ews_pwd = Environment.GetEnvironmentVariable("EWS_PWD");

            //if (uLog == null || uPass == null) throw new ArgumentNullException("User or password is not provided");
       
            return new WebCredentials();

        }
        public ExchangeService CrEwsService(string uLog,ExchangeCredentials exCreds)
        {
            AuthService au = new AuthService();
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2016,TimeZoneInfo.Utc);
            service.Credentials = exCreds;
            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            //проверка подлинности кредов
            
            try
            {
                var checkCreds = service.FindFolders(WellKnownFolderName.Calendar, new FolderView(1000));
            }
            catch (Microsoft.Exchange.WebServices.Data.ServiceRequestException Ex)
            {
                return null;
            }        
            if (Globs.ExCre == null)
                    Globs.ExCre = new Dictionary<string, ExchangeCredentials>();
            if (!au.RegCheck(uLog))
                Globs.ExCre.Add(uLog, service.Credentials);
            else
                Globs.ExCre[uLog] = service.Credentials;
            return service;
        }
        public Apps[] FindAppointments(ExchangeService service, string[] CalendarIds, string startDate,string endDate)
        {
            IEWSActing EWS = new EWSs();
            //CalendarFolder calendar = CalendarFolder.Bind(service, CalendarId, new PropertySet());          
            ItemView iView = new ItemView(200);
            SearchFilter greaterthanfilter = new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, DateTime.Parse(startDate));
            SearchFilter lessthanfilter = new SearchFilter.IsLessThanOrEqualTo(AppointmentSchema.End, DateTime.Parse(endDate));
            SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, greaterthanfilter, lessthanfilter);
            //SearchFilter searchFilter = new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, DateTime.Parse(startDate));

            //View.date
            // Limit the properties returned to the appointment's subject, start time, and end time.
            iView.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);
            Apps[] apps = new Apps[100];
            int i = 0;
            foreach (string CalendarId in CalendarIds)
            {
                FindItemsResults<Item> appointments = service.FindItems(CalendarId, searchFilter, iView);
                //FindItemsResults<Item> appointments = service.FindItems(CalendarId, iView);
                // Retrieve a collection of appointments by using the calendar view. ];                
                foreach (Appointment a in appointments)
                {
                    a.Culture = "ru-RU";
                    a.StartTimeZone = TimeZoneInfo.Local;   
                    a.EndTimeZone = TimeZoneInfo.Local;   
                    apps[i] = new Apps{
                        App=a,
                        CalId= CalendarId
                    };
                    i++;
                }
            }
            int j = 0;
            Apps[] appsFin= new Apps[Array.IndexOf(apps, null)];
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

        public Cal[] GetCals(string userLogin)
        {
            
            IEWSActing EWS = new EWSs();
            IAuthService Auth = new AuthService();

            ExchangeService service = CrEwsService(userLogin,Auth.getService(userLogin));
            if (service == null) return null;
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
