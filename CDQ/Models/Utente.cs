using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class Utente : RealmObject
    {
        [PrimaryKey]
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Telefono { get; set; }

        public Esercente Esercente { get; set; } //per definire l'utente legato all'esercente

        public DateTimeOffset? DataUltimoLogin { get; set; }
        public DateTimeOffset? DataPenultimoLogin { get; set; }

        public string DataUltimoLogin_OUT
        {
            get
            {
                return DataUltimoLogin?.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }

        public string DataPenultimoLogin_OUT
        {
            get
            {
                return DataPenultimoLogin?.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }


    }

}