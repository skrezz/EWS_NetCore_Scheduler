using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Text.Json;

namespace EWS_NetCore_Scheduler.Interfaces
{
    public interface ISchedulingService
    {    
            JsonResult GetAppos(string[] CalendarIds, string startD, string currentViewState);
            string[] GetRelatedRecurrenceCalendarItems(ExchangeService service, Appointment calendarItem);
            string PostAppo(JsonElement JSPostAppo);
            string PostOrEditAppo(ExchangeService service, Appointment[] newAppos, string calId);
            string DelAppo(string id);
            Cal[] GetCals();
        
    }
}
