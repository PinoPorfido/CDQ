using System;
using CDQ.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpEsercente
    {
        public Esercente Esercente { get; set; }

        public List<SelectListItem> ListaCategorie { get; set; }
        public List<SelectListItem> ListaOrari { get; set; }
        public List<SelectListItem> ListaSlot { get; set; }


        public bool Upd { get; set; } = false;
        public string IDCategoria { get; set; }
        public string IDOraInizio { get; set; }
        public string IDOraFine { get; set; }
        public string IDSlot { get; set; }

    }
}
