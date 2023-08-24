using EWS_NetCore_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using EWS_NetCore_Scheduler.Interfaces;
using EWS_NetCore_Scheduler.data;
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
        JsonResult RegUser(JsonElement userData);
        string LogUser();
        JsonResult AccessTokenCreate(JsonElement userData);
        bool RegCheck(string uLogin);
        ExchangeCredentials getService(string uLogin);
    }
    public class AuthService : IAuthService
    {
        public const string iss = "IAD_EWS_Server"; // издатель токена
        public const string aud = "IAD_EWS_Client"; // потребитель токена
        const string ssKey = "superSecureKeyThatmaybe!_needtoBeChanged?-When(I)willUnderstandsHowtodo_it_BETTER";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ssKey));

        public JsonResult AccessTokenCreate(JsonElement userData)
        {
            var uCreds = JsonObject.Parse(userData.ToString());
            string lgn = uCreds[0].ToString();
            //проверяем не регался ли пользователь ранее
            if (RegCheck(lgn))
            {
                var alreadyReg = new
                {
                    answr = "Allready registered"
                };
                return new JsonResult(alreadyReg);
            }             
            //Создаем токен
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, lgn) };
            var jwt = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                login = lgn
            };
            //создаем сервис
            IEWSActing EWS = new EWSs();
            string pwd = uCreds[1].ToString();
            EWS.CrEwsService(lgn,new WebCredentials(lgn,pwd));

            return new JsonResult(response);
        }

        public ExchangeCredentials getService(string uLogin)
        {
            if (Globs.ExCre != null)
            foreach (KeyValuePair<string, ExchangeCredentials> uCred in Globs.ExCre)
            {
                if (uCred.Key.Equals(uLogin))
                {
                    return uCred.Value;
                }
            }
            return new WebCredentials("","") ;
        }

        public string LogUser()
        {
            throw new NotImplementedException();
        }

        public bool RegCheck(string uLogin)
        {
            if(Globs.ExCre!=null)
            foreach (KeyValuePair<string, ExchangeCredentials> uCred in Globs.ExCre)
            {
                if(uCred.Key.Equals(uLogin))
                {
                    return true;
                }
            }
            return false;
        }

        public JsonResult RegUser(JsonElement userData)
        {
            
            return AccessTokenCreate(userData);
        }
    }
}
