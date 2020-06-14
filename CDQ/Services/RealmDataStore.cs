using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CDQ.Models;
using CDQ.Utils;
using Realms;
using Realms.Sync;

namespace CDQ.Services
{
    public static class RealmDataStore
    {
        static User User;

        static Realm Realm;
        static bool AppConfig;

        private const int IDW430 = 5;
        private const int IDINPS = 7;
        private const int IDINAIL = 6;

        private const int IDRUOLONAZ = 0;

        static string SRealmUri;
        static string SRealmPath;

        private static readonly HttpClient client = new HttpClient();

        public static void SetAppConfig()
        {
            AppConfig = true;
        }

        async public static Task<string> RealmURI()
        {

            try
            {

                var response = await client.PostAsync(Settings.RealmUriService, null);

                var responseString = await response.Content.ReadAsStringAsync();

                return responseString;

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return "";
            }
        }

        async public static Task<string> RealmPath()
        {
            try
            {

                var response = await client.PostAsync(Settings.RealmPathService, null);

                var responseString = await response.Content.ReadAsStringAsync();

                return responseString;

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return "";
            }
        }


        async public static Task<Realm> GetRealm(bool async = false,bool compact = false)
        {
            if (AppConfig)
            {
                return await GetRealmApp(async, compact);
            }

            string pathRealm = "C:/CDQ/DB/cdq.realm";

            if (!File.Exists(pathRealm))
            {
                pathRealm = "/Users/luigi/Dropbox/Realm/CDQ/cdq.realm";
            }

            var config = new RealmConfiguration(pathRealm) { SchemaVersion = 13};


            //if (async)
            //{
            //    bool compactOk = compact && Realm.Compact(config);
            //    //Realm realm = Realm.GetInstance(config);
            //    Console.WriteLine("Compact:" + compactOk);
            //}

            Realm realm = Realm.GetInstance(config);

            string path = realm.Config.DatabasePath;

            //Console.WriteLine("RealmPath:" + path);
            
            return realm;

            
        }

        async public static Task<Realm> GetRealm1(bool async = false,bool compact=false)
        {
            if (AppConfig && Realm != null) return Realm;

            if (SRealmUri==null)
            {
                //SRealmUri = await RealmURI();
                //SRealmPath = await RealmPath();
                SRealmUri = Settings.RealmUri;
                SRealmPath = Settings.RealmPath;

            }

            if (User == null || async) User = await User.LoginAsync(Credentials.UsernamePassword(Settings.RealmUser, Settings.RealmPassword, false), new Uri(SRealmUri));
            var realmUrl = new Uri(SRealmPath);
            var config = new FullSyncConfiguration(realmUrl, User);
            //config.SchemaVersion = 6;
            Realm realm = null;
            if (async)
            {
                if (compact) Realm.Compact(config);
                realm = await Realm.GetInstanceAsync(config);
            }
            else realm = Realm.GetInstance(config);

            if (AppConfig) Realm = realm;

            return realm;
        }

        async public static Task<Realm> GetRealmApp(bool async = false, bool compact = false)
        {
            if (AppConfig && Realm != null) return Realm;

            if (SRealmUri == null)
            {
                SRealmUri = await RealmURI();
                SRealmPath = await RealmPath();

            }

            if (User == null || async) User = await User.LoginAsync(Credentials.UsernamePassword(Settings.RealmUser, Settings.RealmPassword, false), new Uri(SRealmUri));
            var realmUrl = new Uri(SRealmPath);
            var config = new FullSyncConfiguration(realmUrl, User);
            Realm realm = null;
            if (async)
            {
                if (compact) Realm.Compact(config);
                realm = await Realm.GetInstanceAsync(config);
            }
            else realm = Realm.GetInstance(config);

            if (AppConfig) Realm = realm;

            return realm;
        }



        async static public Task<bool> LoadRealmAsync()
        {
            SetAppConfig();

            try
            {

                if (User != null) await User.LogOutAsync();

                if (SRealmUri == null)
                {
                    SRealmUri = await RealmURI();
                    SRealmPath = await RealmPath();

                }

                var user = User.LoginAsync(Credentials.UsernamePassword(Settings.RealmUser, Settings.RealmPassword, false), new Uri(SRealmUri));
                User = await user;

                var realmUrl = new Uri(SRealmPath);

                var config = new FullSyncConfiguration(realmUrl, User);

                Realm = await Realm.GetInstanceAsync(config);

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //try { await PopupNavigation.Instance.PopAsync(); } catch (IndexOutOfRangeException) { }
                return false;
            }
        }


        internal async static Task CambiaPassword(Utente utente, string password)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            utente.Password = password;

            trans.Commit();
        }



        internal static async Task<int> CheckRuoli(Utente utenti)
        {
            var vRealmDb = await GetRealm(true);

            IEnumerable ur = vRealmDb.All<RuoloUtente>().Where(a => a.Utente == utenti);

            int ris = 0;

            foreach (RuoloUtente UteRu in ur)
            {
                //costruisce il vettore che rappresenta i ruoli assegnati all'utente
                ris += UteRu.Ruolo.Valore;
            }

            return ris;

        }


        internal static async Task<string> CapienzaResidua(string IDCalendario, string IDUtente)
        {
            var vRealmDb = await GetRealm(true);

            int Booked = -1;

            Calendario calendario = vRealmDb.Find<Calendario>(IDCalendario);

            Utente utente = vRealmDb.Find<Utente>(IDUtente);

            var prenotazione = vRealmDb.All<Prenotazione>().Where(a => a.Calendario == calendario);

            int ris = calendario.Capienza;

            string info = "";

            ris -= prenotazione.Count();

            if (prenotazione.Count() == 0) ris = -ris;

            int i = 1;

            foreach (Prenotazione pr in prenotazione)
            {
                info = info + Environment.NewLine + i + ". " + pr.Utente.Cognome + " " + pr.Utente.Nome + " - " + pr.Nota;
                i += 1;

                if (pr.Utente.Mail == IDUtente) Booked = 1;

            }

            info = info == "" ? "Nessuna prenotazione" : "Dettagli Prenotazioni" + info;
            
            return ris + "#" + info + "#" + Booked;

        }


        internal static async Task<Configurazione> Configurazione()
        {
            var vRealmDb = await GetRealm();

            var lista = vRealmDb.All<Configurazione>();

            if (lista.Count() == 0) return null;

            return lista.First();
        }


        internal async static Task AggiornaLogin(string IDUtenti)
        {
            var vRealmDb = await GetRealm();

            Utente utenti = vRealmDb.Find<Utente>(IDUtenti);
            
            if (utenti != null)
            {
                var trans = vRealmDb.BeginWrite();
                if (utenti.DataUltimoLogin != null)
                {
                    utenti.DataPenultimoLogin = utenti.DataUltimoLogin;
                }
                else
                {
                    utenti.DataPenultimoLogin = DateTimeOffset.Now;
                }
                utenti.DataUltimoLogin = DateTimeOffset.Now;

                trans.Commit();
            }
        }

        internal async static Task<string> AggiornaPassword(string IDUtente, string pwd, string npwd, string cnpwd)
        {
            var vRealmDb = await GetRealm();

            Utente utenti = vRealmDb.Find<Utente>(IDUtente);

            if (pwd == npwd || pwd == cnpwd) return "Non si può cambiare la password con quella corrente";

            if (npwd != cnpwd) return "La nuova password non coincide con la conferma della nuova password";
            
            var trans = vRealmDb.BeginWrite();

            utenti.Password = npwd;

            trans.Commit();

            return "Password corretamente modificata";
        }

        internal static async Task<IEnumerable<RegioniItaliane>> ListaRegioni()
        {
            var vRealmDb = await GetRealm();
            return vRealmDb.All<RegioniItaliane>().OrderBy(r => r.Regione);
        }

        internal static async Task<IEnumerable<Status>> ListaStatus(int Provinciale)
        {
            var vRealmDb = await GetRealm();
            return vRealmDb.All<Status>();
        }

        internal static async Task<IEnumerable<TipoPratica>> ListaTipiPratica(int Provinciale)
        {
            var vRealmDb = await GetRealm();
            return vRealmDb.All<TipoPratica>().OrderBy(a=> a.Valore);
        }


        internal static async Task<IEnumerable<Gradi>> ListaGradi()
        {
            var vRealmDb = await GetRealm();
            return vRealmDb.All<Gradi>();
        }

        internal static async Task<Status> Status(string ID)
        {
            var vRealmDb = await GetRealm();
            
            Status status = vRealmDb.Find<Status>(ID);

            return status;
        }

        internal static async Task<Esercente> Esercente(string ID)
        {
            var vRealmDb = await GetRealm();

            Esercente esercente = vRealmDb.Find<Esercente>(ID);

            return esercente;
        }

        internal static async Task<Pianificazione> Pianificazione(string ID)
        {
            var vRealmDb = await GetRealm();

            Pianificazione pianificazione = vRealmDb.Find<Pianificazione>(ID);

            return pianificazione;
        }

        internal static async Task<Calendario> Calendario(string ID)
        {
            var vRealmDb = await GetRealm();

            Calendario calendario = vRealmDb.Find<Calendario>(ID);

            return calendario;
        }


