using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CDQ.Utils;

namespace CDQ.Models.Helper
{
    public class StrutturaCella
    {
        public Calendario Calendario { get; set; }
        public int TipoCella { get; set; }
        public int Riga { get; set; }
        public int Colonna { get; set; }

    }

}