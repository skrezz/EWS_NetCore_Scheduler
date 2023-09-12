

using EWS_NetCore_Scheduler.data;
using System.Data.SQLite;

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
        public static bool dbInsertOrUpdate(string lgn,string rt, int rTokenTimeLimit)
        {
            var test = DateTime.UtcNow.Add(TimeSpan.FromDays(rTokenTimeLimit)).ToString();


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
                        return false;
                    }
                bool lgnFound = false;
                using (SQLiteCommand sqlite_cmd = new SQLiteCommand(sqlite_conn))
                {
                   
                    sqlite_cmd.CommandText = "SELECT extime FROM rtdb where lgn=\"" + lgn + "\"";
                    SQLiteDataReader sqlite_datareader;
                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    if (sqlite_datareader.StepCount > 0)
                    {

                        while (sqlite_datareader.Read())
                        {
                            if (DateTime.Compare(DateTime.Parse(sqlite_datareader.GetString(0)), DateTime.UtcNow) > -1)
                                lgnFound = true;
                        }

                    }
                    sqlite_datareader.Close();
                    sqlite_cmd.Dispose();
                }
                using (SQLiteCommand sqlite_cmd = new SQLiteCommand(sqlite_conn))
                {
                    
                    if (lgnFound)
                    {
                        sqlite_cmd.CommandText = "update rtdb set rt =\"" + rt + "\" where  lgn=\"" + lgn + "\"";
                        sqlite_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                       
                        sqlite_cmd.CommandText = "INSERT INTO rtdb(lgn, rt, extime) VALUES(\"" + lgn + "\",\"" + rt + "\",\"" + DateTime.UtcNow.Add(TimeSpan.FromDays(rTokenTimeLimit)).ToString() + "\" ); ";
                        sqlite_cmd.ExecuteNonQuery();
                    }
                }

                sqlite_conn.Close();
            }
            
            return true;
           
        }
    }
}
