﻿using System.Text.Json;
using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using EWS_NetCore_Scheduler.Service;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Exchange.WebServices.Data;
using EWS_NetCore_Scheduler.Interfaces;


namespace EWS_NetCore_Scheduler.Service
{
    public interface ISchedulingService
    {
        public JsonResult GetApposInfo(string startD);
        public string PostAppo(JsonResult JSPullAppo);
    }
    public class SchedulingService: ISchedulingService
    {   
        public JsonResult GetApposInfo(string startD)
        {
            IEWSActing EWSAct = new EWSs();
            ExchangeService service = EWSAct.CrEwsService();
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
                string[] RecStrings = RecStrings = EWSAct.GetRelatedRecurrenceCalendarItems(service, a);
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

        public string PostAppo(JsonResult JSPullAppo)
        {
            IEWSActing EWSAct = new EWSs();
            ExchangeService service = EWSAct.CrEwsService();
            return EWSAct.PostOrEditAppo(service, JSPullAppo);            
        }
    }
}
