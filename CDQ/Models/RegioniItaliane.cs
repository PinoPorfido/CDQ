using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class RegioniItaliane : RealmObject
    {
        [PrimaryKey]
        public string Codice { get; set; } //XX dove è il codice Regione contenuto nel Codice della tabella ComuniItaliani

        public string Regione { get; set; }

    }
   
}