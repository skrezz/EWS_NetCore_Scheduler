using System.Text.Json;
using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;


namespace EWS_NetCore_Scheduler.Service
{
    public interface ISchedulingService
    {
        public JsonResult GetApposInfo(string startD);
    }
    public class SchedulingService: ISchedulingService
    {
        public JsonResult GetApposInfo(string startD)
        {
            string creds = "";
            if (Environment.GetEnvironmentVariable(Environment.GetEnvironmentVariable("COMPUTERNAME")) != null)
            {
                creds = Environment.GetEnvironmentVariable(Environment.GetEnvironmentVariable("COMPUTERNAME"));
            }
            //string creds = Utils.GetLine(PCName, "Creds");
            
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            string t1 = creds.Remove(creds.IndexOf(";"));
            string t2 = creds.Substring(creds.IndexOf(";"));

            if (creds.Contains(";"))
            service.Credentials = new WebCredentials(creds.Remove(creds.IndexOf(";")), creds.Substring(creds.IndexOf(";")+1));
            else
                service.Credentials = new WebCredentials(creds.Remove(creds.IndexOf(";")), creds.Substring(creds.IndexOf(";")+1));
            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            //service.AutodiscoverUrl("skrezz@outlook.com", RedirectionUrlValidationCallback);
            DateTime startDate = DateTime.Parse(startD);
            DateTime endDate = startDate.AddDays(1);
            //const int NUM_APPTS = 5;
            // Initialize the calendar folder object with only the folder ID. 
            CalendarFolder calendar = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet());
            // Set the start and end time and number of appointments to retrieve.
            ItemView iView = new ItemView(20);
            // Limit the properties returned to the appointment's subject, start time, and end time.
            iView.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);
            
            // Retrieve a collection of appointments by using the calendar view.
            FindItemsResults<Item> appointments = service.FindItems(WellKnownFolderName.Calendar, iView);

            Appo[] ApposArray = new Appo[appointments.Items.Count];
            
            int i = 0;
            foreach (Appointment a in appointments)
            {
                string[] RecStrings = RecStrings = EWSs.GetRelatedRecurrenceCalendarItems(service, a);
                Appo appo = new Appo();                
                appo.title = a.Subject.ToString();
                appo.startDate = a.Start.ToString();
                appo.endDate = a.End.ToString();
                if (a.IsAllDayEvent != null)                
                    appo.allDay = a.IsAllDayEvent;
                if (a.Id != null)
                    appo.id = a.Id.ToString();
                if (RecStrings[0]!=null)
                {
                    appo.rRule = "RRULE:"+RecStrings[1];
                    appo.DTSTART = "DTSTART;"+RecStrings[0];
                }
                /*if (a.ex != null)
                    appo.exDate = a.Recurrence.ToString();*/
                ApposArray[i] = appo;
                i++;
            }

            return new JsonResult(ApposArray);
        }
    }
}
