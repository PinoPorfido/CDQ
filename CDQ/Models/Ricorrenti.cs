using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class Ricorrenti : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "Cognome")]
        public string Cognome { get; set; }
		
        [Display(Name = "Nome")]
        public string Nome { get; set; }

		[Display(Name = "Codice Fiscale")]
        public string CodiceFiscale { get; set; }

        [Display(Name = "Domicilio")]
        public string Domicilio { get; set; }

        [Display(Name = "% Riferimento")]
        [Range(0.0, 100)]
        public double Percentuale { get; set; }

        public Proposte Proposte { get; set; }
		public Relazioni Relazioni { get; set; }

    }

}