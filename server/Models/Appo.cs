namespace EWS_NetCore_Scheduler.Models
{
    public class Appo
    {
        public string startDate { get; set; }
        public string endDate { get; set; }                
        public string title { get; set; }       
        public bool allDay { get; set; }
        public string id { get; set; }
        public string DTSTART { get; set; }
        public string rRule { get; set; }
        public string exDate { get; set; }
    }
}