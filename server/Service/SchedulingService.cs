﻿using System.Text.Json;
using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Exchange.WebServices.Data;
using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.data;
using System.Text.Json.Nodes;
using System.Security.Cryptography.Xml;
using System.Globalization;
using System.Runtime.Intrinsics.X86;

namespace EWS_NetCore_Scheduler.Service
{

    public class SchedulingService: ISchedulingService
    {
        private readonly IEWSActing _EWSActing;
        public SchedulingService(IEWSActing EWS)
        {
            _EWSActing = EWS;
        }
        public string DelAppo(string id,string userLogin)
        {
            IEWSActing EWS = _EWSActing;
            IAuthService Auth = new AuthService();
            ExchangeService service = EWS.CrEwsService(userLogin, Auth.getService(userLogin));
            Appointment delAppo = _EWSActing.EWSAppoBind(service, id, new PropertySet(BasePropertySet.IdOnly));
            _EWSActing.EWSDelAppo(delAppo);
            return "deleted";
        }

        public JsonResult GetAppos(string[] CalendarIds, string startDate, string currentViewState, string userLogin)
        {
            int calCounter = 0;
            
            foreach(string calendarId in CalendarIds)
            {
                if (calendarId != "")
                    calCounter++;
            }
            if (calCounter == 0)
            {
                Appo[] ApposFake = new Appo[] {
                           new Appo
                           {
                               startDate=""
                           }
                        };
                return new JsonResult(ApposFake);

            }
            string[] trueCals = new string[calCounter];
            int jk = 0;
            for(int j=0;j< CalendarIds.Length;j++)
            {
                if (CalendarIds[j] != "")
                {
                    trueCals[jk] = CalendarIds[j];
                    jk++;
                }
            }
            string endDate = "";
            DateTime CurDate = DateTime.Parse(startDate);           
            switch (currentViewState)
            {
                case "Month":
                    DateTime dtTemp = new DateTime(CurDate.Year, CurDate.Month, 1);
                    startDate = dtTemp.ToShortDateString();
                    endDate = dtTemp.AddMonths(1).AddSeconds(-1).ToShortDateString();
                    break;                
                case "Week":
                    startDate = CurDate.AddDays(-(int)CurDate.DayOfWeek + 1).Date.ToShortDateString();
                    endDate= CurDate.AddDays(7-(int)CurDate.DayOfWeek).Date.ToShortDateString();
                    break;
                case "Day":
                    startDate = CurDate.AddHours(-1).Date.ToShortDateString();
                    endDate = CurDate.AddHours(25).Date.ToShortDateString();
                    break;
            }

            
            IEWSActing EWS = _EWSActing;
            IAuthService Auth = new AuthService();
            ExchangeService service = EWS.CrEwsService(userLogin, Auth.getService(userLogin));


            //ExCret.Add(service.Credentials);
            //Globs.ExCre.Add(service.Credentials);            
            // Set the start and end time and number of appointments to retrieve.
            Apps[] appointmentsTMP = EWS.FindAppointments(service, trueCals, startDate,endDate);
            Appointment[] appointments = new Appointment[appointmentsTMP.Length];
            for (int iAp=0; iAp < appointmentsTMP.Length; iAp++)
            {
                appointments[iAp] = appointmentsTMP[iAp].App;
            }

            Appo[] ApposArray = new Appo[appointments.Length];

            int i = 0;
            if (appointments.Length > 0)
            {
                foreach (Appointment a in appointments)
                {
                    string[] RecStrings = RecStrings = GetRelatedRecurrenceCalendarItems(service, a);
                    Appo appo = new Appo();
                    appo.title = a.Subject.ToString();
                    appo.startDate = a.Start.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    appo.endDate = a.End.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    appo.calId= appointmentsTMP[i].CalId;
                    if (a.IsAllDayEvent != null)
                        appo.allDay = a.IsAllDayEvent;
                    if (a.Id != null)
                        appo.id = a.Id.ToString();
                    else
                        appo.id = "";
                    if (RecStrings[0] != null)
                    {
                        appo.rRule = "RRULE:" + RecStrings[1];
                        appo.DTSTART = "DTSTART;" + RecStrings[0];
                    }
                    else
                    {
                        appo.rRule = "";
                        appo.DTSTART = "";
                    }

                    ApposArray[i] = appo;
                    i++;
                }
                return new JsonResult(ApposArray);
            }
            else
            {
                ApposArray = new Appo[1];
                ApposArray[0] = new Appo();
                ApposArray[0].startDate = "";
                return new JsonResult(ApposArray);
            }
        }

        public Cal[] GetCals(JsonElement userData)
        {
            IEWSActing EWS = new EWSs();
            string userLogin = userData[0].ToString();
            userLogin = userLogin.Replace("\"", "");
            userLogin = userLogin.Replace("[", "");
            userLogin = userLogin.Replace("]", "");

            return EWS.GetCals(userLogin);
        }

