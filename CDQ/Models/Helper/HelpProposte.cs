using System;
using System.Collections.Generic;
using CDQ.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpProposte
    {
        public List<SelectListItem> ListaAnni { get; set; }

        public int Anno { get; set; }
        
        public IEnumerable<Proposte> ListaProposte {get; set; }
    }
}
