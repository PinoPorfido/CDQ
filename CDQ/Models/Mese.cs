using System;
using System.Collections.Generic;
using Realms;

namespace CDQ.Models
{
    public class Mese : RealmObject
    {
        [PrimaryKey]
        public int ID { get; set; } 
        public string Nome { get; set; }

        public string Head { get { return "ID,Nome"; } }
        public string Content { get { return ID + "," + Nome; } }
    }
   
}