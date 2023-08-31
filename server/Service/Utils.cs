

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
        public static bool fileWriteLine(string filePath, string line)
        {
            string ln = "";
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        while ((ln = sr.ReadLine()) != null)
                        {
                            if (ln.Contains(line))
                                return true;
                        }
                    }
                    File.AppendAllText(filePath, line + Environment.NewLine);
                }
                   
                else
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.WriteLine(filePath, line + Environment.NewLine);
                    }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
