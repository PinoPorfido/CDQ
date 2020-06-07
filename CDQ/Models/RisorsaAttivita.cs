using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class RisorsaAttivita : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        public Risorsa Risorsa { get; set; }
        public Attivita Attivita { get; set; }
        public Esercente Esercente { get; set; }

        public int Capienza { get; set; }

    }

}