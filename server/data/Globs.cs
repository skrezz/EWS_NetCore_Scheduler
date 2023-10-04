using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;

namespace EWS_NetCore_Scheduler.data
{
    static class Globs
    {
        public static Dictionary<string, ExchangeCredentials>? ExCre;
        public static string refTokensPath = "D:\\Proggin\\Igor\\Scheduler\\EWS_NetCore_Scheduler\\server\\data\\refTokens.txt";
    }
}
