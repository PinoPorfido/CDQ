using System;
using System.Collections.Generic;
using Realms;

namespace CDQ.Models
{
    public class TipoPratica : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Descrizione { get; set; }
        public int Valore { get; set; }
    }
   
}