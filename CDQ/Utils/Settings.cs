using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace CDQ.Utils
{
    public static class Settings
    {
        public const string RealmUser = "cdq";
        public const string RealmPassword = "cdq";
        public const string RealmUriService = "https://elabora.appspot.com/federrealmuri";
        public const string RealmPathService = "https://elabora.appspot.com/federrealmpath";
        internal const string RealmPath = "realms://gigiosoft.us1a.cloud.realm.io/cdq";
        internal const string RealmUri = "https://gigiosoft.us1a.cloud.realm.io/";

        public const bool DoBackup = false;

        public const int AdminCode = 250613;
        public static string AdminMail = "luigi.degiacomo@gmail.com";
        public const string Pwd = "25201914";

        public const int SecondiPerNoConnect = 20;

        public static bool DBLocal = false;
        public static bool DBInstall = false;

        public static string OneSignalAppId = "ba37675f-590b-4920-879b-ef1a8c05d248";

        public static string PDF_TEMP_NAME = "temp.pdf";

        public static string TALK_DOCUMENT_PDF_SERVICE = "https://gigios-222618.appspot.com/talkdocumentpdf";
        public static string TALK_DOCUMENT_EXCEL_SERVICE = "https://gigios-222618.appspot.com/talkdocumentexcel";
        public static string TALK_NOTIFICATION_SERVICE = "https://gigios-222618.appspot.com/talknotifier";
        public static string TALK_NOTIFICATION_SERVICE_BACKUP = "https://talk-222512.appspot.com/talknotifierbackup";

        public static string IMPOSTA_USER_NAME = "<Imposta Username>";
        public static string DEFAULT_PASSWORD = DateTime.Now.Year + "";

        public static string COLORE_TRASPARENTE = "Transparent";
        public static string COLORE_GIALLO = "#F1CB13";
        public static string COLORE_ARANCIO = "#E37314";
        public static string COLORE_ROSSO = "DarkRed";

        public static Color COLORE_DA_CONFERMARE = Color.Orange;
        public const string AS_GOOGLEMAPS = "Apri con Google Maps";
        public const string AS_APPLEMAPS = "Apri con Mappe";

        public const string LINK_APPLE_STORE = "https://itunes.apple.com/it/app/ft-cosenza/id1467184544?mt=8";

        public const string ADESIONE_PDF = "/etc/pdf/Adesione.pdf";
        public const string ADESIONE_PENSIONATI_PDF = "/etc/pdf/AdesionePensionati.pdf";
        public const string RICEVUTA_PDF = "/etc/pdf/Ricevuta.pdf";
        public const string RITENUTA_PDF = "/etc/pdf/Ritenuta.pdf";



        public const string DISDETTA_PDF = "/etc/pdf/Disdetta.pdf";
        public const string BOLLETTINO_PDF = "/etc/pdf/Bollettino.pdf";

        public const double HOUR = 2;

        public static string COPYRIGHT = "© GigiosSoft " + DateTime.Now.Year;
        public static string TELEFONO = "3346988626";
        public static string EMAIL = "luigi.degiacomo@gmail.com";

        public static int ORE_MENO = 9;
        public static int ORE_PIU = 18;

        public static string DEVELOPMENT = "Development";

        public static int AULA = 1;
        public static int RICEVUTA = 2;
        public static int ZONALE = 3;
        public static int COMMERCIALISTA = 4;
        public static int TUTOR = 5;
        public static int MASTERTUTOR = 6;
        public static int NSOCIO = 7;
        public static int NSOCIOLP = 8;
        public static int CONSULENTE = 9;

        public static string ID_ASSOCIATO_SERVIZIO_FINANZIATO = "_SERVFIN";
        public static string ID_AGENTE_NAZIONALE = "N001";
    }

}
