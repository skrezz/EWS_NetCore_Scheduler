

namespace EWS_NetCore_Scheduler.Service
{
    public class Utils
    {
        public static string GetLine(string mrkr, string target)
        {
            string ln;
            string filen="";
            switch (target)
                {
                case "Creds":
                    filen = @"..\Server\data\Creds.txt";
                    break;
            }
            if (target == "") return mrkr;
            using (StreamReader xmlF = new StreamReader(filen))
            {
                while (( ln = xmlF.ReadLine()) != null)
                {
                    if (ln.StartsWith(mrkr + " ") || ln.StartsWith(mrkr + ";"))
                        return ln.Substring(mrkr.Length+1);
                }
            }

                    return mrkr;
        }
    }
}
