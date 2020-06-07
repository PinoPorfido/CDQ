using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class PatrimonioImmobiliare : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "Atto di disposizione")]
        public string Descrizione { get; set; }
		
		[Display(Name = "Superficie")]
        public double Superficie { get; set; }

		[Display(Name = "Valori OMI/VAM")]
        [DataType(DataType.Currency)]
        public double ValoreOmiVan { get; set; }
		
		public Proposte Proposte { get; set; }
		public Relazioni Relazioni { get; set; }

        [Display(Name = "Stima per intero")]
        [DataType(DataType.Currency)]
        public double StimaPerIntero
        {
            get
            {
                return Math.Round(Superficie * ValoreOmiVan, 2);

            }
        }
    }

}