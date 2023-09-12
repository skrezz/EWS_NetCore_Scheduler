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
using System.Data.SQLite;

namespace EWS_NetCore_Scheduler.Service
{
    public interface IAuthService
    {
        JsonResult RegUser(JsonElement userData);
        string LogUser();
        JsonResult TokenCreate(JsonElement userData, int aTokenTimeLimit, int rTokenTimeLimit);
        bool RegCheck(string uLogin);
        ExchangeCredentials getService(string uLogin);
        string RefreshTokenCheck(string userLogin, string refToken);
        bool CheckCreds(JsonElement userData);

    }
    public class AuthService : IAuthService
    {
        public const string iss = "IAD_EWS_Server"; // издатель токена
        public const string aud = "IAD_EWS_Client"; // потребитель токена
        const string ssKey = "superSecureKeyThatmaybe!_needtoBeChanged?-When(I)willUnderstandsHowtodo_it_BETTER";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ssKey));

        public JsonResult TokenCreate(JsonElement userData,int aTokenTimeLimit, int rTokenTimeLimit)
        {
            IEWSActing EWS = new EWSs();
            var uCreds = JsonObject.Parse(userData.ToString());
            string lgn = uCreds[0].ToString();       
     
            //Создаем acces токен
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, lgn) };
            var jwt = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(aTokenTimeLimit)),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedAccesJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            //Создаем рефреш токен
            var encodedRefreshJwt = "";
            string refToken = RefreshTokenCheck(lgn, uCreds[1].ToString());
            if (refToken == "")
            {
                claims = new List<Claim> { new Claim(ClaimTypes.Name, lgn) };
                jwt = new JwtSecurityToken(
                    issuer: iss,
                    audience: aud,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromDays(rTokenTimeLimit)),
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                    );
                encodedRefreshJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            else
                encodedRefreshJwt = refToken;

            Utils.dbInsertOrUpdate(lgn, encodedRefreshJwt, rTokenTimeLimit);
            //Utils.fileWriteLine(Globs.refTokensPath, lgn, encodedRefreshJwt);
            var response = new
            {
                access_token = encodedAccesJwt,
                refresh_token = encodedRefreshJwt,
                login = lgn
            };


            return new JsonResult(response);
        }
        public bool CheckCreds(JsonElement userData)
        {
            IEWSActing EWS = new EWSs();
            var uCreds = JsonObject.Parse(userData.ToString());
            string lgn = uCreds[0].ToString();
            string pwd = uCreds[1].ToString();
            if (EWS.CrEwsService(lgn, new WebCredentials(lgn, pwd)) == null)
                return false;
            return true;
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
        /* Версия для txt файла
         * public string RefreshTokenCheck(string userLogin, string refToken)
         {
             string ln = "";
             if(File.Exists(Globs.refTokensPath))
             using (StreamReader sr = new StreamReader(Globs.refTokensPath))
             {

                 while ((ln = sr.ReadLine()) != null)
                 {
                         if (ln.Contains(userLogin) && ln.Contains(refToken))
                         {
                             return refToken;
                         }
                 }
             }
                 return "";
         }*/

        //версия для SQLite файла
        public string RefreshTokenCheck(string userLogin, string refToken)
        {           
            if (File.Exists(Globs.refTokensPath))
            {
                SQLiteConnection sqlite_conn = new SQLiteConnection(@"Data Source=D:\Proggin\Igor\Scheduler\EWS_NetCore_Scheduler\server\data\rtdb.db");
                try
                {
                    sqlite_conn.Open();
                }
                catch (Exception ex)
                {
                    sqlite_conn.Close();
                    return ex + "-не могу подключиться к бд";
                }
                using (SQLiteCommand sqlite_cmd = new SQLiteCommand(sqlite_conn))
                {
                    sqlite_cmd.CommandText = "SELECT rt,extime FROM rtdb where lgn=\""+ userLogin + "\"";
                    SQLiteDataReader sqlite_datareader;
                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        if (refToken == sqlite_datareader.GetString(0) && DateTime.Compare(DateTime.Parse(sqlite_datareader.GetString(1)), DateTime.UtcNow) > -1)
                        {
                            string res = sqlite_datareader.GetString(0);
                            sqlite_datareader.Close();
                            sqlite_conn.Close();
                            return res;
                        }
                    }
                    sqlite_datareader.Close();
                    sqlite_cmd.Dispose();
                }
                sqlite_conn.Close();
            }            
            return "";
        }

            public JsonResult RegUser(JsonElement userData)
        {
            if (CheckCreds(userData))
            {
                JsonElement userDataForToken = JsonSerializer.SerializeToElement(new string[] { userData[0].ToString(), userData[2].ToString() });        
               
                return new JsonResult(TokenCreate(userDataForToken, 30, 60));
            }
            return new JsonResult("WrongCreds");
        }
    }
}
