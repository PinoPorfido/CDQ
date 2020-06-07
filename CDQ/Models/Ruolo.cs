using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class Ruolo : RealmObject
    {
        [PrimaryKey] 
        public string ID { get; set; }
        public string Descrizione { get; set; }
        public int Valore { get; set; }
    }

}