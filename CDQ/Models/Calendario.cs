using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CDQ.Utils;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class Calendario : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        public RisorsaAttivita RisorsaAttivita { get; set; }
        public Esercente Esercente { get; set; }
        public DateTimeOffset Data { get; set; }
        public int OraInizio { get; set; }
        public int OraFine { get; set; }

        public Pianificazione Pianificazione { get; set; } //origine del record

        public int Capienza { get; set; }

        public string OraInizio_OUT
        {
            get
            {
                return Utils.Utils.ConvertiOrario(OraInizio);            
            }
        }

        public string OraFine_OUT
        {
            get
            {
                return Utils.Utils.ConvertiOrario(OraFine);
            }
        }


        public string Data_OUT
        {
            get
            {
                return Data.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }

    }

}