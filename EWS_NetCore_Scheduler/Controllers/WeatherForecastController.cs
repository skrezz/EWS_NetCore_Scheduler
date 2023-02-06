using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;

namespace EWS_NetCore_Scheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        /*private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }*/
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;
            Uri redirectionUri = new Uri(redirectionUrl);
            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials("skrezz@outlook.com", "Snips123");
            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;
            service.AutodiscoverUrl("skrezz@outlook.com", RedirectionUrlValidationCallback);
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(30);
            //const int NUM_APPTS = 5;
            // Initialize the calendar folder object with only the folder ID. 
            CalendarFolder calendar = CalendarFolder.Bind(service, WellKnownFolderName.Calendar, new PropertySet());
            // Set the start and end time and number of appointments to retrieve.
            CalendarView cView = new CalendarView(startDate, endDate);
            // Limit the properties returned to the appointment's subject, start time, and end time.
            cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End, AppointmentSchema.Location);
            // Retrieve a collection of appointments by using the calendar view.
            FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);
            //Console.WriteLine("\nThe first " + NUM_APPTS + " appointments on your calendar from " + startDate.Date.ToShortDateString() +
               //               " to " + endDate.Date.ToShortDateString() + " are: \n");
            string strt = "Unnown1";
            string end = "Unnown2";
            string subj = "Unnown3";
            string loc="Unnown4";

            foreach (Appointment a in appointments)
            {
                subj=a.Subject.ToString();
                strt=a.Start.ToString();
                end=a.End.ToString();
                if (a.Location != null)
                {
                    loc= a.Location.ToString();
                }
                break;
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateS = strt,
                DateE = end,
                Loc = loc,
                Subj=subj
            })
            .ToArray();
        }

        /*[HttpGet(Name = "TestSmth")]
        public IEnumerable<WeatherForecast> Gety()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = 123,
                Summary = "312"
            })
            .ToArray();
        }*/
    }
}