        internal static async Task<CausaDebito> CausaDebito(string ID)
        {
            var vRealmDb = await GetRealm();

            CausaDebito causaDebito = vRealmDb.Find<CausaDebito>(ID);

            return causaDebito;
        }

        internal static async Task<Utente> Utenti(string ID)
        {
            var vRealmDb = await GetRealm();

            Utente utenti = vRealmDb.Find<Utente>(ID);

            return utenti;
        }



        internal static async Task<Status> AggiornaStatus(Status status)
        {
            var vRealmDb = await GetRealm();

            Status statusMod = vRealmDb.Find<Status>(status.ID);

            var trans = vRealmDb.BeginWrite();

            statusMod.Descrizione = status.Descrizione;

            trans.Commit();

            return status;
        }

        internal static async Task<Esercente> AggiornaEsercente(Esercente esercente, string IDCategoria,string IDOraInizio, string IDOraFine, string IDSlot)
        {
            var vRealmDb = await GetRealm();

            Esercente eseMod = vRealmDb.Find<Esercente>(esercente.ID);
            Categoria categoria = vRealmDb.Find<Categoria>(IDCategoria);

            var trans = vRealmDb.BeginWrite();

            eseMod.RagioneSociale = esercente.RagioneSociale;
            eseMod.Indirizzo = esercente.Indirizzo;
            eseMod.CAP = esercente.CAP;
            eseMod.Comune = esercente.Comune;
            eseMod.Provincia = esercente.Provincia;
            eseMod.Telefono = esercente.Telefono;
            eseMod.Sitoweb = esercente.Sitoweb;
            eseMod.RagioneSocialeF = esercente.RagioneSocialeF;
            eseMod.IndirizzoF = esercente.IndirizzoF;
            eseMod.CAPF = esercente.CAPF;
            eseMod.ComuneF = esercente.ComuneF;
            eseMod.ProvinciaF = esercente.ProvinciaF;
            eseMod.PEC = esercente.PEC;
            eseMod.SDI = esercente.SDI;
            eseMod.Mail = esercente.Mail;
            eseMod.Categoria = categoria;
            eseMod.OraInizio = Convert.ToInt32(IDOraInizio);
            eseMod.OraFine = Convert.ToInt32(IDOraFine);
            eseMod.MinutiSlot = Convert.ToInt32(IDSlot);

            trans.Commit();

            return esercente;
        }


        internal static async Task<Pianificazione> AggiornaPianificazione(Pianificazione pianificazione, string IDRisorsaAttivita, int IDGiorno, string IDOraInizio, string IDOraFine, string IDEsercente)
        {
            var vRealmDb = await GetRealm();

            Pianificazione pMod = vRealmDb.Find<Pianificazione>(pianificazione.ID);
            Giorno giorno = vRealmDb.Find<Giorno>(IDGiorno);
            Esercente esercente = vRealmDb.Find<Esercente>(IDEsercente);
            RisorsaAttivita risorsaAttivita = vRealmDb.Find<RisorsaAttivita>(IDRisorsaAttivita);


            var trans = vRealmDb.BeginWrite();

            pMod.RisorsaAttivita = risorsaAttivita;
            pMod.Esercente = esercente;
            pMod.Giorno = giorno;
            pMod.OraInizio = Convert.ToInt32(IDOraInizio);
            pMod.OraFine = Convert.ToInt32(IDOraFine);
            pMod.Capienza = pianificazione.Capienza;

            trans.Commit();

            return pianificazione;
        }


        internal static async Task<CausaDebito> AggiornaCausaDebito(CausaDebito causaDebito)
        {
            var vRealmDb = await GetRealm();

            CausaDebito cdMod = vRealmDb.Find<CausaDebito>(causaDebito.ID);

            var trans = vRealmDb.BeginWrite();

            cdMod.Descrizione = causaDebito.Descrizione;

            trans.Commit();

            return causaDebito;
        }

        internal static async Task<Status> EliminaStatus(Status status)
        {
            var vRealmDb = await GetRealm();

            Status statusDel = vRealmDb.Find<Status>(status.ID);

            var trans = vRealmDb.BeginWrite();

            vRealmDb.Remove(statusDel);

            trans.Commit();

            return status;
        }

        internal static async Task<CausaDebito> EliminaCausaDebito(CausaDebito causaDebito)
        {
            var vRealmDb = await GetRealm();

            CausaDebito cdDel = vRealmDb.Find<CausaDebito>(causaDebito.ID);

            var trans = vRealmDb.BeginWrite();

            vRealmDb.Remove(cdDel);

            trans.Commit();

            return causaDebito;
        }

        internal static async Task<Status> InserisciStatus(Status status)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            vRealmDb.Add(status);

            trans.Commit();

