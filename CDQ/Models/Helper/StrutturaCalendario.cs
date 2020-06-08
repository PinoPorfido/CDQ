using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CDQ.Utils;

namespace CDQ.Models.Helper
{
    public class StrutturaCalendario
    {
        public string sOrarioCella { get; set; }
		public int iOrarioCella { get; set; }
		public StrutturaCella SC1 { get; set; }
        public StrutturaCella SC2 { get; set; }
        public StrutturaCella SC3 { get; set; }
        public StrutturaCella SC4 { get; set; }
        public StrutturaCella SC5 { get; set; }
        public StrutturaCella SC6 { get; set; }
        public StrutturaCella SC0 { get; set; }

    }

}