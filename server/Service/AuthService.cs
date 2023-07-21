using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace EWS_NetCore_Scheduler.Service
{
    public interface IAuthService
    {
        string RegUser(JsonElement userData);   
    }
    public class AuthService : IAuthService
    {
        public string RegUser(JsonElement userData)
        {
            
            return "";
        }
    }
}
