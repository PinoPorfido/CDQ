using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class Esercente : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string RagioneSociale { get; set; }
        public string Indirizzo { get; set; }
        public string CAP { get; set; }
        public string Comune { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }
        public string Sitoweb { get; set; }

        public Categoria Categoria { get; set; }

        public string RagioneSocialeF { get; set; }
        public string IndirizzoF { get; set; }
        public string CAPF { get; set; }
        public string ComuneF { get; set; }
        public string ProvinciaF { get; set; }
        public string PEC { get; set; }
        public string SDI { get; set; }
        public string Mail { get; set; }

        public int OraInizio { get; set; }
        public int OraFine { get; set; }

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

        public int MinutiSlot { get; set; }

    }

}