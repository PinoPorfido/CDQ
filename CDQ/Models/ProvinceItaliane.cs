using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class ProvinceItaliane : RealmObject
    {
        [PrimaryKey]
        public string Codice { get; set; } //YYY dove è il codice Provincia contenuto nel Codice della tabella ComuniItaliani

        public string Provincia { get; set; }

        public string SiglaProvincia { get; set; }

    }
   
}