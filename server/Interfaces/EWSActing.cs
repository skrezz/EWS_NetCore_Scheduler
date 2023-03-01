using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Interfaces
{
    public interface IEWSActing
    {
            WebCredentials getWebCreds();
            ExchangeService CrEwsService();                 
            
            string EWSDelAppo(Appointment appointment);
        FindItemsResults<Item> appointments(ExchangeService service);
        Appointment EWSAppoBind(ExchangeService service, string id, PropertySet PSet);
        void EWSAppoUpdate(Appointment appo, ConflictResolutionMode conflictResolutionMode, SendInvitationsOrCancellationsMode mode);
        Appointment EWSBindToRecurringMaster(ExchangeService service, string id, PropertySet props);
    }
}
