using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class Relazioni : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public Proposte PropostaRiferimento { get; set; }
        public string User { get; set; }
        public string Oggetto { get; set; }
        public string Tribunale { get; set; }
        public string SezioneTribunale { get; set; }
        public string LuogoData { get; set; }
        public string CauseIndebitamentoeDiligenza { get; set; }
        public string ResocontoPagamenti5Anni { get; set; }
        public string AttoNominaProfessionista { get; set; }
        public string TitoloNominativoProfessionista { get; set; }
        public string IndirizzoProfessionista { get; set; }
        public string RecapitiProfessionista { get; set; }
        public int RateProposte { get; set; }
        public double RataMax { get; set; }
        public int ComponentiNucleo { get; set; }
        public double RedditoMensile { get; set; }
        public double PatrimonioMobProQ { get; set; }
        public double PatrimonioImmobProQ { get; set; }
        public string SafeCode { get; set; } = Guid.NewGuid().GetHashCode().ToString();


        public DateTimeOffset? DataCreazione { get; set; }
        public DateTimeOffset? DataUltimaModifica { get; set; }

        public string DataCreazione_OUT
        {
            get
            {
                return DataCreazione?.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }

        public string DataUltimaModifica_OUT
        {
            get
            {
                return DataUltimaModifica?.DateTime.AddHours(Settings.HOUR).ToShortDateString();
            }
        }

    }

}