using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class AttiDisposizione : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "Atto di disposizione")]
        public string Descrizione { get; set; }
		
		public Proposte Proposte { get; set; }
        public Relazioni Relazioni { get; set; }
    }

}