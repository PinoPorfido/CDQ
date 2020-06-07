using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class RuoloUtente : RealmObject
    {
        public Utente Utente { get; set; }
        public Ruolo Ruolo { get; set; }
    }

}