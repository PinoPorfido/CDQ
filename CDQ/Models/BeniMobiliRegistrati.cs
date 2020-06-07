using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class BeniMobiliRegistrati : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "Atto di disposizione")]
        public string Descrizione { get; set; }
		
		[Display(Name = "Targa")]
        public string Targa { get; set; }

		[Display(Name = "Stima settore")]
        [DataType(DataType.Currency)]
        public double Stima { get; set; }
		
		public Proposte Proposte { get; set; }
		public Relazioni Relazioni { get; set; }

    }

}