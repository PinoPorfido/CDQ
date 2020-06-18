using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CDQ.Models;
using CDQ.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpPrenotazione
    {

        //public List<SelectListItem> ListaOrari { get; set; }
        public IEnumerable<StrutturaCalendario> ListaStrutturaCalendari { get; set; }
        public string Settimana { get; set; }
        public int IDSettimana { get; set; }
        public string IDCalendario { get; set; }
        public int Anno { get; set; }
        public string ModePrenotazione { get; set; }
        public string IDCalendarioE { get; set; }
        public string IDCalendarioU { get; set; }
        public string Esercente { get; set; }
        public DateTime pg { get; set; }
        public string Nota { get; set; }

        public Calendario Calendario { get; set; }

        //public List<SelectListItem> ListaRisorseAttivita;
        public List<SelectListItem> ListaRisorseAttivitaR;
        //public List<SelectListItem> ListaRisorseAttivitaCapienza;
        //public List<SelectListItem> ListaGiorni;
        public string ModeCalendario { get; set; }
        //public string IDRisorsaAttivita { get; set; }
        public string IDGiorno { get; set; }
        //public string IDOraInizio { get; set; }
        //public string IDOraFine { get; set; }
        public int IDSettimanaC { get; set; }
        //public bool IsNascosto { get; set; }
        public bool ModeEdit { get; set; }
        public int Capienza { get; set; }
        public string  HasConflict { get; set; } //"0": nessun conflitto, altro>:conflitto
        public string IDRisorsaAttivitaR { get; set; }

        public string IDEsercente { get; set; }


    }
}
