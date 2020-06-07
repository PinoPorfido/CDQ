using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Globalization;

namespace CDQ.Utils
{
    public static class Utils
    {
        public static string AggiungiStringa(int Valore, string Carattere, int Lunghezza)
        {
            string X = "";
            string Y = Valore.ToString();

            for (int i = 0; i < Lunghezza - Y.Length; i++)
            {
                X += Carattere;

            }

            return X + Y;
        }


        public static string ConvertiOrario (int Orario)
        {

            double ora = Convert.ToDouble(Orario);
            double Ora = ora / 60;

            int iOra = Convert.ToInt32(Math.Truncate(Ora));

            double minuti = (Ora - iOra) * 60;
            int iMinuti = Convert.ToInt32(Math.Truncate(minuti));

            string appo = AggiungiStringa(iOra, "0", 2) + ":" + AggiungiStringa(iMinuti, "0", 2);
            
            return appo;

        }

        public static int SettimanaAnno(DateTime giorno)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            Calendar myCal = cul.Calendar;

            return myCal.GetWeekOfYear(giorno, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static DateTime PrimoGiornoSettimana(int year, int weekOfYear)
        {
            CultureInfo ci = CultureInfo.CurrentCulture;
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }


    }

}
