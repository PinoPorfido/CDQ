using System;
using System.ComponentModel.DataAnnotations;
using CDQ.Utils;
using Realms;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CDQ.Models
{
    public class MasseDebitorie : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public Proposte Proposte { get; set; }
        public Relazioni Relazioni { get; set; }

        [Display(Name = "Creditore")]
        public string Creditore { get; set; }

        [Display(Name = "Causa indebitamento")]
        public CausaDebito CausaDebito { get; set; }

        [Display(Name = "Annotazione")]
        public string Nota { get; set; }

        [Display(Name = "Grado")]
        public Gradi Grado { get; set; }

        [Display(Name = "Importo")]
        [DataType(DataType.Currency)]
        [Range(0.0, 100000000)]
        public double Importo { get; set; }
        
        [Display(Name = "% Soddisfo")]
        [Range(0.0, 100)]
        public double PercentualeSoddisfo { get; set; }

        [Display(Name = "Soddisfo")]
        [DataType(DataType.Currency)]
        public double Soddisfo
        {
            get
            {
                return Math.Round(Importo * PercentualeSoddisfo/100, 2);

            }
        }

    }

}