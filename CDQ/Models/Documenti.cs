using System;
using CDQ.Utils;
using Realms;

namespace CDQ.Models
{
    public class Documenti : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Versione { get; set; }
        public string Categoria { get; set; }
		public string TipoFile { get; set; }
		public string NomeFile { get; set; }

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