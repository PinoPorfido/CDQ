using System;
using System.Collections.Generic;
using System.Drawing;
using CDQ.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpChart
    {
        public List<string>[] Colors;
        public List<string>[] Labels;
        public List<string>[] Values;
        public List<string>[] IDs;


        public string[] Label;

        public string Serialize(List<string> v)
        {
            string ser = string.Join("+", v.ToArray());
            return "'" + ser.Replace("+", "','") + "'";
        }

        public string SerializeNumber(List<string> v)
        {
            return string.Join(",", v.ToArray());
        }

        public string SerializeLabels(int i)
        {
            return Serialize(Labels[i]);
        }

        public string SerializeValues(int i)
        {
            return SerializeNumber(Values[i]);
        }

        public string SerializeIDs(int i)
        {
            return Serialize(IDs[i]);
        }

        public string SerializeColors(int i)
        {
            return Serialize(Colors[i]);
        }

        public static string Color(int i)
        {
            
            switch (i)
            {
                case 0: return "#4b77a9";
                case 1: return "#5f255f";
                case 2: return "#d21243";
                case 3: return "#B27200";
                case 4: return "DimGray";
                case 5: return "DarkOliveGreen";

            }
            return "";
        }
       
    }   
}
