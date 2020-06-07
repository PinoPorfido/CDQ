using System;
using System.Collections.Generic;
using Realms;

namespace CDQ.Models
{
    public class Giorno : RealmObject
    {
        [PrimaryKey]
        public int ID { get; set; } 
        public string Nome { get; set; }
        public int Ordine { get; set; }

    }

}