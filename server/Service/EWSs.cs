using EWS_NetCore_Scheduler.Interfaces;
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
        public string[] GetRelatedRecurrenceCalendarItems(ExchangeService service, Appointment calendarItem)
        {
            //Appointment calendarItem = Appointment.Bind(service, itemId, new PropertySet(AppointmentSchema.AppointmentType));
            string[] rrule = new string[2]; 
            Appointment recurrMaster = new Appointment(service);
            PropertySet props = new PropertySet(AppointmentSchema.AppointmentType,
                                                AppointmentSchema.Subject,
                                                AppointmentSchema.FirstOccurrence,
                                                AppointmentSchema.LastOccurrence,
                                                AppointmentSchema.ModifiedOccurrences,
                                                AppointmentSchema.DeletedOccurrences,
                                                AppointmentSchema.Recurrence);
            // If the item ID is not for a recurring master, retrieve the recurring master for the series.
            switch (calendarItem.AppointmentType)
            {
                // Calendar item is a recurring master so use Appointment.Bind
                case AppointmentType.RecurringMaster:
                    recurrMaster = Appointment.Bind(service, calendarItem.Id, props);
                    break;
                // The calendar item is a single instance meeting, so there are no instances to modify or delete.
                case AppointmentType.Single:
                    Console.WriteLine("Item id must reference a calendar item that is part of a recurring series.");
                    return rrule;
                // The calendar item is an occurrence in the series, so use BindToRecurringMaster.
                case AppointmentType.Occurrence:
                    recurrMaster = Appointment.BindToRecurringMaster(service, calendarItem.Id, props);
                    break;
                // The calendar item is an exception to the series, so use BindToRecurringMaster.                
                case AppointmentType.Exception:
                    recurrMaster = Appointment.BindToRecurringMaster(service, calendarItem.Id, props);
                    break;
            }
            Recurrence RecFin=recurrMaster.Recurrence;
            //Working with reccurences require a right pattern. Right now it works only with Weekly Pattern.
            switch (recurrMaster.Recurrence.GetType().ToString())
            {
                case string a when a.Contains("WeeklyPattern"):
                    //Taking collsection of days from reccurence
                    Recurrence.WeeklyPattern wp = (Recurrence.WeeklyPattern)recurrMaster.Recurrence;
                    DayOfTheWeekCollection dotwCol = wp.DaysOfTheWeek;
                    string recDays = "";
                    //forming rrule
                    foreach(DayOfTheWeek dotw in dotwCol)
                    {
                        recDays = recDays + dotw.ToString().ToUpper().Remove(2)+",";
                    }
                    rrule[0] = 
                        "TZID=" + TimeZone.CurrentTimeZone.StandardName + 
                        ":" + RecFin.StartDate.Year + RecFin.StartDate.Month + RecFin.StartDate.Day + 
                        "T" + RecFin.StartDate.TimeOfDay.Hours + RecFin.StartDate.TimeOfDay.Minutes + RecFin.StartDate.TimeOfDay.Seconds; //"FREQ=WEEKLY;UNTIL=19971007T000000Z; WKST=SU;BYDAY=TU,TH"
                    rrule[1] = "FREQ=WEEKLY;UNTIL=" + RecFin.EndDate.Value.Year + RecFin.EndDate.Value.Month + RecFin.EndDate.Value.Day +
                         "T" + RecFin.EndDate.Value.TimeOfDay.Hours + RecFin.EndDate.Value.TimeOfDay.Minutes + RecFin.EndDate.Value.TimeOfDay.Seconds +
                         ";WKST=MO;" +
                         "BYDAY="+ recDays;
                    break;
            } 
            return rrule;
        }

        public string PostOrEditAppo(ExchangeService service, JsonElement JSPullAppo)
        {
            ISchedulingService ssPost = new SchedulingService();
            Appointment[] newAppos = ssPost.PostAppoLogic(service, JSPullAppo);
            foreach (Appointment newAppo in newAppos)
            {
                newAppo.Save(SendInvitationsMode.SendToNone);
                Item item = Item.Bind(service, newAppo.Id, new PropertySet(ItemSchema.Subject));
            }
            return "ok";
        }
    }
}
