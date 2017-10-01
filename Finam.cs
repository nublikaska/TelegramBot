using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.Bot.Examples.Echo
{
    class Finam
    {
        private string date;
        private string connection;
        private string from;
        private string df;
        private string mf;
        private string yf;

        private string to;
        private string dt;
        private string mt;
        private string yt;
        private string em;
        private string code;
        private string cn;

        private string f { get; set; }

        private void SetFrom(string d, string m, string y)
        {
            from = "&from=" + d + "." + m + "." + y;
            df = "&df=" + (Convert.ToInt32(d)).ToString();
            mf = "&mf=" + (Convert.ToInt32(m) - 1).ToString();
            yf = "&yf=" + (Convert.ToInt32(y)).ToString();
        }

        private void SetTo(string d, string m, string y)
        {
            to = "&to=" + d + "." + m + "." + y;
            dt = "&dt=" + (Convert.ToInt32(d)).ToString();
            mt = "&mt=" + (Convert.ToInt32(m) - 1).ToString();
            yt = "&yt=" + (Convert.ToInt32(y)).ToString();
        }

        private void SetConnection()
        {
            connection = "http://export.finam.ru/statistic.txt?+market=1" +
                em +
                code +
                "&apply=0" +
                df +
                mf +
                yf +
                from +
                dt +
                mt +
                yt +
                to +
                "&p=1" +
                "&f=GAZP_170928_170928" +
                "&e=.txt" +
                cn +
                "&dtf=3" +
                "&tmf=2" +
                "&MSOR=0" +
                "&mstime=on" +
                "&mstimever=1" +
                "&sep=5" +
                "&sep2=1" +
                "&datf=12";
        }

        private void SetEm_code_cn(string em, string code, string cn)
        {
            this.em = em;
            this.code = code;
            this.cn = cn;
        }

        private Boolean Download()
        {
            WebClient wb = new WebClient();
            try
            {
                wb.DownloadFile(connection, "statistic.txt");
                return true;
            }
            catch (WebException e)
            {
                return false;
            }
        }

        private string GetItem(string tick, int numberItem)
        {
            String[] words = tick.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words[numberItem];
        }

        public string GetTransactionForDay(String Date, String radioButton, String ReturnVolOrCount)
        {
            Double result = 0;
            Double count = 0;

            if (radioButton == "RadioButtonGazprom")
                SetEm_code_cn("&em=16842", "&code=GAZP", "&cn=GAZP");
            else
                SetEm_code_cn("&em=3", "&code=SBER", "&cn=SBER");

            SetFrom(Date.Substring(0, 2), Date.Substring(3, 2), Date.Substring(6));
            SetTo(Date.Substring(0, 2), Date.Substring(3, 2), Date.Substring(6));
            SetConnection();

            Download();

            FileStream file1 = new FileStream("statistic.txt", FileMode.Open);
            StreamReader reader = new StreamReader(file1);

            while (!reader.EndOfStream)
            {
                result += Convert.ToInt32(GetItem(reader.ReadLine(), 3));
                count++;
            }
            reader.Close();

            if (count > 0)
            {
                Double res = result / count;
                if (ReturnVolOrCount == "Vol")
                    return res.ToString("G17");
                else if (ReturnVolOrCount == "Count")
                    return count.ToString();
            }
            else
                return "count = 0";

            return "error";
        }

        public string[] Compare(String Date1, String Date2, String radioButton)
        {
            int before;
            int today;

            try
            {
                before = Convert.ToInt32(GetTransactionForDay(Date1, radioButton, "Vol"));
            }
            catch (Exception e)
            {
                return new string[] { "error", "error", "error" };
            };

            try
            {
                today = Convert.ToInt32(GetTransactionForDay(Date2, radioButton, "Vol"));
            }
            catch (Exception e)
            {
                return new string[] { "error", "error", "error" };
            };

            if (today > before)
                return new string[] { today.ToString(), ">", before.ToString() };
            else if (today < before)
                return new string[] { today.ToString(), "<", before.ToString() };
            else
                return new string[] { today.ToString(), "=", before.ToString() };
        }

        public string AverageForPeriod(ref DateTime DateFrom, DateTime DateTo, String radioButton)
        {
            Double Summ = 0;
            string temp = "";
            Double Count = 0;

            while (DateFrom.ToShortDateString().ToString() != DateTo.ToShortDateString().ToString())
            {
                temp = GetTransactionForDay(DateFrom.ToShortDateString().ToString(), radioButton, "Count");
                if (temp != "error")
                {
                    Summ += Convert.ToInt32(temp);
                    Count++;
                }

                DateFrom = DateFrom.AddDays(1);
            }

            temp = GetTransactionForDay(DateFrom.ToShortDateString().ToString(), radioButton, "Count");
            if (temp != "error")
            {
                Summ += Convert.ToInt32(temp);
                Count++;
            }

            Double res = Summ / Count;
            return res.ToString("G17");
        }
    }
}