        public string[] GetRelatedRecurrenceCalendarItems(ExchangeService service, Appointment calendarItem)
        {
            IEWSActing EWS = new EWSs();
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
                    recurrMaster = EWS.EWSAppoBind(service, calendarItem.Id.ToString(), props);
                    break;
                // The calendar item is a single instance meeting, so there are no instances to modify or delete.
                case AppointmentType.Single:
                    Console.WriteLine("Item id must reference a calendar item that is part of a recurring series.");
                    return rrule;
                // The calendar item is an occurrence in the series, so use BindToRecurringMaster.
                case AppointmentType.Occurrence:
                    recurrMaster = EWS.EWSBindToRecurringMaster(service, calendarItem.Id.ToString(), props);
                    break;
                // The calendar item is an exception to the series, so use BindToRecurringMaster.                
                case AppointmentType.Exception:
                    recurrMaster = EWS.EWSBindToRecurringMaster(service, calendarItem.Id.ToString(), props);
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

        public string PostAppo(JsonElement JSPostAppo,string userLogin)
        {
            //Deserialise json to array of JsonObject[] 
            var items = JsonObject.Parse(JSPostAppo.ToString());
            //var type = items[0].GetType();
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
            //creating or editing appointments
            IEWSActing EWS = new EWSs();
            IAuthService Auth = new AuthService();

            ExchangeService service = EWS.CrEwsService(userLogin, Auth.getService(userLogin));
            string calId = "";
            foreach(JsonObject jso in JsObjs)
            {
               /* #region Convert UTC to Local Timezone
                //Problem is in service it wont work with TimeZoneInfo.Local

                //start
                string TrueStartDateTime = "";
                if (jso["startDate"] != null&& jso["startDate"].ToString()!="")
                {
                    DateTime ChangedTime = DateTime.Parse(jso["startDate"].ToString());
                    ChangedTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    jso["startDate"] = ChangedTime;
                    TrueStartDateTime = jso["startDate"].ToString().Remove(jso["startDate"].ToString().IndexOf("+"));
                    TrueStartDateTime = TrueStartDateTime + "Z";
                    TrueStartDateTime = TrueStartDateTime.Substring(1);
                }
                //end
                string TrueEndDateTime = "";
                if (jso["endDate"] != null)
                {
                    DateTime ChangedTime = DateTime.Parse(jso["endDate"].ToString());
                    ChangedTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    jso["endDate"] = ChangedTime;
                    TrueEndDateTime = jso["endDate"].ToString().Remove(jso["endDate"].ToString().IndexOf("+"));
                    TrueEndDateTime = TrueEndDateTime + "Z";
                    TrueEndDateTime = TrueEndDateTime.Substring(1);
                }
                #endregion*/
                if (jso["id"] == null)
                {
                    Appointment newAppo = new Appointment(service);
                    newAppo.Subject = jso["title"].ToString();
                    newAppo.Start = DateTime.Parse(jso["startDate"].ToString());
                    newAppo.End = DateTime.Parse(jso["endDate"].ToString());
                    calId = jso["calId"].ToString();
                    newAppos[j] = newAppo;
                    
                }
                else if(jso["title"]!=null&& jso["title"].ToString()== "deleteIt")
                {
                    DelAppo(jso["id"].ToString(),userLogin);
                }
                else
                {
                    try
                    {
                        Appointment editAppo = EWS.EWSAppoBind(service, jso["id"].ToString(), AppoSchemas.AppoPropsSet(1));
                        //string test = jso["title"].ToString();
                        if (jso["title"] != null) editAppo.Subject = jso["title"].ToString();
                        if (jso["startDate"] != null) editAppo.Start = DateTime.Parse(jso["startDate"].ToString());
                        if (jso["endDate"] != null) editAppo.End = DateTime.Parse(jso["endDate"].ToString());
                        if (jso["calId"] != null) calId = jso["calId"].ToString();
                        //stealed from internet. What is that?
                        // Unless explicitly specified, the default is to use SendToAllAndSaveCopy.
                        // This can convert an appointment into a meeting. To avoid this,
                        // explicitly set SendToNone on non-meetings.
                        SendInvitationsOrCancellationsMode mode = editAppo.IsMeeting ?
                        SendInvitationsOrCancellationsMode.SendToAllAndSaveCopy : SendInvitationsOrCancellationsMode.SendToNone;
                        // Send the update request to the Exchange server.
                        EWS.EWSAppoUpdate(editAppo, ConflictResolutionMode.AlwaysOverwrite, mode);
                        newAppos[j] = editAppo;
                        
                    }
                    catch (Microsoft.Exchange.WebServices.Data.ServiceResponseException ex)
                    { 
                    }
                }
                j++;
            }              
            
            return PostOrEditAppo(service, newAppos,calId);            
        }

        public string PostOrEditAppo(ExchangeService service,Appointment[] newAppos,string calId)
        {
                    
            foreach (Appointment newAppo in newAppos)
            {
                if (newAppo != null)
                    try
                    {
                        if (newAppo.Id == null)
                            newAppo.Save(calId, SendInvitationsMode.SendToNone);
                        else
                        {
                            newAppo.Move(calId);
                            continue;
                            // newAppo.Update();
                        }
                        Item item = Item.Bind(service, newAppo.Id, new PropertySet(ItemSchema.Subject));
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        //need a counter and output about results
                    }
            }
            return "ok";
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
