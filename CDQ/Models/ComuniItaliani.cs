using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class ComuniItaliani : RealmObject
    {

        [PrimaryKey]
        public int ID { get; set; } 

        public string Codice { get; set; } //XX-YYY-ZZZ dove XX è il codice Regione, YYY è il codice Provincia, ZZZ è il progressivo comune nella provincia

        public string Regione { get; set; }
        
        public string Provincia { get; set; }
        
        public string Comune { get; set; }

        public string CAP { get; set; } //stringa con separatore di tutti i CAP disponibili per il comune

        public string SiglaProvincia { get; set; }
		
		public string CodiceIstat { get; set; }

    }
   
}