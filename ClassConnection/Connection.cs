using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassModule;
using static ClassConnection.Connection;

namespace ClassConnection
{
    public class Connection
    {
        public static string err = "Dont have error";
        public List<User> users = new List<User>();
        public List<Call> calls = new List<Call>();

        public enum tabels { users, calls }

        public string localPath = "";

        public OleDbDataReader QueryAccess(string query)
        {   
            try
            {
                localPath = Directory.GetCurrentDirectory();
                OleDbConnection connect = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + localPath + "/db.accdb");
                connect.Open();
                OleDbCommand cmd = new OleDbCommand(query, connect);
                OleDbDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                err = (ex.Message.ToString());
                return null;
            }
        }

        public int SetLastId(tabels tabel)
        {
            try
            {
                LoadData(tabel);
                switch (tabel.ToString())
                {
                    case "users":
                        if (users.Count >= 1)
                        {
                            int max_status = users[0].id;
                            max_status = users.Max(x => x.id);
                            return max_status + 1;
                        }
                        else return 1;
                    case "calls":
                        if (calls.Count >= 1)
                        {
                            int max_status = calls[0].id;
                            max_status = calls.Max(x => x.id);
                            return max_status + 1;
                        }
                        else return 1;

                    default:
                        return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        public void LoadData(tabels zap)
        {
            try
            {
                OleDbDataReader itemQuery = QueryAccess("SELECT * FROM [" + zap.ToString() + "] ORDER BY [Код]");

                if (zap.ToString() == "users")
                {
                    users.Clear();
                    while (itemQuery.Read())
                    {
                        User newEl = new User();
                        newEl.id = Convert.ToInt32(itemQuery.GetValue(0));
                        newEl.phone_num = Convert.ToString(itemQuery.GetValue(1));
                        newEl.fio_user = Convert.ToString(itemQuery.GetValue(2));
                        newEl.passport_data = Convert.ToString(itemQuery.GetValue(3));
                        users.Add(newEl);
                    }
                }
                if (zap.ToString() == "calls")
                {
                    calls.Clear();
                    while (itemQuery.Read())
                    {
                        Call newEl = new Call();
                        newEl.id =            Convert.ToInt32(itemQuery.GetValue(0));
                        newEl.user_id =       Convert.ToInt32(itemQuery.GetValue(1));
                        newEl.category_call = Convert.ToInt32(itemQuery.GetValue(2));
                        newEl.date =          Convert.ToString(itemQuery.GetValue(3));
                        newEl.time_start =    Convert.ToString(itemQuery.GetValue(4));
                        newEl.time_end =      Convert.ToString(itemQuery.GetValue(5));
                        calls.Add(newEl);
                    }
                }

                if (itemQuery != null) itemQuery.Close();
            }
            catch
            {
                Console.WriteLine("NULL");
            }
        }

        public bool ItsNumber(string str) => new Regex(@"((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$").IsMatch(str);

        public bool ItsOnlyFIO(string str) => new Regex(@"[а-яА-Я ]$").IsMatch(str);
        
    }
}
