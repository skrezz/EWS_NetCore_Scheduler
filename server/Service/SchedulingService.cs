using System.Text.Json;
using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Exchange.WebServices.Data;
using EWS_NetCore_Scheduler.Interfaces;
using System.Text.Json.Nodes;
using System.Security.Cryptography.Xml;

namespace EWS_NetCore_Scheduler.Service
{
    public interface ISchedulingService
    {
        Appo[] GetApposLogic(ExchangeService service, string startD);
        string[] GetRelatedRecurrenceCalendarItems(ExchangeService service, Appointment calendarItem);
        public Appointment[] PostAppoLogic(ExchangeService service, JsonElement JSPullAppo);
    }
    public class SchedulingService: ISchedulingService
    {
        public Appo[] GetApposLogic(ExchangeService service, string startD)
        {
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
                string[] RecStrings = RecStrings = GetRelatedRecurrenceCalendarItems(service, a);
                Appo appo = new Appo();
                appo.title = a.Subject.ToString();
                appo.startDate = a.Start.ToString();
                appo.endDate = a.End.ToString();
                if (a.IsAllDayEvent != null)
                    appo.allDay = a.IsAllDayEvent;
                if (a.Id != null)
                    appo.id = a.Id.ToString();
                if (RecStrings[0] != null)
                {
                    appo.rRule = "RRULE:" + RecStrings[1];
                    appo.DTSTART = "DTSTART;" + RecStrings[0];
                }
                /*if (a.ex != null)
                    appo.exDate = a.Recurrence.ToString();*/
                ApposArray[i] = appo;
                i++;
            }
            return ApposArray;
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
            Recurrence RecFin = recurrMaster.Recurrence;
            //Working with reccurences require a right pattern. Right now it works only with Weekly Pattern.
            switch (recurrMaster.Recurrence.GetType().ToString())
            {
                case string a when a.Contains("WeeklyPattern"):
                    //Taking collsection of days from reccurence
                    Recurrence.WeeklyPattern wp = (Recurrence.WeeklyPattern)recurrMaster.Recurrence;
                    DayOfTheWeekCollection dotwCol = wp.DaysOfTheWeek;
                    string recDays = "";
                    //forming rrule
                    foreach (DayOfTheWeek dotw in dotwCol)
                    {
                        recDays = recDays + dotw.ToString().ToUpper().Remove(2) + ",";
                    }
                    rrule[0] =
                        "TZID=" + TimeZone.CurrentTimeZone.StandardName +
                        ":" + RecFin.StartDate.Year + RecFin.StartDate.Month + RecFin.StartDate.Day +
                        "T" + RecFin.StartDate.TimeOfDay.Hours + RecFin.StartDate.TimeOfDay.Minutes + RecFin.StartDate.TimeOfDay.Seconds; //"FREQ=WEEKLY;UNTIL=19971007T000000Z; WKST=SU;BYDAY=TU,TH"
                    rrule[1] = "FREQ=WEEKLY;UNTIL=" + RecFin.EndDate.Value.Year + RecFin.EndDate.Value.Month + RecFin.EndDate.Value.Day +
                         "T" + RecFin.EndDate.Value.TimeOfDay.Hours + RecFin.EndDate.Value.TimeOfDay.Minutes + RecFin.EndDate.Value.TimeOfDay.Seconds +
                         ";WKST=MO;" +
                         "BYDAY=" + recDays;
                    break;
            }
            return rrule;
        }

        public Appointment[] PostAppoLogic(ExchangeService service, JsonElement JSPullAppo)
        {
            
            var items = JsonNode.Parse(JSPullAppo.ToString());
            JsonObject[] JsObjs =new JsonObject[1];
            Appointment[] newAppos = new Appointment[1];
            if (items.GetType().Name=="JsonArray")
            {
                JsObjs = new JsonObject[items.AsArray().Count];
                int i =0;
                foreach(var item in items.AsArray())
                {
                    JsObjs[i] = item.AsObject();
                    i++;
                }
                newAppos = new Appointment[items.AsArray().Count];
            }
            else
            {
                JsObjs[0] = items.AsObject();
            }
            int j = 0;
            
            foreach(JsonObject jso in JsObjs)
            {
                if (jso["id"] == null)
                {
                    Appointment newAppo = new Appointment(service);
                    newAppo.Subject = jso["title"].ToString();
                    newAppo.Start = DateTime.Parse(jso["startDate"].ToString());
                    newAppos[j] = newAppo;
                    
                }
                else
                {
                    try
                    {
                        Appointment editAppo = Appointment.Bind(service, jso["id"].ToString(), AppoSchemas.AppoPropsSet(1));
                        if (jso["title"] != null) editAppo.Subject = jso["title"].ToString();
                        if (jso["startDate"] != null) editAppo.Start = DateTime.Parse(jso["startDate"].ToString());
                        if (jso["endDate"] != null) editAppo.End = DateTime.Parse(jso["endDate"].ToString());
                        //stealed from internet. What is that?
                        // Unless explicitly specified, the default is to use SendToAllAndSaveCopy.
                        // This can convert an appointment into a meeting. To avoid this,
                        // explicitly set SendToNone on non-meetings.
                        SendInvitationsOrCancellationsMode mode = editAppo.IsMeeting ?
                        SendInvitationsOrCancellationsMode.SendToAllAndSaveCopy : SendInvitationsOrCancellationsMode.SendToNone;
                        // Send the update request to the Exchange server.
                        editAppo.Update(ConflictResolutionMode.AlwaysOverwrite, mode);
                        newAppos[j] = editAppo;
                        
                    }
                    catch (Microsoft.Exchange.WebServices.Data.ServiceResponseException ex)
                    { 
                    }
                }
                j++;
            }  
                /*newAppo.Subject = "Test3";
            newAppo.Start = DateTime.Now;*/
            //newAppo.Save(SendInvitationsMode.SendToNone);
            
            
            return newAppos;            
        }
    }
    public class AppoSchemas
    {
        public static PropertySet AppoPropsSet(int TemplateNumber) 
        { 
            switch (TemplateNumber) 
            {
                case 1:
                    return new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End, AppointmentSchema.Id, AppointmentSchema.IsAllDayEvent,AppointmentSchema.IsMeeting);
                    break;
            }
            return new PropertySet(BasePropertySet.IdOnly);
        }
    }
}
