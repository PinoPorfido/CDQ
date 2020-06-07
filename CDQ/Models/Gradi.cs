using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class Gradi : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "Grado")]
        public string Descrizione { get; set; }
        public bool IsSistema { get; set; }

        public int Aggregazione { get; set; }
    }

}