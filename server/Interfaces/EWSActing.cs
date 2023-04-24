﻿using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EWS_NetCore_Scheduler.Interfaces
{
    public interface IEWSActing
    {
        WebCredentials getWebCreds();
        ExchangeService CrEwsService();

        string EWSDelAppo(Appointment appointment);
        Apps[] FindAppointments(ExchangeService service, string[] CalendarId, string startDate);
        Appointment EWSAppoBind(ExchangeService service, string id, PropertySet PSet);
        void EWSAppoUpdate(Appointment appo, ConflictResolutionMode conflictResolutionMode, SendInvitationsOrCancellationsMode mode);
        Appointment EWSBindToRecurringMaster(ExchangeService service, string id, PropertySet props);
        Cal[] GetCals();
    }
}
