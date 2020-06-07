using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CDQ.Models;
using CDQ.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpAttivitaRisorse
    {
     
        public List<SelectListItem> ListaAttivit { get; set; }
        public List<SelectListItem> ListaRisors { get; set; }

		public bool Ins { get; set; }
        public int Tab { get; set; }

        public Attivita Attivita { get; set; }
        public Risorsa Risorsa { get; set; }
        public RisorsaAttivita RisorsaAttivita { get; set; }

        public string IDAttivita { get; set; }
        public string IDRisorsa { get; set; }
        public string IDRisorsaAttivita { get; set; }
        //

        public Esercente Esercente { get; set; }

        public IEnumerable<Attivita> ListaAttivita;
        public IEnumerable<Risorsa> ListaRisorse;
        public IEnumerable<RisorsaAttivita> ListaRisorseAttivita;

        public string ModeAttivita { get; set; }
        public string ModeRisorsa { get; set; }
        public string ModeRisorsaAttivita { get; set; }
    }
}
