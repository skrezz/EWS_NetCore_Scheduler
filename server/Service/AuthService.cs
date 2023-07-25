using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using EWS_NetCore_Scheduler.Interfaces;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text;

namespace EWS_NetCore_Scheduler.Service
{
    public interface IAuthService
    {
        string RegUser(JsonElement userData);
        void AccessTokenCreate(JsonElement userData);
    }
    public class AuthService : IAuthService
    {
        public void AccessTokenCreate(JsonElement userData)
        {
            var uCreds = JsonObject.Parse(userData.ToString());
            string lgn = uCreds[0].ToString();
            string pwd = uCreds[1].ToString();
            IEWSActing EWS = new EWSs();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, lgn) };
            var jwt = new JwtSecurityToken(
                issuer: "back",
                audience: "front",
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(pwd)), SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            // ExchangeService service = EWS.CrEwsService(lgn,pwd);
        }

        public string RegUser(JsonElement userData)
        {
            AccessTokenCreate(userData);
            return "";
        }
    }
}
