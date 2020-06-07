using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class BeniMobili : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "Atto di disposizione")]
        public string Descrizione { get; set; }
		
		[Display(Name = "Costo Storico")]
        [DataType(DataType.Currency)]
        public double CostoStorico { get; set; }

		[Display(Name = "% variazione")]
        public double PercentualeVariazione { get; set; }

		public Proposte Proposte { get; set; }
		public Relazioni Relazioni { get; set; }
		
		[Display(Name = "Stima")]
        [DataType(DataType.Currency)]
        public double StimaPerIntero
        {
            get
            {
                return Math.Round(CostoStorico * (1-PercentualeVariazione), 2);

            }
        }

    }

}