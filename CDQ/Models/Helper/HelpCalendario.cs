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

        public IEnumerable<Calendario> ListaCalendari { get; set; }

        public string Mode { get; set; }
		
    }
}
