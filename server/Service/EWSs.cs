using Microsoft.Exchange.WebServices.Data;

namespace EWS_NetCore_Scheduler.Service
{
    public class EWSs
    {
        public static FindItemsResults<Item> GetAppos(DateTime startDate, DateTime endDate, ExchangeService service)
        {
            return null;
        }
        public static string[] GetRelatedRecurrenceCalendarItems(ExchangeService service, Appointment calendarItem)
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

            //recurrMaster.Recurrence;
            Recurrence RecFin=recurrMaster.Recurrence;
            switch (recurrMaster.Recurrence.GetType().ToString())
            {
                case string a when a.Contains("WeeklyPattern"):
                    Recurrence.WeeklyPattern wp = (Recurrence.WeeklyPattern)recurrMaster.Recurrence;
                    DayOfTheWeekCollection dotwCol = wp.DaysOfTheWeek;
                    string recDays = "";
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
    }
}
