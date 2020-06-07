using System;
using System.Collections.Generic;
using CDQ.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpRelazioni
    {
        public List<SelectListItem> ListaAnni { get; set; }

        public int Anno { get; set; }

        public string IDPropostaRiferimento { get; set; }
        public string IDSafeCodePropostaRiferimento { get; set; }

        public IEnumerable<Relazioni> ListaRelazioni {get; set; }
    }
}
