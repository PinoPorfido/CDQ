using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CDQ.Models;
using CDQ.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpCalendario
    {

        public List<SelectListItem> ListaOrari { get; set; }

        public IEnumerable<StrutturaCalendario> ListaStrutturaCalendari { get; set; }

        public string Settimana { get; set; }

        public int IDSettimana { get; set; }

        public int Anno { get; set; }

        public string Mode { get; set; }
		
    }
}
