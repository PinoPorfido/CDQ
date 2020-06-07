using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class TestiPrecaricati : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string TipoTesto { get; set; }
        public string Riferimento { get; set; }
        public string Testo { get; set; }
    }

}