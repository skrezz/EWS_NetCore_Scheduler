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
            string PCName = "";
            if (Environment.GetEnvironmentVariable("COMPUTERNAME") != null)
            {
                PCName = Environment.GetEnvironmentVariable("COMPUTERNAME");
            }
            string creds = Utils.GetLine(PCName, "Creds");
            
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            string t1 = creds.Remove(creds.IndexOf(" "));
            string t2 = creds.Substring(creds.IndexOf(" "));

            if (creds.Contains(" "))
            service.Credentials = new WebCredentials(creds.Remove(creds.IndexOf(" ")), creds.Substring(creds.IndexOf(" ")+1));
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
            CalendarView cView = new CalendarView(startDate, endDate);
            // Limit the properties returned to the appointment's subject, start time, and end time.
            cView.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);
            // Retrieve a collection of appointments by using the calendar view.
            FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);

            Appo[] ApposArray = new Appo[appointments.Items.Count];

            int i = 0;
            foreach (Appointment a in appointments)
            {
                Appo appo = new Appo();
                appo.title = a.Subject.ToString();
                appo.startDate = a.Start.ToString();
                appo.endDate = a.End.ToString();
                if (a.Sensitivity != null)
                {
                    appo.type = a.Sensitivity.ToString();
                }
                ApposArray[i] = appo;
                i++;
            }

            return new JsonResult(ApposArray);
        }
    }
}