            return status;
        }

        internal static async Task<CausaDebito> InserisciCausaDebito(CausaDebito causaDebito)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            vRealmDb.Add(causaDebito);

            trans.Commit();

            return causaDebito;
        }



        internal static async Task<IEnumerable<ComuniItaliani>> ListaComuni()
        {
            var vRealmDb = await GetRealm();
            return vRealmDb.All<ComuniItaliani>().OrderBy(n => n.Comune);
        }

        internal static async Task<IEnumerable<ProvinceItaliane>> ListaProvince()
        {
            var vRealmDb = await GetRealm();
            return vRealmDb.All<ProvinceItaliane>().OrderBy(n => n.SiglaProvincia);
        }
        


        internal static async Task<bool> CheckCFDuplicato(string CF)
        {
            //var vRealmDb = await GetRealm(true);

            //int As = vRealmDb.All<Associato>().Where(a => a.CodiceFiscale == CF).Count();

            //if (As > 0) return true;

            return false;

        }

        internal static async Task<byte> CheckRelazioneDaProposta(string IDProposta, string IDSafeCode)
        {
            var vRealmDb = await GetRealm(true);

            if (IDProposta == "" || IDSafeCode == "") return 0; //nessun risultato utilizzabile


            int As = vRealmDb.All<Proposte>().Where(a => a.ID == IDProposta && a.SafeCode == IDSafeCode).Count();

            if (As > 0) return 2; //ok - può caricare

            return 1; //errore di uno o entrambi i volari

        }

        internal static async Task<Utente> CheckUtente(Utente utenteLogin)
        {
            var vRealmDb = await GetRealm(true,true);

            //var lista = vRealmDb.All<Utente>();

            string ID = utenteLogin.Mail;

            Utente utenti = vRealmDb.Find<Utente>(ID);

            if (utenti == null || utenti.Password != utenteLogin.Password) return null;

            return utenti;
        }

        internal static async Task<Utente> CheckUtente(string username, string password)
        {
            var vRealmDb = await GetRealm(true,true);

            Utente utente = vRealmDb.Find<Utente>(username);

            if (utente == null || utente.Password != password) return null;

            return utente;
        }


        internal static async Task<IEnumerable<Status>> ListaStatus(string user)
        {
            var vRealmDb = await GetRealm();

            return vRealmDb.All<Status>().Where(z=> z.User == user || z.User == null).OrderBy(z=> z.IsSistema).ThenBy(z=> z.ID);
        }

        internal static async Task<IEnumerable<CausaDebito>> ListaCauseDebito(string user)
        {
            var vRealmDb = await GetRealm();

            return vRealmDb.All<CausaDebito>().Where(z => z.User == user || z.User == null).OrderBy(z => z.IsSistema).ThenBy(z => z.ID);
        }

        internal static async Task<IEnumerable<Categoria>> ListaCategorie()
        {
            var vRealmDb = await GetRealm();

            return vRealmDb.All<Categoria>().OrderBy(z => z.Descrizione);
        }

        internal static async Task<IEnumerable<Giorno>> ListaGiorni()
        {
            var vRealmDb = await GetRealm();

            return vRealmDb.All<Giorno>().OrderBy(z => z.Ordine);
        }

        internal static async Task<IEnumerable<Calendario>> ListaEventi()
        {
            var vRealmDb = await GetRealm();

            List<Calendario> listaOutput = new List<Calendario>();

            IEnumerable<Calendario> lista = vRealmDb.All<Calendario>().OrderBy(z => z.Data).ThenBy(z => z.OraInizio);
            foreach (Calendario item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal static async Task<IEnumerable<Mese>> ListaMesi()
        {
            var vRealmDb = await GetRealm();

            return vRealmDb.All<Mese>().OrderBy(m => m.ID);
        }


        internal static async Task<Mese> Mese(int mese)
        {
            var vRealmDb = await GetRealm();

            return vRealmDb.Find<Mese>(mese);
        }



        internal async static Task Backup()
        {
            var vRealmDb = await GetRealm(true);
            string path = vRealmDb.Config.DatabasePath;
            string data = "_" + DateTime.Now.ToShortDateString().Replace("/", "-") + "_" + DateTime.Now.ToShortTimeString().Replace(":", ".");
            File.Copy(path, "/Users/luigi/Desktop/Federterziario/CDQ.realm");

        }



        public async static Task LogOut()
        {
            await GetRealm(true,true);
            if (User != null) await User.LogOutAsync();
            Realm = null;
        }


        internal async static Task AggiornaProposta(Proposte prop, string IDStatus, string IDTipiPratica, bool ins, string user)
        {
            var vRealmDb = await GetRealm();

            Status status = vRealmDb.Find<Status>(IDStatus);
            TipoPratica tipoPratica = vRealmDb.Find<TipoPratica>(IDTipiPratica);

            if (ins)
            {
                var trans = vRealmDb.BeginWrite();

                prop.Status = status;
                prop.TipoPratica = tipoPratica;
                prop.DataCreazione = DateTimeOffset.Now;
                prop.DataUltimaModifica = DateTimeOffset.Now;
                prop.User = user;

                vRealmDb.Add(prop);

                TestiPrecaricati testiPrecaricati = vRealmDb.All<TestiPrecaricati>().Where(a=> a.Riferimento== "SpeseMensili").First();
                //crea le spese standard da modificare in seguito
                if (testiPrecaricati != null)
                {
                    string testo = testiPrecaricati.Testo;
                    string[] words = testo.Split('#');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] != "")
                        {
                            SpeseMese speseMese = new SpeseMese
                            {
                                Descrizione = words[i],
                                Importo = 0,
                                Proposte = prop
                            };

                            vRealmDb.Add(speseMese);
                        }
                    }
                }

                TestiPrecaricati testiPrecaricatiB = vRealmDb.All<TestiPrecaricati>().Where(a => a.Riferimento == "BeniMobili").First();
                //crea le spese standard da modificare in seguito
                if (testiPrecaricatiB != null)
                {
                    string testo = testiPrecaricatiB.Testo;
                    string[] words = testo.Split('#');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] != "")
                        {
                            BeniMobili beniMobili = new BeniMobili
                            {
                                Descrizione = words[i],
                                CostoStorico = 0,
                                PercentualeVariazione = 0,
                                Proposte = prop
                            };

                            vRealmDb.Add(beniMobili);
                        }
                    }
                }


                TestiPrecaricati testiPrecaricatiM = vRealmDb.All<TestiPrecaricati>().Where(a => a.Riferimento == "MassaDebitoria").First();
                CausaDebito causaDebito = vRealmDb.All<CausaDebito>().Where(a => a.Descrizione == "Spese di Procedura").First();
                Gradi gradi = vRealmDb.Find<Gradi>("G1");
                //crea le spese standard da modificare in seguito
                if (testiPrecaricatiM != null)
                {
                    string testo = testiPrecaricatiM.Testo;
                    string[] words = testo.Split('#');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] != "")
                        {
                            MasseDebitorie masseDebitorie = new MasseDebitorie
                            {
                                Creditore = words[i],
                                CausaDebito = causaDebito,
                                Grado = gradi,
                                Proposte = prop,
                                PercentualeSoddisfo = 100
                            };

                            vRealmDb.Add(masseDebitorie);
                        }
                    }
                }


                trans.Commit();
            }
            else
            {
                Proposte proposte = vRealmDb.Find<Proposte>(prop.ID);

                var trans = vRealmDb.BeginWrite();

                proposte.Status = status;
                proposte.TipoPratica = tipoPratica;
                proposte.DataUltimaModifica = DateTimeOffset.Now;
                proposte.Oggetto = prop.Oggetto;
                proposte.Tribunale = prop.Tribunale;
                proposte.SezioneTribunale = prop.SezioneTribunale;
                proposte.LuogoData = prop.LuogoData;
                proposte.CauseIndebitamentoeDiligenza = prop.CauseIndebitamentoeDiligenza;
                proposte.ResocontoPagamenti5Anni = prop.ResocontoPagamenti5Anni;
                proposte.AttoNominaProfessionista = prop.AttoNominaProfessionista;
                proposte.TitoloNominativoProfessionista = prop.TitoloNominativoProfessionista;
                proposte.IndirizzoProfessionista = prop.IndirizzoProfessionista;
                proposte.RecapitiProfessionista = prop.RecapitiProfessionista;
                proposte.RateProposte = prop.RateProposte;
                proposte.RataMax = prop.RataMax;
                proposte.ComponentiNucleo = prop.ComponentiNucleo;
                proposte.RedditoMensile = prop.RedditoMensile;
                proposte.PatrimonioMobProQ = prop.PatrimonioMobProQ;
                proposte.PatrimonioImmobProQ = prop.PatrimonioImmobProQ;

                trans.Commit();
            }
        }


        internal async static Task AggiornaRelazione(Relazioni prop, string IDStatus, string IDTipiPratica, bool ins, string user)
        {
            var vRealmDb = await GetRealm();

            Status status = vRealmDb.Find<Status>(IDStatus);
            TipoPratica tipoPratica = vRealmDb.Find<TipoPratica>(IDTipiPratica);


            if (ins)
            {
                var trans = vRealmDb.BeginWrite();

                prop.Status = status;
                prop.TipoPratica = tipoPratica;
                prop.DataCreazione = DateTimeOffset.Now;
                prop.DataUltimaModifica = DateTimeOffset.Now;
                prop.User = user;

                vRealmDb.Add(prop);

                TestiPrecaricati testiPrecaricati = vRealmDb.All<TestiPrecaricati>().Where(a => a.Riferimento == "SpeseMensili").First();
                //crea le spese standard da modificare in seguito
                if (testiPrecaricati != null)
                {
                    string testo = testiPrecaricati.Testo;
                    string[] words = testo.Split('#');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] != "")
                        {
                            SpeseMese speseMese = new SpeseMese
                            {
                                Descrizione = words[i],
                                Importo = 0,
                                Relazioni = prop
                            };

                            vRealmDb.Add(speseMese);
                        }
                    }
                }

                TestiPrecaricati testiPrecaricatiB = vRealmDb.All<TestiPrecaricati>().Where(a => a.Riferimento == "BeniMobili").First();
                //crea le spese standard da modificare in seguito
                if (testiPrecaricatiB != null)
                {
                    string testo = testiPrecaricatiB.Testo;
                    string[] words = testo.Split('#');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] != "")
                        {
                            BeniMobili beniMobili = new BeniMobili
                            {
                                Descrizione = words[i],
                                CostoStorico = 0,
                                PercentualeVariazione = 0,
                                Relazioni = prop
                            };

                            vRealmDb.Add(beniMobili);
                        }
                    }
                }


                TestiPrecaricati testiPrecaricatiM = vRealmDb.All<TestiPrecaricati>().Where(a => a.Riferimento == "MassaDebitoria").First();
                CausaDebito causaDebito = vRealmDb.All<CausaDebito>().Where(a => a.Descrizione == "Spese di Procedura").First();
                Gradi gradi = vRealmDb.Find<Gradi>("G1");
                //crea le spese standard da modificare in seguito
                if (testiPrecaricatiM != null)
                {
                    string testo = testiPrecaricatiM.Testo;
                    string[] words = testo.Split('#');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] != "")
                        {
                            MasseDebitorie masseDebitorie = new MasseDebitorie
                            {
                                Creditore = words[i],
                                CausaDebito = causaDebito,
                                Grado = gradi,
                                Relazioni = prop,
                                PercentualeSoddisfo = 100
                            };

                            vRealmDb.Add(masseDebitorie);
                        }
                    }
                }


                trans.Commit();
            }
            else
            {
                Relazioni relazioni = vRealmDb.Find<Relazioni>(prop.ID);

                var trans = vRealmDb.BeginWrite();

                relazioni.Status = status;
                relazioni.TipoPratica = tipoPratica;
                relazioni.DataUltimaModifica = DateTimeOffset.Now;
                relazioni.Oggetto = prop.Oggetto;
                relazioni.Tribunale = prop.Tribunale;
                relazioni.SezioneTribunale = prop.SezioneTribunale;
                relazioni.LuogoData = prop.LuogoData;
                relazioni.CauseIndebitamentoeDiligenza = prop.CauseIndebitamentoeDiligenza;
                relazioni.ResocontoPagamenti5Anni = prop.ResocontoPagamenti5Anni;
                relazioni.AttoNominaProfessionista = prop.AttoNominaProfessionista;
                relazioni.TitoloNominativoProfessionista = prop.TitoloNominativoProfessionista;
                relazioni.IndirizzoProfessionista = prop.IndirizzoProfessionista;
                relazioni.RecapitiProfessionista = prop.RecapitiProfessionista;
                relazioni.RateProposte = prop.RateProposte;
                relazioni.RataMax = prop.RataMax;
                relazioni.ComponentiNucleo = prop.ComponentiNucleo;
                relazioni.RedditoMensile = prop.RedditoMensile;
                relazioni.PatrimonioMobProQ = prop.PatrimonioMobProQ;
                relazioni.PatrimonioImmobProQ = prop.PatrimonioImmobProQ;

                trans.Commit();
            }
        }



        internal async static Task<Proposte> Proposte(string ID)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(ID);

            return proposte;

        }

        internal async static Task<Relazioni> Relazioni(string ID)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(ID);

            return relazioni;

        }



        internal async static Task<IEnumerable<Proposte>> ListaProposte(int anno, string user)
        {
            var vRealmDb = await GetRealm();

            DateTime dtMin = new DateTime(anno, 1, 1);
            DateTime dtMax = new DateTime(anno, 12, 31);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = (AppConfig || environment == Settings.DEVELOPMENT);
            DateTime dt1 = dtMin;
            DateTime dt2 = dtMax;
            if (true || !isDevelopment)
            {
                dt1 = dtMin.AddHours(-Settings.ORE_MENO);
                dt2 = dtMax.AddHours(-Settings.ORE_MENO + Settings.ORE_PIU);
            }

            var lista = vRealmDb.All<Proposte>().Where(s => s.DataUltimaModifica >= dt1 && s.DataUltimaModifica <= dt2 && s.User == user).OrderByDescending(s => s.DataUltimaModifica);

            return lista;
        }

        internal async static Task<IEnumerable<Relazioni>> ListaRelazioni(int anno, string user)
        {
            var vRealmDb = await GetRealm();

            DateTime dtMin = new DateTime(anno, 1, 1);
            DateTime dtMax = new DateTime(anno, 12, 31);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = (AppConfig || environment == Settings.DEVELOPMENT);
            DateTime dt1 = dtMin;
            DateTime dt2 = dtMax;
            if (true || !isDevelopment)
            {
                dt1 = dtMin.AddHours(-Settings.ORE_MENO);
                dt2 = dtMax.AddHours(-Settings.ORE_MENO + Settings.ORE_PIU);
            }

            var lista = vRealmDb.All<Relazioni>().Where(s => s.DataUltimaModifica >= dt1 && s.DataUltimaModifica <= dt2 && s.User == user).OrderByDescending(s => s.DataUltimaModifica);

            return lista;
        }


        internal async static Task AggiornaRicorrenti(string IDRicorrenti, Ricorrenti ricorrenti)
        {
            var vRealmDb = await GetRealm();

            Ricorrenti md = vRealmDb.Find<Ricorrenti>(IDRicorrenti);

            var trans = vRealmDb.BeginWrite();

            md.Cognome = ricorrenti.Cognome.Replace("'", "`");
            md.Nome = ricorrenti.Nome.Replace("'", "`");
            md.CodiceFiscale = ricorrenti.CodiceFiscale.Replace("'", "`");
            md.Domicilio = ricorrenti.Domicilio.Replace("'", "`");
            md.Percentuale = ricorrenti.Percentuale;

            trans.Commit();
        }

        internal async static Task AggiornaAttivita(string IDAttivita, Attivita attivita)
        {
            var vRealmDb = await GetRealm();

            Attivita md = vRealmDb.Find<Attivita>(IDAttivita);

            var trans = vRealmDb.BeginWrite();

            md.Descrizione = attivita.Descrizione;

            trans.Commit();
        }


        internal async static Task AggiornaRisorsaAttivita(string IDRisorsaAttivita, RisorsaAttivita risorsaAttivita, string IDAttivita, string IDRisorsa)
        {
            var vRealmDb = await GetRealm();


            Attivita attivita = vRealmDb.Find<Attivita>(IDAttivita);
            Risorsa risorsa = vRealmDb.Find<Risorsa>(IDRisorsa);

            RisorsaAttivita md = vRealmDb.Find<RisorsaAttivita>(IDRisorsaAttivita);

            var trans = vRealmDb.BeginWrite();

            md.Attivita = attivita;
            md.Risorsa = risorsa;
            md.Capienza = risorsaAttivita.Capienza;

            trans.Commit();
        }

        internal async static Task AggiornaRisorsa(string IDRisorsa, Risorsa risorsa)
        {
            var vRealmDb = await GetRealm();

            Risorsa md = vRealmDb.Find<Risorsa>(IDRisorsa);

            var trans = vRealmDb.BeginWrite();

            md.Descrizione = risorsa.Descrizione;
            md.Capienza = risorsa.Capienza;

            trans.Commit();
        }

        internal async static Task AggiornaMasseDebitorie(string IDMasseDebitorie, MasseDebitorie masseDebitorie, string IDCauseDebito, string IDGrado)
        {
            var vRealmDb = await GetRealm();

            CausaDebito causaDebito = vRealmDb.Find<CausaDebito>(IDCauseDebito);
            Gradi gradi = vRealmDb.Find<Gradi>(IDGrado);
            MasseDebitorie md = vRealmDb.Find<MasseDebitorie>(IDMasseDebitorie);

            var trans = vRealmDb.BeginWrite();

            md.Creditore = masseDebitorie.Creditore.Replace("'", "`");
            md.CausaDebito = causaDebito;
            md.Nota = masseDebitorie.Nota;
            md.Grado = gradi;
            md.Importo = masseDebitorie.Importo;
            md.PercentualeSoddisfo = masseDebitorie.PercentualeSoddisfo;

            trans.Commit();
        }

        internal async static Task AggiornaAttiDisposizione(string IDAttiDisposizione, AttiDisposizione attiDisposizione)
        {
            var vRealmDb = await GetRealm();

            AttiDisposizione ad = vRealmDb.Find<AttiDisposizione>(IDAttiDisposizione);

            var trans = vRealmDb.BeginWrite();

            ad.Descrizione = attiDisposizione.Descrizione.Replace("'", "`");

            trans.Commit();
        }


        internal async static Task AggiornaPatrimonioImmobiliare(string IDPatrimonioImmobiliare, PatrimonioImmobiliare patrimonioImmobiliare)
        {
            var vRealmDb = await GetRealm();

            PatrimonioImmobiliare ad = vRealmDb.Find<PatrimonioImmobiliare>(IDPatrimonioImmobiliare);

            var trans = vRealmDb.BeginWrite();

            ad.Descrizione = patrimonioImmobiliare.Descrizione.Replace("'", "`");
            ad.Superficie = patrimonioImmobiliare.Superficie;
            ad.ValoreOmiVan = patrimonioImmobiliare.ValoreOmiVan;

            trans.Commit();
        }

        internal async static Task AggiornaBeniMobiliRegistrati(string IDBeniMobiliRegistrati, BeniMobiliRegistrati beniMobiliRegistrati)
        {
            var vRealmDb = await GetRealm();

            BeniMobiliRegistrati ad = vRealmDb.Find<BeniMobiliRegistrati>(IDBeniMobiliRegistrati);

            var trans = vRealmDb.BeginWrite();

            ad.Descrizione = beniMobiliRegistrati.Descrizione.Replace("'", "`");
            ad.Targa = beniMobiliRegistrati.Targa;
            ad.Stima = beniMobiliRegistrati.Stima;

            trans.Commit();
        }

        internal async static Task AggiornaBeniMobili(string IDBeniMobili, BeniMobili beniMobili)
        {
            var vRealmDb = await GetRealm();

            BeniMobili ad = vRealmDb.Find<BeniMobili>(IDBeniMobili);

            var trans = vRealmDb.BeginWrite();

            ad.Descrizione = beniMobili.Descrizione.Replace("'", "`");
            ad.CostoStorico = beniMobili.CostoStorico;

            trans.Commit();
        }

        internal async static Task AggiornaSpeseMese(string IDSpeseMese, SpeseMese speseMese)
        {
            var vRealmDb = await GetRealm();

            SpeseMese ad = vRealmDb.Find<SpeseMese>(IDSpeseMese);

            var trans = vRealmDb.BeginWrite();

            ad.Descrizione = speseMese.Descrizione.Replace("'", "`");
            ad.Importo = speseMese.Importo;

            trans.Commit();
        }

        internal async static Task AggiornaAnnotazioniAggiuntive(string IDAnnotazioniAggiuntive, AnnotazioniAggiuntive annotazioniAggiuntive)
        {
            var vRealmDb = await GetRealm();

            AnnotazioniAggiuntive ad = vRealmDb.Find<AnnotazioniAggiuntive>(IDAnnotazioniAggiuntive);

            var trans = vRealmDb.BeginWrite();

            ad.Descrizione = annotazioniAggiuntive.Descrizione.Replace("'", "`");

            trans.Commit();
        }



        internal async static Task EliminaMasseDebitorie(string IDMasseDebitorie)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            MasseDebitorie ap = vRealmDb.Find<MasseDebitorie>(IDMasseDebitorie);

            vRealmDb.Remove(ap);

            trans.Commit();
        }

        internal async static Task EliminaAttiDisposizione(string IDAttiDisposizione)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            AttiDisposizione ap = vRealmDb.Find<AttiDisposizione>(IDAttiDisposizione);

            vRealmDb.Remove(ap);

            trans.Commit();
        }

        internal async static Task EliminaSpeseMese(string IDSpeseMese)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            SpeseMese ap = vRealmDb.Find<SpeseMese>(IDSpeseMese);

            vRealmDb.Remove(ap);

            trans.Commit();
        }

        internal async static Task EliminaAnnotazioniAggiuntive(string IDAnnotazioniAggiuntive)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            AnnotazioniAggiuntive ap = vRealmDb.Find<AnnotazioniAggiuntive>(IDAnnotazioniAggiuntive);

            vRealmDb.Remove(ap);

            trans.Commit();
        }

        internal async static Task EliminaPatrimonioImmobiliare(string IDPatrimonioImmobiliare)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            PatrimonioImmobiliare ap = vRealmDb.Find<PatrimonioImmobiliare>(IDPatrimonioImmobiliare);

            vRealmDb.Remove(ap);

            trans.Commit();
        }

        internal async static Task EliminaBeniMobiliRegistrati(string IDBeniMobiliRegistrati)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            BeniMobiliRegistrati ap = vRealmDb.Find<BeniMobiliRegistrati>(IDBeniMobiliRegistrati);

            vRealmDb.Remove(ap);

            trans.Commit();
        }

        internal async static Task EliminaBeniMobili(string IDBeniMobili)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            BeniMobili ap = vRealmDb.Find<BeniMobili>(IDBeniMobili);

            vRealmDb.Remove(ap);

            trans.Commit();
        }


        internal async static Task EliminaRicorrenti(string IDRicorrenti)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            Ricorrenti ap = vRealmDb.Find<Ricorrenti>(IDRicorrenti);

            vRealmDb.Remove(ap);

            //eliminazione di tutte le dipendenze nella proposta o nella relazione collegata

            trans.Commit();
        }

        internal async static Task EliminaAttivita(string IDAttivita)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            Attivita ap = vRealmDb.Find<Attivita>(IDAttivita);

            vRealmDb.Remove(ap);

            //eliminazione di tutte le dipendenze dell'attività

            trans.Commit();
        }

        internal async static Task EliminaRisorsa(string IDRisorsa)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            Risorsa ap = vRealmDb.Find<Risorsa>(IDRisorsa);

            vRealmDb.Remove(ap);

            //eliminazione di tutte le dipendenze della risorsa

            trans.Commit();
        }

        internal async static Task EliminaPianificazione(string IDPianificazione)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            Pianificazione ap = vRealmDb.Find<Pianificazione>(IDPianificazione);

            vRealmDb.Remove(ap);

            //eliminazione di tutte le dipendenze della risorsa

            trans.Commit();
        }

        internal async static Task EliminaCalendario(string IDCalendario)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            Calendario ap = vRealmDb.Find<Calendario>(IDCalendario);

            //eliminazione prenotazioni
            var prenotazione = vRealmDb.All<Prenotazione>().Where(a=> a.Calendario == ap);

            vRealmDb.RemoveRange(prenotazione);

            //elimina calenadrio
            vRealmDb.Remove(ap);


            trans.Commit();
        }

        internal async static Task EliminaPrenotazione(string IDCalendario, string IDUtente)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            Calendario ap = vRealmDb.Find<Calendario>(IDCalendario);

            Utente utente = vRealmDb.Find<Utente>(IDUtente);

            //eliminazione prenotazioni
            var prenotazione = vRealmDb.All<Prenotazione>().Where(a => a.Calendario == ap && a.Utente == utente);

            vRealmDb.RemoveRange(prenotazione);

             trans.Commit();
        }


        internal async static Task EliminaRisorsaAttivita(string IDRisorsaAttivita)
        {
            var vRealmDb = await GetRealm();

            var trans = vRealmDb.BeginWrite();

            RisorsaAttivita ap = vRealmDb.Find<RisorsaAttivita>(IDRisorsaAttivita);

            vRealmDb.Remove(ap);

            //eliminazione di tutte le dipendenze della risorsa

            trans.Commit();
        }


        internal async static Task InserisciMasseDebitorie(string IDproposte, MasseDebitorie masseDebitorie, string IDCausaDebito, string IDGrado)
        {
            var vRealmDb = await GetRealm();

            CausaDebito causaDebito = vRealmDb.Find<CausaDebito>(IDCausaDebito);
            Gradi gradi = vRealmDb.Find<Gradi>(IDGrado);
            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            MasseDebitorie md = new MasseDebitorie
            {
                Proposte = proposte,
                Creditore = masseDebitorie.Creditore.Replace("'", "`"),
                CausaDebito = causaDebito,
                Nota = masseDebitorie.Nota,
                Grado = gradi,
                Importo = masseDebitorie.Importo,
                PercentualeSoddisfo = masseDebitorie.PercentualeSoddisfo
            };

            vRealmDb.Add(md);

            trans.Commit();
        }

        internal async static Task InserisciRicorrenti(string IDproposte, Ricorrenti ricorrenti)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);

            var trans = vRealmDb.BeginWrite();

            Ricorrenti md = new Ricorrenti
            {
                Proposte = proposte,
                Cognome = ricorrenti.Cognome.Replace("'", "`"),
                Nome = ricorrenti.Nome.Replace("'", "`"),
                CodiceFiscale = ricorrenti.CodiceFiscale,
                Domicilio = ricorrenti.Domicilio.Replace("'", "`"),
                Percentuale = ricorrenti.Percentuale,
            };

            vRealmDb.Add(md);

            trans.Commit();
        }

        internal async static Task InserisciAttivita(string IDesercente, Attivita attivita)
        {
            var vRealmDb = await GetRealm();

            Esercente esercente = vRealmDb.Find<Esercente>(IDesercente);

            var trans = vRealmDb.BeginWrite();

            Attivita md = new Attivita
            {
                Esercente = esercente,
                Descrizione = attivita.Descrizione
            };

            vRealmDb.Add(md);

            trans.Commit();
        }

        internal async static Task InserisciRisorsaAttivita(string IDesercente, RisorsaAttivita risorsaAttivita, string IDAttivita, string IDRisorsa)
        {
            var vRealmDb = await GetRealm();

            Esercente esercente = vRealmDb.Find<Esercente>(IDesercente);
            Attivita attivita = vRealmDb.Find<Attivita>(IDAttivita);
            Risorsa risorsa = vRealmDb.Find<Risorsa>(IDRisorsa);

            var trans = vRealmDb.BeginWrite();

            RisorsaAttivita md = new RisorsaAttivita
            {
                Esercente = esercente,
                Attivita = attivita,
                Risorsa = risorsa,
                Capienza = risorsaAttivita.Capienza
            };

            vRealmDb.Add(md);

            trans.Commit();
        }



        internal async static Task InserisciRisorsa(string IDesercente, Risorsa risorsa)
        {
            var vRealmDb = await GetRealm();

            Esercente esercente = vRealmDb.Find<Esercente>(IDesercente);

            var trans = vRealmDb.BeginWrite();

            Risorsa md = new Risorsa
            {
                Esercente = esercente,
                Descrizione = risorsa.Descrizione,
                Capienza = risorsa.Capienza
            };

            vRealmDb.Add(md);

            trans.Commit();
        }

        internal async static Task CreaCalendario(string IDesercente, int IDSettimana, int Anno)
        {
            var vRealmDb = await GetRealm();

            //crea il calenadio aziendale per la settimana indicata basandosi sulla Pianificazione dell'Esercente

            Esercente esercente = vRealmDb.Find<Esercente>(IDesercente);

            DateTime dtPrimoGiornoSettimana = Utils.Utils.PrimoGiornoSettimana(Anno, IDSettimana);
            DateTime dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);

            IEnumerable<Pianificazione> pianificazione = await ListaPianificazioni(esercente, -1, "-1", "-1", true, false, false);

            var trans = vRealmDb.BeginWrite();

            if (pianificazione.Count()>0)   //cancelliamo tutto solo se la pianificazione ha almeno un record da inserire
            {
                //cancellazione prenotazioni
                var listaC = vRealmDb.All<Calendario>().Where(ss => ss.Data <= dtUltimoGiornoSettimana && ss.Data >= dtPrimoGiornoSettimana && ss.Esercente == esercente);
                foreach (Calendario cc in listaC)
                {
                    var listaP = vRealmDb.All<Prenotazione>().Where(ss => ss.Calendario == cc);
                    vRealmDb.RemoveRange<Prenotazione>(listaP);
                }

                //cancellazione calendario
                vRealmDb.RemoveRange<Calendario>(listaC);
            }

            //inserimento nuovi elementi nel Calnedario
            foreach (Pianificazione ss in pianificazione)
            {
                Calendario md = new Calendario
                {
                    RisorsaAttivita = ss.RisorsaAttivita,
                    Esercente = esercente,
                    OraInizio = ss.OraInizio,
                    OraFine = ss.OraFine,
                    Pianificazione = ss,
                    Capienza = ss.Capienza,
                    Data = dtPrimoGiornoSettimana.AddDays(ss.Giorno.ID-1).AddHours(12)
                };
                vRealmDb.Add(md);
            }

            trans.Commit();
        }


        internal async static Task InserisciPrenotazione(string IDesercente, string IDCalendario, string IDUtente, string Nota)
        {
            var vRealmDb = await GetRealm();

            Esercente esercente = vRealmDb.Find<Esercente>(IDesercente);
            Calendario calendario = vRealmDb.Find<Calendario>(IDCalendario);
            Utente utente = vRealmDb.Find<Utente>(IDUtente);

            var trans = vRealmDb.BeginWrite();

            Prenotazione md = new Prenotazione
            {
                Esercente = esercente,
                Calendario = calendario,
                Utente = utente,
                Nota = Nota
            };

            vRealmDb.Add(md);

            trans.Commit();
        }


        internal async static Task InserisciRicorrentiR(string IDrelazioni, Ricorrenti ricorrenti)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);

            var trans = vRealmDb.BeginWrite();

            Ricorrenti md = new Ricorrenti
            {
                Relazioni = relazioni,
                Cognome = ricorrenti.Cognome.Replace("'", "`"),
                Nome = ricorrenti.Nome.Replace("'", "`"),
                CodiceFiscale = ricorrenti.CodiceFiscale,
                Domicilio = ricorrenti.Domicilio.Replace("'", "`"),
                Percentuale = ricorrenti.Percentuale,
            };

            vRealmDb.Add(md);

            trans.Commit();
        }


        internal async static Task InserisciPianificazione(Pianificazione pianificazione, string IDRisorsaAttivita, int IDGiorno, string IDOraInizio, string IDOraFine, string IDEsercente)
        {
            var vRealmDb = await GetRealm();

            RisorsaAttivita risorsaAttivita = vRealmDb.Find<RisorsaAttivita>(IDRisorsaAttivita);
            Giorno giorno = vRealmDb.Find<Giorno>(IDGiorno);
            Esercente esercente = vRealmDb.Find<Esercente>(IDEsercente);

            if (risorsaAttivita == null || IDOraFine == null || IDOraInizio == null) return;

            var trans = vRealmDb.BeginWrite();

            if (IDGiorno == 7)
            {
                for (int i = 0; i <= 6; i++)
                {
                    giorno = vRealmDb.Find<Giorno>(i);
                    Pianificazione md = new Pianificazione
                    {
                        RisorsaAttivita = risorsaAttivita,
                        Esercente = esercente,
                        Giorno = giorno,
                        OraInizio = Convert.ToInt32(IDOraInizio),
                        OraFine = Convert.ToInt32(IDOraFine),
                        Capienza = pianificazione.Capienza
                    };
                    vRealmDb.Add(md);
                }
            }
            else
            {
                Pianificazione md = new Pianificazione
                {
                    RisorsaAttivita = risorsaAttivita,
                    Esercente = esercente,
                    Giorno = giorno,
                    OraInizio = Convert.ToInt32(IDOraInizio),
                    OraFine = Convert.ToInt32(IDOraFine),
                    Capienza = pianificazione.Capienza
                };

            vRealmDb.Add(md);
            }

            trans.Commit();
        }

        internal async static Task InserisciCalendario(Calendario calendario, string IDRisorsaAttivita, string Data, string IDOraInizio, string IDOraFine, string IDEsercente)
        {
            var vRealmDb = await GetRealm();

            RisorsaAttivita risorsaAttivita = vRealmDb.Find<RisorsaAttivita>(IDRisorsaAttivita);
            Esercente esercente = vRealmDb.Find<Esercente>(IDEsercente);

            if (risorsaAttivita == null || IDOraFine == null || IDOraInizio == null || Data == null) return;

            string[] dtAppo = Data.Split("#");

            DateTime DataOK = new DateTime (Convert.ToInt32(dtAppo[0]), Convert.ToInt32(dtAppo[1]), Convert.ToInt32(dtAppo[2]));

            DataOK = DataOK.AddHours(12);

            var trans = vRealmDb.BeginWrite();

            Calendario md = new Calendario
            {
                RisorsaAttivita = risorsaAttivita,
                Esercente = esercente,
                Data = DataOK,
                OraInizio = Convert.ToInt32(IDOraInizio),
                OraFine = Convert.ToInt32(IDOraFine),
                Capienza = calendario.Capienza
            };

            vRealmDb.Add(md);

            trans.Commit();
        }


        internal async static Task AggiornaCalendario(Calendario calendario, string IDRisorsaAttivita, string Data, string IDOraInizio, string IDOraFine, string IDEsercente, string IDCalendario)
        {
            var vRealmDb = await GetRealm();

            RisorsaAttivita risorsaAttivita = vRealmDb.Find<RisorsaAttivita>(IDRisorsaAttivita);
            Esercente esercente = vRealmDb.Find<Esercente>(IDEsercente);
            Calendario cMod = vRealmDb.Find<Calendario>(IDCalendario);


            if (risorsaAttivita == null || IDOraFine == null || IDOraInizio == null || Data == null) return;

            string[] dtAppo = Data.Split("#");

            DateTime DataOK = new DateTime(Convert.ToInt32(dtAppo[0]), Convert.ToInt32(dtAppo[1]), Convert.ToInt32(dtAppo[2]));

            DataOK = DataOK.AddHours(12);

            var trans = vRealmDb.BeginWrite();

            cMod.RisorsaAttivita = risorsaAttivita;
            cMod.Esercente = esercente;
            cMod.Data = DataOK;
            cMod.OraInizio = Convert.ToInt32(IDOraInizio);
            cMod.OraFine = Convert.ToInt32(IDOraFine);
            cMod.Capienza = calendario.Capienza;

            trans.Commit();
        }

        internal async static Task InserisciMasseDebitorieR(string IDrelazioni, MasseDebitorie masseDebitorie, string IDCausaDebito, string IDGrado)
        {
            var vRealmDb = await GetRealm();

            CausaDebito causaDebito = vRealmDb.Find<CausaDebito>(IDCausaDebito);
            Gradi gradi = vRealmDb.Find<Gradi>(IDGrado);
            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            MasseDebitorie md = new MasseDebitorie
            {
                Relazioni = relazioni,
                Creditore = masseDebitorie.Creditore.Replace("'", "`"),
                CausaDebito = causaDebito,
                Nota = masseDebitorie.Nota,
                Grado = gradi,
                Importo = masseDebitorie.Importo,
                PercentualeSoddisfo = masseDebitorie.PercentualeSoddisfo
            };

            vRealmDb.Add(md);

            trans.Commit();
        }


        internal async static Task InserisciAttiDisposizione(string IDproposte, AttiDisposizione attiDisposizione)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            AttiDisposizione ad = new AttiDisposizione
            {
                Proposte = proposte,
                Descrizione = attiDisposizione.Descrizione.Replace("'", "`")
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciAttiDisposizioneR(string IDrelazioni, AttiDisposizione attiDisposizione)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            AttiDisposizione ad = new AttiDisposizione
            {
                Relazioni = relazioni,
                Descrizione = attiDisposizione.Descrizione.Replace("'", "`")
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }



        internal async static Task InserisciPatrimonioImmobiliare(string IDproposte, PatrimonioImmobiliare patrimonioImmobiliare)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            PatrimonioImmobiliare ad = new PatrimonioImmobiliare
            {
                Proposte = proposte,
                Descrizione = patrimonioImmobiliare.Descrizione.Replace("'", "`"),
                Superficie = patrimonioImmobiliare.Superficie,
                ValoreOmiVan = patrimonioImmobiliare.ValoreOmiVan
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciPatrimonioImmobiliareR(string IDrelazioni, PatrimonioImmobiliare patrimonioImmobiliare)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            PatrimonioImmobiliare ad = new PatrimonioImmobiliare
            {
                Relazioni = relazioni,
                Descrizione = patrimonioImmobiliare.Descrizione.Replace("'", "`"),
                Superficie = patrimonioImmobiliare.Superficie,
                ValoreOmiVan = patrimonioImmobiliare.ValoreOmiVan
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciBeniMobiliRegistrati(string IDproposte, BeniMobiliRegistrati beniMobiliRegistrati)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            BeniMobiliRegistrati ad = new BeniMobiliRegistrati
            {
                Proposte = proposte,
                Descrizione = beniMobiliRegistrati.Descrizione.Replace("'", "`"),
                Targa = beniMobiliRegistrati.Targa,
                Stima = beniMobiliRegistrati.Stima
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciBeniMobiliRegistratiR(string IDrelazioni, BeniMobiliRegistrati beniMobiliRegistrati)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            BeniMobiliRegistrati ad = new BeniMobiliRegistrati
            {
                Relazioni = relazioni,
                Descrizione = beniMobiliRegistrati.Descrizione.Replace("'", "`"),
                Targa = beniMobiliRegistrati.Targa,
                Stima = beniMobiliRegistrati.Stima
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciBeniMobili(string IDproposte, BeniMobili beniMobili)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            BeniMobili ad = new BeniMobili
            {
                Proposte = proposte,
                Descrizione = beniMobili.Descrizione.Replace("'", "`"),
                CostoStorico = beniMobili.CostoStorico
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciBeniMobiliR(string IDrelazioni, BeniMobili beniMobili)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            BeniMobili ad = new BeniMobili
            {
                Relazioni = relazioni,
                Descrizione = beniMobili.Descrizione.Replace("'", "`"),
                CostoStorico = beniMobili.CostoStorico
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciSpeseMese(string IDproposte, SpeseMese speseMese)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            SpeseMese ad = new SpeseMese
            {
                Proposte = proposte,
                Descrizione = speseMese.Descrizione.Replace("'", "`"),
                Importo = speseMese.Importo
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciSpeseMeseR(string IDrelazioni, SpeseMese speseMese)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            SpeseMese ad = new SpeseMese
            {
                Relazioni = relazioni,
                Descrizione = speseMese.Descrizione.Replace("'", "`"),
                Importo = speseMese.Importo
            };

            vRealmDb.Add(ad);

            trans.Commit();
        }

        internal async static Task InserisciAnnotazioniAggiuntive(string IDproposte, AnnotazioniAggiuntive annotazioniAggiuntive)
        {
            var vRealmDb = await GetRealm();

            Proposte proposte = vRealmDb.Find<Proposte>(IDproposte);


            var trans = vRealmDb.BeginWrite();

            AnnotazioniAggiuntive ia = new AnnotazioniAggiuntive
            {
                Proposte = proposte,
                Descrizione = annotazioniAggiuntive.Descrizione.Replace("'","`")
            };

            vRealmDb.Add(ia);

            trans.Commit();
        }

        internal async static Task InserisciAnnotazioniAggiuntiveR(string IDrelazioni, AnnotazioniAggiuntive annotazioniAggiuntive)
        {
            var vRealmDb = await GetRealm();

            Relazioni relazioni = vRealmDb.Find<Relazioni>(IDrelazioni);


            var trans = vRealmDb.BeginWrite();

            AnnotazioniAggiuntive ia = new AnnotazioniAggiuntive
            {
                Relazioni = relazioni,
                Descrizione = annotazioniAggiuntive.Descrizione.Replace("'", "`")
            };

            vRealmDb.Add(ia);

            trans.Commit();
        }

        internal async static Task RelazioneDaProposta(string IDProposta, string user)
        {
            var vRealmDb = await GetRealm();

            //seleziono la proposta di riferimento e creo una relazione a partire da questa
            Proposte proposte = vRealmDb.Find<Proposte>(IDProposta);

            var trans = vRealmDb.BeginWrite();

            //Relazione
            Relazioni relazione = new Relazioni
            {
                PropostaRiferimento = proposte,
                User = user,
                Oggetto = proposte.Oggetto,
                Status = proposte.Status,
                Tribunale = proposte.Tribunale,
                SezioneTribunale = proposte.SezioneTribunale,
                LuogoData = proposte.LuogoData,
                CauseIndebitamentoeDiligenza = proposte.CauseIndebitamentoeDiligenza,
                ResocontoPagamenti5Anni = proposte.ResocontoPagamenti5Anni,
                AttoNominaProfessionista = proposte.AttoNominaProfessionista,
                TitoloNominativoProfessionista = proposte.TitoloNominativoProfessionista,
                IndirizzoProfessionista = proposte.IndirizzoProfessionista,
                RecapitiProfessionista = proposte.RecapitiProfessionista,
                RateProposte = proposte.RateProposte,
                RataMax = proposte.RataMax,
                ComponentiNucleo = proposte.ComponentiNucleo,
                RedditoMensile = proposte.RedditoMensile,
                PatrimonioMobProQ = proposte.PatrimonioMobProQ,
                PatrimonioImmobProQ = proposte.PatrimonioImmobProQ,
                DataCreazione = DateTimeOffset.Now,
                DataUltimaModifica = DateTimeOffset.Now
            };

            vRealmDb.Add(relazione);

            //Ricorrenti
            IEnumerable<Ricorrenti> ricorrenti = await ListaRicorrenti(proposte);
            foreach (Ricorrenti ss in ricorrenti)
            {
                Ricorrenti md = new Ricorrenti
                {
                    Relazioni = relazione,
                    Cognome = ss.Cognome,
                    Nome = ss.Nome,
                    CodiceFiscale = ss.CodiceFiscale,
                    Domicilio = ss.Domicilio,
                    Percentuale = ss.Percentuale
                };
                vRealmDb.Add(md);
            }


            //Masse debitorie
            IEnumerable<MasseDebitorie> masseDebitorie = await ListaMasseDebitorie(proposte);
            foreach (MasseDebitorie ss in masseDebitorie)
            {
                MasseDebitorie md = new MasseDebitorie
                {
                    Relazioni = relazione,
                    Creditore = ss.Creditore,
                    CausaDebito = ss.CausaDebito,
                    Nota = ss.Nota,
                    Grado = ss.Grado,
                    Importo = ss.Importo,
                    PercentualeSoddisfo = ss.PercentualeSoddisfo
                };
                vRealmDb.Add(md);
            }

            //Atti di disposizione
            IEnumerable<AttiDisposizione> attiDisposizione = await ListaAttiDisposizione(proposte);
            foreach (AttiDisposizione ss in attiDisposizione)
            {
                AttiDisposizione md = new AttiDisposizione
                {
                    Relazioni = relazione,
                    Descrizione = ss.Descrizione
                };
                vRealmDb.Add(md);
            }

            //Annotazioni
            IEnumerable<AnnotazioniAggiuntive> annotazioniAggiuntive = await ListaAnnotazioniAggiuntive(proposte);
            foreach (AnnotazioniAggiuntive ss in annotazioniAggiuntive)
            {
                AnnotazioniAggiuntive md = new AnnotazioniAggiuntive
                {
                    Relazioni = relazione,
                    Descrizione = ss.Descrizione
                };
                vRealmDb.Add(md);
            }

            //Spese Mese
            IEnumerable<SpeseMese> speseMese = await ListaSpeseMese(proposte);
            foreach (SpeseMese ss in speseMese)
            {
                SpeseMese md = new SpeseMese
                {
                    Relazioni = relazione,
                    Descrizione = ss.Descrizione,
                    Importo = ss.Importo
                };
                vRealmDb.Add(md);
            }

            //Patrimonio Immobiliare
            IEnumerable<PatrimonioImmobiliare> patrimonioImmobiliare = await ListaPatrimonioImmobiliare(proposte);
            foreach (PatrimonioImmobiliare ss in patrimonioImmobiliare)
            {
                PatrimonioImmobiliare md = new PatrimonioImmobiliare
                {
                    Relazioni = relazione,
                    Descrizione = ss.Descrizione,
                    Superficie = ss.Superficie,
                    ValoreOmiVan = ss.ValoreOmiVan
                };
                vRealmDb.Add(md);
            }

            //Beni Mobili Registrati
            IEnumerable<BeniMobiliRegistrati> beniMobiliRegistrati = await ListaBeniMobiliRegistrati(proposte);
            foreach (BeniMobiliRegistrati ss in beniMobiliRegistrati)
            {
                BeniMobiliRegistrati md = new BeniMobiliRegistrati
                {
                    Relazioni = relazione,
                    Descrizione = ss.Descrizione,
                    Targa = ss.Targa,
                    Stima = ss.Stima
                };
                vRealmDb.Add(md);
            }

            //Beni Mobili
            IEnumerable<BeniMobili> beniMobili = await ListaBeniMobili(proposte);
            foreach (BeniMobili ss in beniMobili)
            {
                BeniMobili md = new BeniMobili
                {
                    Relazioni = relazione,
                    Descrizione = ss.Descrizione,
                    CostoStorico = ss.CostoStorico
                };
                vRealmDb.Add(md);
            }

            trans.Commit();
        }



        internal async static Task<string> TestoPrecaricato(string ID)
        {
            var vRealmDb = await GetRealm();

            TestiPrecaricati testiPrecaricati = vRealmDb.All<TestiPrecaricati>().Where(a=> a.Riferimento==ID).First();

            return testiPrecaricati.Testo;

        }


        internal async static Task<IEnumerable<MasseDebitorie>> ListaMasseDebitorie(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<MasseDebitorie> listaOutput = new List<MasseDebitorie>();

            IEnumerable<MasseDebitorie> lista = vRealmDb.All<MasseDebitorie>().Where(ss => ss.Proposte == proposte);
            foreach (MasseDebitorie item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }


        internal async static Task<IEnumerable<Pianificazione>> ListaPianificazioni(Esercente esercente, int IDGiorno, string IDAttivita, string IDRisorsa, bool IsSortGiorno, bool IsSortAttivita, bool IsSortRisorsa)
        {
            var vRealmDb = await GetRealm();

            bool bAtt = false;
            bool bRis = false;
            
            IEnumerable<RisorsaAttivita> risorsaAttivita = null;

            List<Pianificazione> listaOutput = new List<Pianificazione>();

            IEnumerable<Pianificazione> lista = vRealmDb.All<Pianificazione>().Where(ss => ss.Esercente == esercente);

            Attivita attivita = vRealmDb.All<Attivita>().First();

            if (IDGiorno != -1 && IDGiorno != 7)
            {
                Giorno giorno = vRealmDb.Find<Giorno>(IDGiorno);
                lista = vRealmDb.All<Pianificazione>().Where(ss => ss.Esercente == esercente && ss.Giorno == giorno);
            }

            if (IDAttivita != "-1")
            {
                attivita = vRealmDb.Find<Attivita>(IDAttivita);
                risorsaAttivita = vRealmDb.All<RisorsaAttivita>().Where(ss => ss.Attivita == attivita);
                bAtt = true;
            }

            if (IDRisorsa != "-1")
            {
                Risorsa risorsa = vRealmDb.Find<Risorsa>(IDRisorsa);
                if (bAtt)
                {
                    risorsaAttivita = vRealmDb.All<RisorsaAttivita>().Where(ss => ss.Risorsa == risorsa && ss.Attivita == attivita);
                }
                else
                {
                    risorsaAttivita = vRealmDb.All<RisorsaAttivita>().Where(ss => ss.Risorsa == risorsa);
                }

                bRis = true;
            }

            foreach (Pianificazione item in lista)
            {
                if (bAtt || bRis)
                {
                    foreach (RisorsaAttivita ra in risorsaAttivita)
                    {
                        if (item.RisorsaAttivita.ID == ra.ID)
                        {
                            listaOutput.Add(item);
                        }
                    }
                }
                else
                {
                    listaOutput.Add(item);
                }
            }

            lista = listaOutput;

            if (IsSortGiorno)
            {
                lista = lista.OrderBy(a=>a.Giorno.Ordine).ThenBy(a=>a.OraInizio);
            }
            if (IsSortAttivita)
            {
                lista = lista.OrderBy(a => a.RisorsaAttivita.Attivita.Descrizione).ThenBy(a => a.Giorno.Ordine).ThenBy(a => a.OraInizio);
            }
            if (IsSortRisorsa)
            {
                lista = lista.OrderBy(a => a.RisorsaAttivita.Risorsa.Descrizione).ThenBy(a => a.Giorno.Ordine).ThenBy(a => a.OraInizio);
            }

            return lista;
        }

        internal async static Task<IEnumerable<Calendario>> ListaCalendari(Esercente esercente, DateTime dtPrimoGiorno)
        {
            var vRealmDb = await GetRealm();

            DateTime dtUltimoGiorno = dtPrimoGiorno.AddDays(7);

            IEnumerable<Calendario> lista = vRealmDb.All<Calendario>().Where(ss => ss.Data < dtUltimoGiorno && ss.Data >= dtPrimoGiorno && ss.Esercente == esercente).OrderBy(ss=> ss.Data).ThenBy(ss=>ss.OraInizio);

            return lista;
        }


        internal async static Task<IEnumerable<Attivita>> ListaAttivita(Esercente esercente)
        {
            var vRealmDb = await GetRealm();

            List<Attivita> listaOutput = new List<Attivita>();

            IEnumerable<Attivita> lista = vRealmDb.All<Attivita>().Where(ss => ss.Esercente == esercente).OrderBy(a=>a.Descrizione);
            foreach (Attivita item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }


        internal async static Task<IEnumerable<Risorsa>> ListaRisorse(Esercente esercente)
        {
            var vRealmDb = await GetRealm();

            List<Risorsa> listaOutput = new List<Risorsa>();

            IEnumerable<Risorsa> lista = vRealmDb.All<Risorsa>().Where(ss => ss.Esercente == esercente);
            foreach (Risorsa item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }


        internal async static Task<IEnumerable<RisorsaAttivita>> ListaRisorseAttivita(Esercente esercente)
        {
            var vRealmDb = await GetRealm();

            List<RisorsaAttivita> listaOutput = new List<RisorsaAttivita>();

            IEnumerable<RisorsaAttivita> lista = vRealmDb.All<RisorsaAttivita>().Where(ss => ss.Esercente == esercente).OrderBy(a=>a.Attivita.Descrizione);
            foreach (RisorsaAttivita item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }


        internal async static Task<IEnumerable<MasseDebitorie>> ListaMasseDebitorieR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<MasseDebitorie> listaOutput = new List<MasseDebitorie>();

            IEnumerable<MasseDebitorie> lista = vRealmDb.All<MasseDebitorie>().Where(ss => ss.Relazioni == relazioni);
            foreach (MasseDebitorie item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }


        internal async static Task<IEnumerable<AttiDisposizione>> ListaAttiDisposizione(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<AttiDisposizione> listaOutput = new List<AttiDisposizione>();

            IEnumerable<AttiDisposizione> lista = vRealmDb.All<AttiDisposizione>().Where(ss => ss.Proposte == proposte);
            foreach (AttiDisposizione item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<AttiDisposizione>> ListaAttiDisposizioneR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<AttiDisposizione> listaOutput = new List<AttiDisposizione>();

            IEnumerable<AttiDisposizione> lista = vRealmDb.All<AttiDisposizione>().Where(ss => ss.Relazioni == relazioni);
            foreach (AttiDisposizione item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<AnnotazioniAggiuntive>> ListaAnnotazioniAggiuntive(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<AnnotazioniAggiuntive> listaOutput = new List<AnnotazioniAggiuntive>();

            IEnumerable<AnnotazioniAggiuntive> lista = vRealmDb.All<AnnotazioniAggiuntive>().Where(ss => ss.Proposte == proposte);
            foreach (AnnotazioniAggiuntive item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<AnnotazioniAggiuntive>> ListaAnnotazioniAggiuntiveR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<AnnotazioniAggiuntive> listaOutput = new List<AnnotazioniAggiuntive>();

            IEnumerable<AnnotazioniAggiuntive> lista = vRealmDb.All<AnnotazioniAggiuntive>().Where(ss => ss.Relazioni == relazioni);
            foreach (AnnotazioniAggiuntive item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<SpeseMese>> ListaSpeseMese(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<SpeseMese> listaOutput = new List<SpeseMese>();

            IEnumerable<SpeseMese> lista = vRealmDb.All<SpeseMese>().Where(ss => ss.Proposte == proposte);
            foreach (SpeseMese item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<SpeseMese>> ListaSpeseMeseR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<SpeseMese> listaOutput = new List<SpeseMese>();

            IEnumerable<SpeseMese> lista = vRealmDb.All<SpeseMese>().Where(ss => ss.Relazioni == relazioni);
            foreach (SpeseMese item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<PatrimonioImmobiliare>> ListaPatrimonioImmobiliare(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<PatrimonioImmobiliare> listaOutput = new List<PatrimonioImmobiliare>();

            IEnumerable<PatrimonioImmobiliare> lista = vRealmDb.All<PatrimonioImmobiliare>().Where(ss => ss.Proposte == proposte);
            foreach (PatrimonioImmobiliare item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<PatrimonioImmobiliare>> ListaPatrimonioImmobiliareR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<PatrimonioImmobiliare> listaOutput = new List<PatrimonioImmobiliare>();

            IEnumerable<PatrimonioImmobiliare> lista = vRealmDb.All<PatrimonioImmobiliare>().Where(ss => ss.Relazioni == relazioni);
            foreach (PatrimonioImmobiliare item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<BeniMobiliRegistrati>> ListaBeniMobiliRegistrati(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<BeniMobiliRegistrati> listaOutput = new List<BeniMobiliRegistrati>();

            IEnumerable<BeniMobiliRegistrati> lista = vRealmDb.All<BeniMobiliRegistrati>().Where(ss => ss.Proposte == proposte);
            foreach (BeniMobiliRegistrati item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<BeniMobiliRegistrati>> ListaBeniMobiliRegistratiR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<BeniMobiliRegistrati> listaOutput = new List<BeniMobiliRegistrati>();

            IEnumerable<BeniMobiliRegistrati> lista = vRealmDb.All<BeniMobiliRegistrati>().Where(ss => ss.Relazioni == relazioni);
            foreach (BeniMobiliRegistrati item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<BeniMobili>> ListaBeniMobili(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<BeniMobili> listaOutput = new List<BeniMobili>();

            IEnumerable<BeniMobili> lista = vRealmDb.All<BeniMobili>().Where(ss => ss.Proposte == proposte);
            foreach (BeniMobili item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<BeniMobili>> ListaBeniMobiliR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<BeniMobili> listaOutput = new List<BeniMobili>();

            IEnumerable<BeniMobili> lista = vRealmDb.All<BeniMobili>().Where(ss => ss.Relazioni == relazioni);
            foreach (BeniMobili item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<Ricorrenti>> ListaRicorrenti(Proposte proposte)
        {
            var vRealmDb = await GetRealm();

            List<Ricorrenti> listaOutput = new List<Ricorrenti>();

            IEnumerable<Ricorrenti> lista = vRealmDb.All<Ricorrenti>().Where(ss => ss.Proposte == proposte);
            foreach (Ricorrenti item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal async static Task<IEnumerable<Ricorrenti>> ListaRicorrentiR(Relazioni relazioni)
        {
            var vRealmDb = await GetRealm();

            List<Ricorrenti> listaOutput = new List<Ricorrenti>();

            IEnumerable<Ricorrenti> lista = vRealmDb.All<Ricorrenti>().Where(ss => ss.Relazioni == relazioni);
            foreach (Ricorrenti item in lista)
            {
                listaOutput.Add(item);
            }

            return listaOutput;
        }

        internal static async Task<string> ValoriDaCF(string CF)
        {
            var vRealmDb = await GetRealm(true);

            bool Corretto = CDQ.Utils.CodiceFiscale.ControlloFormaleOK(CF);

            if (Corretto)
            {
                //dato un codice fiscale restituisce la data di nascita, il comune e la provincia
                string Istat = CF.Substring(11, 4);
                string Comune;
                string Provincia;
                string DataNascita;
                string Anno;
                string Mese;
                string Giorno;
                string Months = "ABCDEHLMPRST";
                string Sesso = "M";

                ComuniItaliani comita = vRealmDb.All<ComuniItaliani>().Where(a => a.CodiceIstat == Istat).First();

                Comune = comita.Comune;
                Provincia = comita.SiglaProvincia;
                Anno = CF.Substring(6, 2);
                Mese = (Months.IndexOf(CF.Substring(8, 1)) + 1).ToString();
                Giorno = CF.Substring(9, 2);
                if (Int32.Parse(Anno) > 30)
                {
                    Anno = (Int32.Parse(Anno) + 1900).ToString();
                }
                else
                {
                    Anno = (Int32.Parse(Anno) + 2000).ToString();
                }

                if (Int32.Parse(Mese) < 10) Mese = "0" + Mese;

                if (Int32.Parse(CF.Substring(9, 2)) > 40)
                {
                    Giorno = (Int32.Parse(Giorno) - 40).ToString();
                    Sesso = "F";
                }

                DataNascita = Anno + "-" + Mese + "-" + Giorno;

                return Comune.ToUpper() + ";" + Provincia + ";" + DataNascita + ";" + Sesso;
            }
            else return ";;;";
        }

       

    }
}
