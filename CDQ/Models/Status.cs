using System;
using System.Collections.Generic;
using Realms;

namespace CDQ.Models
{
    public class Status : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Descrizione { get; set; }
        public bool IsSistema { get; set; }
        public string User { get; set; }
    }
   
}