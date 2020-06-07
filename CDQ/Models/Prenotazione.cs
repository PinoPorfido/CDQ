using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class Prenotazione : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        public Utente utente { get; set; }
        public Calendario Calendario { get; set; }
        public Esercente Esercente { get; set; }

    }

}