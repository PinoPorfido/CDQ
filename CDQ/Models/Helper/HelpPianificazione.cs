using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CDQ.Models;
using CDQ.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpPianificazione
    {

        public List<SelectListItem> ListaOrari { get; set; }

        public string Mode { get; set; }

        public RisorsaAttivita RisorsaAttivita { get; set; }
        public Giorno Giorno { get; set; }

        public string IDRisorsaAttivita { get; set; }
        public int IDGiorno { get; set; }

        public string IDOraInizio { get; set; }
        public string IDOraFine { get; set; }

        //

        public Pianificazione Pianificazione { get; set; }

        public List<SelectListItem> ListaRisorseAttivita;
        public List<SelectListItem> ListaRisorseAttivitaCapienza;
        public List<SelectListItem> ListaGiorni;

        //filtri di ricerca
        public IEnumerable<Pianificazione> ListaPianificazioni { get; set; }

        public int IDGiornoR { get; set; }

        public List<SelectListItem> ListaAttivita { get; set; }
        public int IDAttivita { get; set; }

        public List<SelectListItem> ListaRisorse { get; set; }
        public int IDRisorsa { get; set; }
		
		public bool IsSortGiorno { get; set; }
		public bool IsSortAttivita { get; set; }
    	public bool IsSortRisorsa { get; set; }
        public string Esercente { get; set; }



    }
}
