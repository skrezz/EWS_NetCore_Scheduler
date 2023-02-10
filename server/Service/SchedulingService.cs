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
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials("skrezz@outlook.com", "Snips123");
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
            cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End, AppointmentSchema.Location);
            // Retrieve a collection of appointments by using the calendar view.
            FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);

            Appo[] ApposArray = new Appo[appointments.Items.Count];

            int i = 0;
            foreach (Appointment a in appointments)
            {
                Appo appo = new Appo();
                appo.Subj = a.Subject.ToString();
                appo.DateS = a.Start.ToString();
                appo.DateE = a.End.ToString();
                if (a.Location != null)
                {
                    appo.Loc = a.Location.ToString();
                }
                ApposArray[i] = appo;
                i++;
            }

            return new JsonResult(ApposArray);
        }
    }
}
