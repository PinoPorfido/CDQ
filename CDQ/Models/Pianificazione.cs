using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CDQ.Utils;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class Pianificazione : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        public RisorsaAttivita RisorsaAttivita { get; set; }
        public Esercente Esercente { get; set; }
        public Giorno Giorno { get; set; }
        public int OraInizio { get; set; }
        public int OraFine { get; set; }
        public int Capienza { get; set; }

        public DateTimeOffset? DataInizio { get; set; }
        public DateTimeOffset? DataFine { get; set; }

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


        public string DataInizio_OUT
        {
            get
            {
                return DataInizio?.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }

        public string DataFine_OUT
        {
            get
            {
                return DataFine?.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }

        public string Durata_OUT
        {
            get
            {
                return Utils.Utils.ConvertiOrario(OraFine-OraInizio);
            }
        }


    }

}