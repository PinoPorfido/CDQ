using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using CDQ.Models;
using CDQ.Models.Helper;
using CDQ.Hubs;
using CDQ.Services;
using CDQ.Utils;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using Microsoft.Office.Interop.Word;
using Microsoft.AspNetCore.Diagnostics;

namespace CDQ.Controllers
{

    struct Elenco
    {
        public string Elemento;
        public int Totale;
    }

    public class HomeController : Controller
    {

        private readonly IHubContext<ProgressHub> _progressHubContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IHubContext<ProgressHub> progressHubContext, IWebHostEnvironment hostingEnvironment)
        {
            _progressHubContext = progressHubContext;
            _hostingEnvironment = hostingEnvironment;
        }


        public bool CheckUser()
        {
            string username = HttpContext.Session.GetString("username");

            string webRootPath = _hostingEnvironment.WebRootPath;

            if (username == null)
            {
                HttpContext.Session.Clear();
                return false;
            }
            return true;
        }

        public bool CheckRoleAgente()
        {
            return (HttpContext.Session.GetString("role") == "agente");
        }

        public bool CheckRoleAdmin()
        {
            return (HttpContext.Session.GetString("role") == "admin");

        }

        public bool CheckRoleUtente()
        {
            return (HttpContext.Session.GetString("role") == "utente");
        }


        public async Task<JsonResult> GetEvents()
        {
            IEnumerable<Calendar> events = null;

            //events = RealmDataStore.ListaEventi();

            return new JsonResult(events);
        }


        public bool CheckVisibilitaAgente(string IDAgente)
        {
            string ID = HttpContext.Session.GetString("idagente");
            bool admin = HttpContext.Session.GetString("role") == "admin";

            if (admin) return true;

            return true;

        }

        public string MaxIDAgenteView()
        {
            if (CheckRoleAgente()) return HttpContext.Session.GetString("idagente");
            return null;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            return View();
        }

        //[HttpGet]
        //public IActionResult Calendario()
        //{
        //    if (!CheckUser()) return RedirectToAction(nameof(Login));
        //    if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

        //    return View();
        //}


        [HttpGet]
        public async Task<IActionResult>Dashboard()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            //Grafico 0 - Associati
            List<string> listaLabels0 = new List<string> { "Proposte", "Relazioni"};
            List<string> listaColors0 = new List<string> { HelpChart.Color(0), HelpChart.Color(1) };
            List<string> listaValues0 = new List<string> { "130", "95" };

            //List<string> listaValues0 = new List<string>();

            //var associati = await RealmDataStore.ListaAssociati(HttpContext.Session.GetString("provinciale"));

            //listaValues0.Add(associati.Where(a => a.IsLavoratorePensionato == false && a.IsPrivato == false).Count().ToString());
            //listaValues0.Add(associati.Where(a => a.IsLavoratorePensionato == true && a.IsPrivato == false).Count().ToString());
            //listaValues0.Add(associati.Where(a => a.IsLavoratorePensionato == false && a.IsPrivato == true).Count().ToString());

            //Grafico 1 - Agenti
            List<string> listaLabels1 = new List<string> { "In Lavorazione", "Presentate","In Revisione","Accettate", "Respinte"};
            List<string> listaColors1 = new List<string> { HelpChart.Color(0), HelpChart.Color(1), HelpChart.Color(2), HelpChart.Color(3), HelpChart.Color(4) };
            List<string> listaValues1 = new List<string> { "34", "46", "15", "54", "24" };

            //var agenti = await RealmDataStore.ListaAgenti(true,HttpContext.Session.GetString("provinciale"));

            //listaValues1.Add(agenti.Where(a => a.Ruolo.Codice=="MT").Count().ToString());
            //listaValues1.Add(agenti.Where(a => a.Ruolo.Codice == "FTZONA").Count().ToString());
            //listaValues1.Add(agenti.Where(a => a.Ruolo.Codice == "TUT").Count().ToString());
            //listaValues1.Add(agenti.Where(a => a.Ruolo.Codice == "COM").Count().ToString());
            //listaValues1.Add(agenti.Where(a => a.Ruolo.Codice == "CE").Count().ToString());

            //Grafico 2 - Andamento Provinciale

            List<string> listaLabels2 = new List<string> { "nov 2019", "dic 2019", "gen 2020", "feb 2020", "mar 2020", "apr 2020" };
            List<string> listaColors2 = new List<string> { HelpChart.Color(0), HelpChart.Color(1), HelpChart.Color(2), HelpChart.Color(3), HelpChart.Color(4), HelpChart.Color(5) };
            List<string> listaValues2 = new List<string> { "15", "22", "16", "25", "29", "30" };


            //List<string> listaLabels2 = new List<string>();
            //List<string> listaColors2 = new List<string>();
            //List<string> listaValues2 = new List<string>();

            //Agente agenteProvinciale = await RealmDataStore.Agente(HttpContext.Session.GetString("provinciale"));
            //DateTime dt;

            //for (int i=5;i>=0;i--)
            //{
            //    dt = DateTime.Now.AddMonths(-i);
            //    string mese = dt.ToString("MMM", CultureInfo.CreateSpecificCulture("it"));
            //    listaLabels2.Add(mese+" "+dt.Year);
            //    listaColors2.Add(HelpChart.Color(i));
            //    double s = 100; //await RealmDataStore.SommaDovuto(agenteProvinciale, dt.Month, dt.Year, dt.Month, dt.Year);
            //    listaValues2.Add(Math.Round(s).ToString());
            //}

            //Servizi Erogati 6 mesi
            List<string> listaLabels3 = new List<string> { "nov 2019", "dic 2019", "gen 2020", "feb 2020", "mar 2020", "apr 2020" };
            List<string> listaColors3 = new List<string> { HelpChart.Color(0), HelpChart.Color(1), HelpChart.Color(2), HelpChart.Color(3), HelpChart.Color(4), HelpChart.Color(5) };
            List<string> listaValues3 = new List<string> { "223", "167", "176", "259", "239", "305" };


            //for (int i = 5; i >= 0; i--)
            //{
            //    dt = DateTime.Now.AddMonths(-i);
            //    string mese = dt.ToString("MMM", CultureInfo.CreateSpecificCulture("it"));
            //    listaLabels3.Add(mese + " " + dt.Year);
            //    listaColors3.Add(HelpChart.Color(i));
            //    int n = await RealmDataStore.NumeroServizi(HttpContext.Session.GetString("provinciale"), dt.Month, dt.Year);
            //    double s = await RealmDataStore.TotaleServizi(HttpContext.Session.GetString("provinciale"), dt.Month, dt.Year);

            //    listaValues3.Add(n.ToString());
            //}


            //Grafico 2 - Andamento Tutor trimestre

            //List<string> listaLabels4 = new List<string>();
            //List<string> listaColors4 = new List<string>();
            //List<string> listaValues4 = new List<string>();
            //List<string> listaIDs4 = new List<string>();

            //DateTime dt1 = DateTime.Now.Date.AddMonths(-2);
            //DateTime dt2 = DateTime.Now.Date;


            //var listaAgenti = await RealmDataStore.ListaAgenti(false, HttpContext.Session.GetString("provinciale"));
            //int c = 0;
            //foreach (Agente agente in listaAgenti)
            //{
            //    if (agente.ID == agenteProvinciale.ID || agente.Ruolo.Codice=="FTNAZ") continue;

            //    double saldo = 100; //await RealmDataStore.SommaDovuto(agente, dt1.Month, dt1.Year, dt2.Month, dt2.Year);
            //    if(saldo>1)
            //    {
            //        listaLabels4.Add(agente.Nominativo);
            //        listaIDs4.Add(agente.ID);
            //        listaColors4.Add(HelpChart.Color(c));
            //        c = (c + 1) % 6;
            //        listaValues4.Add(Math.Round(saldo).ToString());
            //    }                

            //}          

            ////

            HelpChart helpChart = new HelpChart
            {
                Colors = new List<string>[] { listaColors0, listaColors1, listaColors2, listaColors3 },
                Labels = new List<string>[] { listaLabels0, listaLabels1, listaLabels2, listaLabels3 },
                Values = new List<string>[] { listaValues0, listaValues1, listaValues2, listaValues3 },
                IDs    = new List<string>[] { null,         null,         null,         null    }
            };

            return View(helpChart);
        }


        public async Task<IActionResult> AllineaRealm()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.GetRealm();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult UnderConstruction()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            return View();
        }



        [HttpGet]
        public async Task<IActionResult> EditEsercente(string ID, bool Upd)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            List<SelectListItem> ListaOrari = new List<SelectListItem>();
            for (int i = 6 ; i <= 48 ; i++)
            {
                SelectListItem si = new SelectListItem { Text = Utils.Utils.ConvertiOrario(i * 30) + "", Value = i*30 + "" };
                ListaOrari.Add(si);
            }

            List<SelectListItem> ListaSlot = new List<SelectListItem>();
            for (int i = 1; i <= 8; i++)
            {
                SelectListItem si = new SelectListItem { Text = Utils.Utils.ConvertiOrario(i * 15) + "", Value = i * 15 + "" };
                ListaSlot.Add(si);
            }



            List<SelectListItem> ListaCategorie = new List<SelectListItem>();
            var listaC = await RealmDataStore.ListaCategorie();
            foreach (Categoria c in listaC)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaCategorie.Add(item);
            }

            Utente utente = await RealmDataStore.Utenti(HttpContext.Session.GetString("username"));

            Esercente esercente = await RealmDataStore.Esercente(utente.Esercente.ID);

            HelpEsercente helpEsercente = new HelpEsercente
            {
                Esercente = esercente,
                Upd = Upd,
                IDCategoria = esercente.Categoria == null ? "-1" : esercente.Categoria.ID,
                ListaCategorie = ListaCategorie,
                ListaOrari = ListaOrari,
                ListaSlot = ListaSlot,
                IDOraInizio = esercente.OraInizio.ToString(),
                IDOraFine = esercente.OraFine.ToString(),
                IDSlot = esercente.MinutiSlot.ToString()

            };

            return View(helpEsercente);
        }


        [HttpPost]
        public async Task<IActionResult> EditEsercente(HelpEsercente helpEsercente)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (!ModelState.IsValid) return View(helpEsercente);

            Esercente esercente = helpEsercente.Esercente;

            await RealmDataStore.AggiornaEsercente(esercente, helpEsercente.IDCategoria, helpEsercente.IDOraInizio, helpEsercente.IDOraFine, helpEsercente.IDSlot);

            return RedirectToAction(nameof(EditEsercente));
        }


        [HttpPost]
        public async Task<IActionResult> Calendario(HelpCalendario helpCalendario)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (!ModelState.IsValid) return View(helpCalendario);

            Calendario calendario = helpCalendario.Calendario;

            if (helpCalendario.ModeCalendario == "Ins")
            {
                await RealmDataStore.InserisciCalendario(calendario, helpCalendario.IDRisorsaAttivita, helpCalendario.IDGiorno, helpCalendario.IDOraInizio, helpCalendario.IDOraFine, HttpContext.Session.GetString("IDesercente"));
            }
            else if (helpCalendario.ModeCalendario == "Upd")
            {
                await RealmDataStore.AggiornaCalendario(calendario, helpCalendario.IDRisorsaAttivita, helpCalendario.IDGiorno, helpCalendario.IDOraInizio, helpCalendario.IDOraFine, HttpContext.Session.GetString("IDesercente"), helpCalendario.IDCalendarioU);
            }

            return RedirectToAction(nameof(Calendario), new { IDSettimana = helpCalendario.IDSettimanaC, Anno = helpCalendario.Anno });
        }

        [HttpPost]
        public async Task<IActionResult> Pianificazione(HelpPianificazione helpPianificazione)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (!ModelState.IsValid) return View(helpPianificazione);

            Pianificazione pianificazione = helpPianificazione.Pianificazione;

            if (helpPianificazione.Mode == "Ins")
            {
                await RealmDataStore.InserisciPianificazione(pianificazione, helpPianificazione.IDRisorsaAttivita, helpPianificazione.IDGiorno, helpPianificazione.IDOraInizio, helpPianificazione.IDOraFine, HttpContext.Session.GetString("IDesercente"));
            }
            else if (helpPianificazione.Mode == "Upd" && helpPianificazione.IDGiorno != 7)
            {
                await RealmDataStore.AggiornaPianificazione(pianificazione, helpPianificazione.IDRisorsaAttivita, helpPianificazione.IDGiorno, helpPianificazione.IDOraInizio, helpPianificazione.IDOraFine, HttpContext.Session.GetString("IDesercente"));
            }

            return RedirectToAction(nameof(Pianificazione));
        }



        [HttpGet]
        public IActionResult Login()
        {
            if(CheckUser()) { return RedirectToAction(nameof(Index));};

            HelpUtente helpUtente = new HelpUtente
            {
                Utente = new Utente { }
            };

            return View(helpUtente);
        }

        [HttpGet]
        public IActionResult Password()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            HelpUtente helpUtente = new HelpUtente
            {
                Utente = new Utente { }
            };

            return View(helpUtente);
        }

        public IActionResult Error()
        {
            var pathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            Exception exception = pathFeature?.Error; // Here will be the exception details.

            ErrorViewModel evm = new ErrorViewModel
            {
                Message = exception.ToString()
            };

            return View(evm);
        }



        [HttpPost]
        public async Task<IActionResult> Login(HelpUtente helpUtente)
        {

            Utente utenti = await RealmDataStore.CheckUtente(helpUtente.Utente);

            await RealmDataStore.AggiornaLogin(helpUtente.Utente.Mail);

            //test commit

            if (helpUtente.Utente.Mail==null || helpUtente.Utente.Mail.Length==0 || utenti == null)
            {
                helpUtente.Message = "Username/Password errati";
            }
            else
            {

                int RuoliUtente = await RealmDataStore.CheckRuoli(utenti);

                HttpContext.Session.SetString("username", utenti.Mail);

                if (utenti.Esercente != null)
                {
                    HttpContext.Session.SetString("IDesercente", utenti.Esercente.ID);
                }
                else
                {
                    HttpContext.Session.SetString("IDesercente", "-");
                }

                string RU = Convert.ToString(RuoliUtente, 2);
                byte nR = 0;

                if (RU == "0") RU = "0000";

                if (RU.Length < 4) RU = Utils.Utils.AggiungiStringa(Convert.ToInt32(RU), "0", 4);

                string ruolo = "";

                if (RU.Substring(0, 1) == "1") ruolo += "#CLI"; nR += 1;
                if (RU.Substring(1, 1) == "1") ruolo += "#ESE"; nR += 1;
                if (RU.Substring(2, 1) == "1") ruolo += "#SYS"; nR += 1;
                if (RU.Substring(3, 1) == "1") ruolo += "#ADM"; nR += 1;

                HttpContext.Session.SetString("ruolo", ruolo);
                HttpContext.Session.SetString("nominativo", utenti.Nome + " " + utenti.Cognome);
                HttpContext.Session.SetString("role", RuoliUtente.ToString());
                HttpContext.Session.SetString("user", utenti.Mail);
                HttpContext.Session.SetString("currentlogin", utenti.DataUltimoLogin_OUT);
                HttpContext.Session.SetString("lastlogin", utenti.DataPenultimoLogin_OUT);

                //a seconda del ruolo abilitiamo le pagine 
                //if (AndBinario(RuoliUtente,Settings.PROPONENTE)) return RedirectToAction(nameof(SchedaAgente), new { utenti.Username });
                //if (AndBinario(RuoliUtente, Settings.RELATORE)) return RedirectToAction(nameof(SchedaAgente), new { utenti.Username });

                //if (nR == 4 && ruolo == "#CLI") return RedirectToAction(nameof(Proposte), new { utenti.Mail });

                //if (HttpContext.Session.GetString("IDEsercenteUtente") != null)
                //{
                //    //devo chiamare direttamente la view CalendarioUtente
                //    string IDEse = HttpContext.Session.GetString("IDEsercenteUtente");
                //    HttpContext.Session.SetString("IDEsercenteUtente", "");
                //    return RedirectToAction(nameof(CalendarioUtente), new { IDEse });
                //}

                return RedirectToAction(nameof(Index));

            }

            return View(helpUtente);

        }


        [HttpPost]
        public async Task<IActionResult> Password(HelpUtente helpUtente)
        {

            Utente utenti = await RealmDataStore.CheckUtente(helpUtente.Utente);

            if (helpUtente.Utente.Mail == null || helpUtente.Utente.Mail.Length == 0 || utenti == null)
            {
                helpUtente.Message = "Utente non riconosciuto";
            }
            else
            {
                string res = await RealmDataStore.AggiornaPassword(helpUtente.Utente.Mail, helpUtente.Utente.Password, helpUtente.NewPassword, helpUtente.ConfirmNewPassword);

                helpUtente.Message = res;

            }

            return View(helpUtente);

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await RealmDataStore.LogOut();
            HttpContext.Session.Clear();
            ViewData["username"] = "";
            return RedirectToAction(nameof(Login));
        }



        public async Task<IActionResult> AttivitaRisorse(string ID = null, int tab = 1)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            IEnumerable<Attivita> ListaAttivita = null;
            IEnumerable<Risorsa> ListaRisorse = null;
            IEnumerable<RisorsaAttivita> ListaRisorseAttivita = null;

            Esercente esercente = await RealmDataStore.Esercente(HttpContext.Session.GetString("IDesercente"));

            ListaAttivita = await RealmDataStore.ListaAttivita(esercente);
            List<SelectListItem> ListaAttivit = new List<SelectListItem>();
            foreach (Attivita c in ListaAttivita)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaAttivit.Add(item);
            }

            ListaRisorse = await RealmDataStore.ListaRisorse(esercente);
            List<SelectListItem> ListaRisors = new List<SelectListItem>();
            foreach (Risorsa c in ListaRisorse)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaRisors.Add(item);
            }

            ListaRisorseAttivita = await RealmDataStore.ListaRisorseAttivita(esercente);

            HelpAttivitaRisorse helpAttivitaRisorse = new HelpAttivitaRisorse
            {
                Esercente = esercente,
                ListaAttivita = ListaAttivita,
                ListaRisorse = ListaRisorse,
                ListaRisorseAttivita = ListaRisorseAttivita,
                Ins = (ID == null),
                Tab = tab,
                ListaAttivit = ListaAttivit,
                ListaRisors = ListaRisors
            };

            return View(helpAttivitaRisorse);
        }



        [HttpGet]
        public async Task<IActionResult> SchedaAgente(string ID, int tab = 0, int mese = -1, int anno = -1, bool IsPosizioniAperte = false)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (!CheckVisibilitaAgente(ID)) return RedirectToAction(nameof(Login));

            //la variabile bPosAperte dice che dobbiamo prendere tutte le operazioni 
            //aperte (ovvero non pagate o pagate parzialmente) indipendentemente dal mese/anno
                        
            
            double Versato = 0;
            byte daPagare = 0;

            string[] words = ID.Split('+');
            if (words.Length > 1)
            {
                ID = words[1];
            }
               
            //var listaSA = await RealmDataStore.ListaStrutturaAgente(MaxIDAgenteView(), ID);

            if (mese == -1)
            {
                anno = DateTime.Now.Year;
                mese = DateTime.Now.Month;
            }

            //Agente agente = await RealmDataStore.Agente(ID);

            
            //Caricamento oggetto complesso ListaServiziAgente
            //var listaConServizi = await RealmDataStore.ListaConsulenzeAgente(agente, mese, anno, IsPosizioniAperte);
            //var listaSpeServizi = await RealmDataStore.ListaSpeseAgente(agente, mese, anno, IsPosizioniAperte);
            //var listaRipartizioni = await RealmDataStore.ListaRipartizioniAgente(agente, mese, anno, IsPosizioniAperte);


            //List<ListaServiziAgente> listaServizi = new List<ListaServiziAgente>();

            //primo ciclo: le consulenze
            //foreach (ConsulenzaServizio ag in listaConServizi)
            //{
            //    //determina il residuo da pagare
            //    Versato = await RealmDataStore.CalcolaVersato("CS", ag.ID);
            //    if (ag.ImportoNetto - Versato == 0 && ag.ImportoNetto != 0) daPagare = 2;
            //    else if (ag.ImportoNetto - Versato > 0 && Versato > 0 ) daPagare = 1;
            //    else if (Versato == 0) daPagare = 0;

            //    //if ((IsPosizioniAperte && daPagare != 2) || !IsPosizioniAperte)
            //    //{
            //    //    ListaServiziAgente LAS = new ListaServiziAgente
            //    //    {
            //    //        Data = ag.Data,
            //    //        ConsulenzaServizio = ag,
            //    //        SpesaServizio = null,
            //    //        Ripartizione = null,
            //    //        nPagato = daPagare,
            //    //        Residuo = ag.ImportoNetto - Versato,
            //    //        IsSelezionato = false
            //    //    };

            //    //    listaServizi.Add(LAS);
            //    //}
            //}

            //secondo ciclo: le spese
            //foreach (SpesaServizio ag in listaSpeServizi)
            //{
            //    //determina il residuo da pagare
            //    Versato = await RealmDataStore.CalcolaVersato("SS", ag.ID);
            //    if (ag.Importo - Versato == 0 && ag.Importo != 0) daPagare = 2;
            //    else if (ag.Importo - Versato > 0 && Versato > 0) daPagare = 1;
            //    else if (Versato == 0) daPagare = 0;

            //    //if ((IsPosizioniAperte && daPagare != 2) || !IsPosizioniAperte)
            //    //{
            //    //    ListaServiziAgente LAS = new ListaServiziAgente
            //    //    {
            //    //        Data = ag.Data,
            //    //        ConsulenzaServizio = null,
            //    //        SpesaServizio = ag,
            //    //        Ripartizione = null,
            //    //        nPagato = daPagare,
            //    //        Residuo = ag.Importo - Versato,
            //    //        IsSelezionato = false
            //    //    };

            //    //    listaServizi.Add(LAS);
            //    //}
            //}
            //terzo ciclo: le ripartizioni
            //foreach (Ripartizione ag in listaRipartizioni)
            //{
            //    //determina il residuo da pagare
            //    Versato = await RealmDataStore.CalcolaVersato("RI", ag.ID);
            //    if (ag.Importo - Versato == 0 && ag.Importo != 0) daPagare = 2;
            //    else if (ag.Importo - Versato > 0 && Versato > 0) daPagare = 1;
            //    else if (Versato == 0) daPagare = 0;

            //    //if ((IsPosizioniAperte && daPagare != 2) || !IsPosizioniAperte)
            //    //{
            //    //    ListaServiziAgente LAS = new ListaServiziAgente
            //    //    {
            //    //        Data = ag.Data,
            //    //        ConsulenzaServizio = null,
            //    //        SpesaServizio = null,
            //    //        Ripartizione = ag,
            //    //        nPagato = daPagare,
            //    //        Residuo = ag.Importo - Versato,
            //    //        IsSelezionato = false
            //    //    };

            //    //    listaServizi.Add(LAS);
            //    //}
            //}


            //var elencoAssociatiServizio = new List<string>();

            //foreach(ConsulenzaServizio cs in listaConServizi)
            //{
            //    string elenco = await RealmDataStore.ElencoAssociatiServizio(cs.Servizio, HttpContext.Session.GetString("provinciale"));

            //    elencoAssociatiServizio.Add(elenco);
            //}

            //foreach (SpesaServizio cs in listaSpeServizi)
            //{
            //    string elenco = await RealmDataStore.ElencoAssociatiServizio(cs.Servizio, HttpContext.Session.GetString("provinciale"));

            //    elencoAssociatiServizio.Add(elenco);
            //}

            //var listaPagamentiAgente = await RealmDataStore.ListaPagamentiAgente(agente, mese, anno, IsPosizioniAperte);

            //var listaRipartizioniTotali = await RealmDataStore.ListaRipartizioniAgente(agente);
            //var listaPagamentiAgenteTotali = await RealmDataStore.ListaPagamentiAgente(agente);

            //var listaSubAgenti = await RealmDataStore.ListaSubAgenti(agente);
            //var listaAssociati = await RealmDataStore.ListaAssociatiPerAgente(agente, HttpContext.Session.GetString("provinciale"));
            //var listaLink = new List<LinkAngente>();
            //foreach (Agente ag in listaSA)
            //{
            //    listaLink.Insert(0, new LinkAngente
            //    {
            //        ID = ag.ID,
            //        Nominativo = ag.Nominativo

            //    });
            //}

            List<SelectListItem> listaMesi = new List<SelectListItem>();
            var listaM = await RealmDataStore.ListaMesi();
            foreach (Mese m in listaM)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = m.Nome,
                    Value = m.ID + ""
                };

                listaMesi.Add(item);
            }

            List<SelectListItem> listaAnni = new List<SelectListItem>();
            for (int i = 2015; i <= DateTime.Now.Year + 1; i++)
            {
                SelectListItem si = new SelectListItem { Text = i + "", Value = i + "" };
                listaAnni.Add(si);
            }

            Mese ms = await RealmDataStore.Mese(mese);

            //listaAssociati = listaAssociati.OrderBy(a => a.NSocio).ThenBy(a => a.Denominazione);

            HelpSchedaAgente helpSchedaAgente = new HelpSchedaAgente
            {
                //Agente = agente,
                //ElencoAssociatiServizio = elencoAssociatiServizio,
                //ListaServizi = listaServizi,
                //ListaRipartizioni = listaRipartizioni,
                //ListaPagamentiAgente = listaPagamentiAgente,
                //ListaRipartizioniTotali = listaRipartizioniTotali,
                //ListaPagamentiAgenteTotali = listaPagamentiAgenteTotali,
                //ListaSubAgenti = listaSubAgenti,
                //ListaAssociati = listaAssociati,
                ListaMesi = listaMesi,
                ListaAnni = listaAnni,
                //ListaLink = listaLink,
                Anno = anno,
                Mese = mese,
                DescMese = ms.Nome,
                Tab = tab,
                IsAdmin = CheckRoleAdmin(),
                IsPosizioniAperte = IsPosizioniAperte
            };

            //PagamentoAgente pagamentoAgente = new PagamentoAgente
            //{
            //    Data = DateTime.Now,
            //    Importo = helpSchedaAgente.Saldo < 0 ? -helpSchedaAgente.Saldo : 0
            //};

            //helpSchedaAgente.PagamentoAgente = pagamentoAgente;

            return View(helpSchedaAgente);
        }

        [HttpGet]
        public async Task<IActionResult> Backup()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.Backup();

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Export()
        {

            //var listac = await RealmDataStore.ListaCausali();
            //Console.WriteLine(listac.First().Head);
            //foreach (Causale x in listac)
            //{
            //    Console.WriteLine(x.Content);
            //}

            var listam = await RealmDataStore.ListaMesi();
            Console.WriteLine(listam.First().Head);
            foreach (Mese x in listam)
            {
                Console.WriteLine(x.Content);
            }

            //var listan = await RealmDataStore.ListaNatureGiuridiche();
            //Console.WriteLine(listan.First().Head);
            //foreach (NaturaGiuridica x in listan)
            //{
            //    Console.WriteLine(x.Content);
            //}

            //var listata = await RealmDataStore.ListaTipiAttivita();
            //Console.WriteLine(listata.First().Head);
            //foreach (TipoAttivita x in listata)
            //{
            //    Console.WriteLine(x.Content);
            //}

            //var listatp = await RealmDataStore.ListaTipiPagamento();
            //Console.WriteLine(listatp.First().Head);
            //foreach (TipoPagamento x in listatp)
            //{
            //    Console.WriteLine(x.Content);
            //}

            //var listar = await RealmDataStore.ListaRuoli();
            //Console.WriteLine(listar.First().Head);
            //foreach (Ruolo x in listar)
            //{
            //    Console.WriteLine(x.Content);
            //}

            //var listaag = await RealmDataStore.ListaAgenti(false, HttpContext.Session.GetString("provinciale"), true);
            //Console.WriteLine(listaag.First().Head);
            //foreach (Agente x in listaag)
            //{
            //    Console.WriteLine(x.Content);
            //}

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Pianificazione(string ID = "-1", int IDGiornoR = -1, string IDAttivita = "-1", string IDRisorsa = "-1", bool IsSortGiorno = false, bool IsSortAttivita = false, bool IsSortRisorsa = false)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            int intOI = 480;
            int intOF = 1200;
            int intSlot = 30;
            double dFattore = 0;

            List<SelectListItem> ListaGiorni = new List<SelectListItem>();

            //aggiunta selezione completa
            SelectListItem itemx = new SelectListItem
            {
                Text = "Tutti i giorni",
                Value = 7 + ""
            };
            ListaGiorni.Add(itemx);

            var listaG = await RealmDataStore.ListaGiorni();
            foreach (Giorno c in listaG)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Nome,
                    Value = c.ID + ""
                };

                ListaGiorni.Add(item);
            }

            Esercente esercente = await RealmDataStore.Esercente(HttpContext.Session.GetString("IDesercente"));

            if (esercente.OraInizio != 0) intOI = esercente.OraInizio;
            if (esercente.OraFine != 0) intOF = esercente.OraFine;
            if (esercente.MinutiSlot != 0) intSlot = esercente.MinutiSlot;

            dFattore = 60 / intSlot;

            List<SelectListItem> ListaOrari = new List<SelectListItem>();
            for (int i = Convert.ToInt32(intOI*dFattore/60); i <= Convert.ToInt32(intOF*dFattore/60); i++)
            {
                SelectListItem si = new SelectListItem { Text = Utils.Utils.ConvertiOrario(i * intSlot) + "", Value = i * intSlot + "" };
                ListaOrari.Add(si);
            }

            List<SelectListItem> ListaAttivita = new List<SelectListItem>();
            var listaA = await RealmDataStore.ListaAttivita(esercente);
            foreach (Attivita c in listaA)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaAttivita.Add(item);
            }

            List<SelectListItem> ListaRisorse = new List<SelectListItem>();
            var ListaR = await RealmDataStore.ListaRisorse(esercente);
            foreach (Risorsa c in ListaR)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaRisorse.Add(item);
            }

            List<SelectListItem> ListaRisorseAttivita = new List<SelectListItem>();
            List<SelectListItem> ListaRisorseAttivitaCapienza = new List<SelectListItem>();
            var ListaRA = await RealmDataStore.ListaRisorseAttivita(esercente);
            foreach (RisorsaAttivita c in ListaRA)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Attivita.Descrizione + "-" + c.Risorsa.Descrizione,
                    Value = c.ID + ""
                };

                ListaRisorseAttivita.Add(item);

                SelectListItem itemc = new SelectListItem
                {
                    Text = c.Capienza + "",
                    Value = c.ID + ""
                };

                ListaRisorseAttivitaCapienza.Add(itemc);
            }


            var ListaPianificazioni = await RealmDataStore.ListaPianificazioni(esercente, IDGiornoR, IDAttivita, IDRisorsa, IsSortGiorno, IsSortAttivita, IsSortRisorsa);


            Pianificazione pianificazione = new Pianificazione
            {
                Capienza = 1 
            };


            if (ID != "-1")
            {
                pianificazione = await RealmDataStore.Pianificazione(ID);
            }


            HelpPianificazione helpPianificazione = new HelpPianificazione
            {
                ListaOrari = ListaOrari,
                Mode = "Ins",
                Pianificazione = pianificazione,
                IDGiorno = pianificazione.Giorno == null ? 7 : pianificazione.Giorno.ID,
                IDRisorsaAttivita = pianificazione.RisorsaAttivita == null ? "-1" : pianificazione.RisorsaAttivita.ID,
                ListaAttivita = ListaAttivita,
                ListaGiorni = ListaGiorni,
                ListaPianificazioni = ListaPianificazioni,
                ListaRisorse = ListaRisorse,
                ListaRisorseAttivita = ListaRisorseAttivita,
                ListaRisorseAttivitaCapienza = ListaRisorseAttivitaCapienza,
                IDGiornoR = 7,
                IsSortAttivita = false,
                IsSortGiorno = false,
                IsSortRisorsa = false,
                Esercente = esercente.RagioneSociale
            };

            return View(helpPianificazione);
        }


        private List<Elenco> AggiungiElemento(List<Elenco> RisAtt, string Ele)
        {

            var itemIndex = RisAtt.FindIndex(z => z.Elemento == Ele);
            if (itemIndex != -1)
            {
                var item = RisAtt.ElementAt(itemIndex);
                int tot = item.Totale;
                tot += 1;
                RisAtt.RemoveAt(itemIndex);
                Elenco Appo;
                Appo.Elemento = Ele;
                Appo.Totale = tot;
                RisAtt.Add(Appo);
            }
            else
            {
                Elenco Appo;
                Appo.Elemento = Ele;
                Appo.Totale = 1;
                RisAtt.Add(Appo);
            }


            return RisAtt;
        }

        [HttpGet]
        public async Task<IActionResult> Calendario(int IDSettimana = -1, int Anno = -1, string Dir = "", bool IsNascosto=false, string IDRisorsaAttivitaR="-1", bool IsRefresh=false)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            DateTime dtPrimoGiornoSettimana = DateTime.Now;
            DateTime dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);

            List<Elenco> RisAtt = new List<Elenco>();

            string HasConflict = "";

            if (IDSettimana == -1 || Dir == "oggi") IDSettimana = Utils.Utils.SettimanaAnno(DateTime.Now); 
            if (Anno == -1) Anno = DateTime.Now.Year;

            dtPrimoGiornoSettimana = Utils.Utils.PrimoGiornoSettimana(Anno, IDSettimana);
            dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);

            if (Dir == "less") //devo tornare indietro di una settimana
            {
                dtPrimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(-7);
                dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);
            }

            if (Dir == "more") //devo tornare indietro di una settimana
            {
                dtPrimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(7);
                dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);
            }

            IDSettimana = Utils.Utils.SettimanaAnno(dtPrimoGiornoSettimana);
            Anno = dtPrimoGiornoSettimana.Year;

            string Settimana = dtPrimoGiornoSettimana.ToShortDateString() + "-" + dtUltimoGiornoSettimana.ToShortDateString();

            List<SelectListItem> ListaGiorni = new List<SelectListItem>();

            List<DateTime> listaG = new List<DateTime>();
            listaG.Add(dtPrimoGiornoSettimana);
            listaG.Add(dtPrimoGiornoSettimana.AddDays(1));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(2));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(3));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(4));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(5));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(6));


            foreach (DateTime c in listaG)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.ToShortDateString(),
                    Value = c.Year + "#" + c.Month + "#" + c.Day
                };

                ListaGiorni.Add(item);
            }


            Esercente esercente = await RealmDataStore.Esercente(HttpContext.Session.GetString("IDesercente"));

            int intOI = 480;
            int intOF = 1200;
            int intSlot = 30;
            double dFattore = 0;


            if (esercente.OraInizio != 0) intOI = esercente.OraInizio;
            if (esercente.OraFine != 0) intOF = esercente.OraFine;
            if (esercente.MinutiSlot != 0) intSlot = esercente.MinutiSlot;

            dFattore = 60 / intSlot;

            List<SelectListItem> ListaOrari = new List<SelectListItem>();
            for (int i = Convert.ToInt32(intOI * dFattore / 60); i <= Convert.ToInt32(intOF * dFattore / 60); i++)
            {
                SelectListItem si = new SelectListItem { Text = Utils.Utils.ConvertiOrario(i * intSlot) + "", Value = i * intSlot + "" };
                ListaOrari.Add(si);
            }

            IEnumerable<Calendario> ListaCalendari = await RealmDataStore.ListaCalendari(esercente, dtPrimoGiornoSettimana, IDRisorsaAttivitaR);
            List<StrutturaCalendario> ListaStrutturaCalendari = new List<StrutturaCalendario>();


            //costruisco la mia ListaStrutturaCalendari
            for (int i = Convert.ToInt32(intOI * dFattore / 60); i <= Convert.ToInt32(intOF * dFattore / 60); i++)
            {

                //creo la struttura con 8 colonne e  n-righe con valori di default               
                //Cella standard
                StrutturaCella SC0 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC1 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC2 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC3 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC4 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC5 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC6 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };

                StrutturaCalendario strutturaCalendario = new StrutturaCalendario
                {

                    sOrarioCella = Utils.Utils.ConvertiOrario(i * intSlot),
                    iOrarioCella = i * intSlot,
                    SC1 = SC1,
                    SC2 = SC2,
                    SC3 = SC3,
                    SC4 = SC4,
                    SC5 = SC5,
                    SC6 = SC6,
                    SC0 = SC0
                };

                ListaStrutturaCalendari.Add(strutturaCalendario);
            }

            List<string> CelleSpente = new List<string>();

            //adesso ciclo sulla ListaCalendari per vedere se esistono attività da descrivere
            foreach (Calendario c in ListaCalendari)
            {

                RisAtt = AggiungiElemento(RisAtt, c.RisorsaAttivita.ID);

                //calcolo la capienza residua
                string Res = await RealmDataStore.CapienzaResidua(c.ID, HttpContext.Session.GetString("username"));
                string[] aCR = Res.Split("#");
                int CR = Convert.ToInt32(aCR[0]);
                bool HasBooking = CR < 0 ? false : true;
                string info = aCR[1];
                int Booked = Convert.ToInt32(aCR[2]);

                foreach (StrutturaCalendario s in ListaStrutturaCalendari)
                {
                    
                    if (c.OraInizio == s.iOrarioCella) //ho trovato un elemento che inizia nella riga
                    {
                        //cerco la colonna giusto
                        switch ((int)c.Data.DayOfWeek)
                        {
                            case 0:  //DOM
                                if (s.SC0.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC0.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "0" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC0.Calendario = c;
                                s.SC0.TipoCella = 1;
                                s.SC0.Riga = (c.OraFine-c.OraInizio)/intSlot;
                                s.SC0.Colonna = 1;
                                if (s.SC0.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC0.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio+intSlot*(x-1) + "#0#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC0.CapienzaResidua = Math.Abs(CR);
                                s.SC0.HasBooking = HasBooking;
                                s.SC0.Info = info;
                                s.SC0.Booked = Booked;
                                break;
                            case 1:  //LUN
                                if (s.SC1.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC1.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "1" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC1.Calendario = c;
                                s.SC1.TipoCella = 1;
                                s.SC1.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC1.Colonna = 1;
                                if (s.SC1.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC1.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#1#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC1.CapienzaResidua = Math.Abs(CR);
                                s.SC1.HasBooking = HasBooking;
                                s.SC1.Info = info;
                                s.SC1.Booked = Booked;
                                break;
                            case 2:  //MAR
                                if (s.SC2.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC2.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "2" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC2.Calendario = c;
                                s.SC2.TipoCella = 1;
                                s.SC2.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC2.Colonna = 1;
                                if (s.SC2.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC2.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#2#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC2.CapienzaResidua = Math.Abs(CR);
                                s.SC2.HasBooking = HasBooking;
                                s.SC2.Info = info;
                                s.SC2.Booked = Booked;
                                break;
                            case 3:  //MER
                                if (s.SC3.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC3.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "3" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC3.Calendario = c;
                                s.SC3.TipoCella = 1;
                                s.SC3.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC3.Colonna = 1;
                                if (s.SC3.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC3.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#3#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC3.CapienzaResidua = Math.Abs(CR);
                                s.SC3.HasBooking = HasBooking;
                                s.SC3.Info = info;
                                s.SC3.Booked = Booked;
                                break;
                            case 4:  //GIO
                                if (s.SC4.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC4.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "4" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC4.Calendario = c;
                                s.SC4.TipoCella = 1;
                                s.SC4.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC4.Colonna = 1;
                                if (s.SC4.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC4.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#4#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC4.CapienzaResidua = Math.Abs(CR);
                                s.SC4.HasBooking = HasBooking;
                                s.SC4.Info = info;
                                s.SC4.Booked = Booked;
                                break;
                            case 5:  //VEN
                                if (s.SC5.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC5.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "5" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC5.Calendario = c;
                                s.SC5.TipoCella = 1;
                                s.SC5.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC5.Colonna = 1;
                                if (s.SC5.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC5.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#5#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC5.CapienzaResidua = Math.Abs(CR);
                                s.SC5.HasBooking = HasBooking;
                                s.SC5.Info = info;
                                s.SC5.Booked = Booked;
                                break;
                            case 6:  //SAB
                                if (s.SC6.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC6.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "6" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC6.Calendario = c;
                                s.SC6.TipoCella = 1;
                                s.SC6.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC6.Colonna = 1;
                                if (s.SC6.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC6.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#6#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC6.CapienzaResidua = Math.Abs(CR);
                                s.SC6.HasBooking = HasBooking;
                                s.SC6.Info = info;
                                s.SC6.Booked = Booked;
                                break;
                        }

                        break;
                    }

                }

            }

            //spengo le celle coperte da eventi precedenti
            foreach (string v in CelleSpente)
            {
                string[] cr = v.Split("#");

                foreach (StrutturaCalendario s in ListaStrutturaCalendari)
                {
                    if (Convert.ToInt32(cr[0]) == s.iOrarioCella) //ho trovato un elemento che inizia nella riga
                    {
                        switch (cr[1])
                        {
                            case "0":
                                s.SC0.TipoCella = -1;
                                break;
                            case "1":
                                s.SC1.TipoCella = -1;
                                break;
                            case "2":
                                s.SC2.TipoCella = -1;
                                break;
                            case "3":
                                s.SC3.TipoCella = -1;
                                break;
                            case "4":
                                s.SC4.TipoCella = -1;
                                break;
                            case "5":
                                s.SC5.TipoCella = -1;
                                break;
                            case "6":
                                s.SC6.TipoCella = -1;
                                break;
                        }
                        break;
                    }

                }
            }

            List<SelectListItem> ListaRisorseAttivita = new List<SelectListItem>();
            List<SelectListItem> ListaRisorseAttivitaR = new List<SelectListItem>();
            List<SelectListItem> ListaRisorseAttivitaCapienza = new List<SelectListItem>();

            if (HasConflict == "" && !IsRefresh)
            {
                //aggiunta selezione completa
                SelectListItem itemx = new SelectListItem
                {
                    Text = "Tutti le Risorse-Attività",
                    Value = "000"
                };
                ListaRisorseAttivitaR.Add(itemx);
                IDRisorsaAttivitaR = "000";
            }

            string[] sConflitti = HasConflict.Split("#");


            var ListaRA = await RealmDataStore.ListaRisorseAttivita(esercente);
            foreach (RisorsaAttivita c in ListaRA)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Attivita.Descrizione + "-" + c.Risorsa.Descrizione,
                    Value = c.ID + ""
                };

                ListaRisorseAttivita.Add(item);
                ListaRisorseAttivitaR.Add(item);

                SelectListItem itemc = new SelectListItem
                {
                    Text = c.Capienza + "",
                    Value = c.ID + ""
                };

                ListaRisorseAttivitaCapienza.Add(itemc);
            }

            Calendario calendario = new Calendario
            {
                Capienza = 1
            };

            HelpCalendario helpCalendario = new HelpCalendario
            {
                ListaOrari = ListaOrari,
                ListaStrutturaCalendari = ListaStrutturaCalendari,
                Settimana = Settimana,
                IDSettimana = IDSettimana,
                Anno = Anno,
                pg = dtPrimoGiornoSettimana,
                Esercente = esercente.RagioneSociale,
                ListaGiorni = ListaGiorni,
                ListaRisorseAttivita = ListaRisorseAttivita,
                ListaRisorseAttivitaCapienza = ListaRisorseAttivitaCapienza,
                Calendario = calendario,
                IDSettimanaC = IDSettimana,
                IsNascosto = false,
                ModeEdit = true,
                ModeCalendario = "Ins",
                HasConflict = HasConflict,
                ListaRisorseAttivitaR = ListaRisorseAttivitaR,
                IDRisorsaAttivitaR = IDRisorsaAttivitaR
            };

            if (HasConflict != "" && IsRefresh == true) HasConflict = "OK";

            if (HasConflict == "")
            {
                return View(helpCalendario);
            }
            else if (HasConflict == "OK")
            {
                return View(helpCalendario);
            }
            else
            {
                RisAtt = RisAtt.OrderByDescending(z => z.Totale).ToList();
                return RedirectToAction(nameof(Calendario), new { IDSettimana, Anno, IDRisorsaAttivitaR = RisAtt[0].Elemento, IsRefresh = true });
            }

        }


        [HttpGet]
        public async Task<IActionResult> CalendarioUtente(string IDEsercente, int IDSettimana = -1, int Anno = -1, string Dir = "", string IDRisorsaAttivitaR = "-1", bool IsRefresh = false)
        {

            //salvo l'id dell'Esercente
            //if (ID != null) HttpContext.Session.SetString("IDEsercenteUtente", ID);

            if (!CheckUser() || IDEsercente == null) return RedirectToAction(nameof(Login));

            DateTime dtPrimoGiornoSettimana = DateTime.Now;
            DateTime dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);

            List<Elenco> RisAtt = new List<Elenco>();

            string HasConflict = "";

            if (IDSettimana == -1 || Dir == "oggi") IDSettimana = Utils.Utils.SettimanaAnno(DateTime.Now);
            if (Anno == -1) Anno = DateTime.Now.Year;

            dtPrimoGiornoSettimana = Utils.Utils.PrimoGiornoSettimana(Anno, IDSettimana);
            dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);

            if (Dir == "less") //devo tornare indietro di una settimana
            {
                dtPrimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(-7);
                dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);
            }

            if (Dir == "more") //devo tornare indietro di una settimana
            {
                dtPrimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(7);
                dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);
            }

            IDSettimana = Utils.Utils.SettimanaAnno(dtPrimoGiornoSettimana);
            Anno = dtPrimoGiornoSettimana.Year;

            string Settimana = dtPrimoGiornoSettimana.ToShortDateString() + "-" + dtUltimoGiornoSettimana.ToShortDateString();

            List<SelectListItem> ListaGiorni = new List<SelectListItem>();

            List<DateTime> listaG = new List<DateTime>();
            listaG.Add(dtPrimoGiornoSettimana);
            listaG.Add(dtPrimoGiornoSettimana.AddDays(1));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(2));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(3));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(4));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(5));
            listaG.Add(dtPrimoGiornoSettimana.AddDays(6));


            foreach (DateTime c in listaG)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.ToShortDateString(),
                    Value = c.Year + "#" + c.Month + "#" + c.Day
                };

                ListaGiorni.Add(item);
            }


            Esercente esercente = await RealmDataStore.Esercente(IDEsercente);

            int intOI = 480;
            int intOF = 1200;
            int intSlot = 30;
            double dFattore = 0;


            if (esercente.OraInizio != 0) intOI = esercente.OraInizio;
            if (esercente.OraFine != 0) intOF = esercente.OraFine;
            if (esercente.MinutiSlot != 0) intSlot = esercente.MinutiSlot;

            dFattore = 60 / intSlot;

            IEnumerable<Calendario> ListaCalendari = await RealmDataStore.ListaCalendari(esercente, dtPrimoGiornoSettimana, IDRisorsaAttivitaR);
            List<StrutturaCalendario> ListaStrutturaCalendari = new List<StrutturaCalendario>();


            //costruisco la mia ListaStrutturaCalendari
            for (int i = Convert.ToInt32(intOI * dFattore / 60); i <= Convert.ToInt32(intOF * dFattore / 60); i++)
            {

                //creo la struttura con 8 colonne e  n-righe con valori di default               
                //Cella standard
                StrutturaCella SC0 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC1 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC2 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC3 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC4 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC5 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };
                //Cella standard
                StrutturaCella SC6 = new StrutturaCella
                {
                    Calendario = null,
                    TipoCella = 0,
                    Riga = 1,
                    Colonna = 1
                };

                StrutturaCalendario strutturaCalendario = new StrutturaCalendario
                {

                    sOrarioCella = Utils.Utils.ConvertiOrario(i * intSlot),
                    iOrarioCella = i * intSlot,
                    SC1 = SC1,
                    SC2 = SC2,
                    SC3 = SC3,
                    SC4 = SC4,
                    SC5 = SC5,
                    SC6 = SC6,
                    SC0 = SC0
                };

                ListaStrutturaCalendari.Add(strutturaCalendario);
            }

            List<string> CelleSpente = new List<string>();

            //adesso ciclo sulla ListaCalendari per vedere se esistono attività da descrivere
            foreach (Calendario c in ListaCalendari)
            {

                RisAtt = AggiungiElemento(RisAtt, c.RisorsaAttivita.ID);

                //calcolo la capienza residua
                string Res = await RealmDataStore.CapienzaResidua(c.ID, HttpContext.Session.GetString("username"), false);
                string[] aCR = Res.Split("#");
                int CR = Convert.ToInt32(aCR[0]);
                bool HasBooking = CR < 0 ? false : true;
                string info = aCR[1];
                int Booked = Convert.ToInt32(aCR[2]);

                foreach (StrutturaCalendario s in ListaStrutturaCalendari)
                {

                    if (c.OraInizio == s.iOrarioCella) //ho trovato un elemento che inizia nella riga
                    {
                        //cerco la colonna giusto
                        switch ((int)c.Data.DayOfWeek)
                        {
                            case 0:  //DOM
                                if (s.SC0.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC0.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "0" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC0.Calendario = c;
                                s.SC0.TipoCella = 1;
                                s.SC0.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC0.Colonna = 1;
                                if (s.SC0.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC0.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#0#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC0.CapienzaResidua = Math.Abs(CR);
                                s.SC0.HasBooking = HasBooking;
                                s.SC0.Info = info;
                                s.SC0.Booked = Booked;
                                break;
                            case 1:  //LUN
                                if (s.SC1.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC1.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "1" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC1.Calendario = c;
                                s.SC1.TipoCella = 1;
                                s.SC1.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC1.Colonna = 1;
                                if (s.SC1.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC1.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#1#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC1.CapienzaResidua = Math.Abs(CR);
                                s.SC1.HasBooking = HasBooking;
                                s.SC1.Info = info;
                                s.SC1.Booked = Booked;
                                break;
                            case 2:  //MAR
                                if (s.SC2.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC2.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "2" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC2.Calendario = c;
                                s.SC2.TipoCella = 1;
                                s.SC2.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC2.Colonna = 1;
                                if (s.SC2.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC2.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#2#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC2.CapienzaResidua = Math.Abs(CR);
                                s.SC2.HasBooking = HasBooking;
                                s.SC2.Info = info;
                                s.SC2.Booked = Booked;
                                break;
                            case 3:  //MER
                                if (s.SC3.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC3.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "3" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC3.Calendario = c;
                                s.SC3.TipoCella = 1;
                                s.SC3.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC3.Colonna = 1;
                                if (s.SC3.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC3.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#3#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC3.CapienzaResidua = Math.Abs(CR);
                                s.SC3.HasBooking = HasBooking;
                                s.SC3.Info = info;
                                s.SC3.Booked = Booked;
                                break;
                            case 4:  //GIO
                                if (s.SC4.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC4.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "4" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC4.Calendario = c;
                                s.SC4.TipoCella = 1;
                                s.SC4.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC4.Colonna = 1;
                                if (s.SC4.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC4.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#4#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC4.CapienzaResidua = Math.Abs(CR);
                                s.SC4.HasBooking = HasBooking;
                                s.SC4.Info = info;
                                s.SC4.Booked = Booked;
                                break;
                            case 5:  //VEN
                                if (s.SC5.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC5.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "5" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC5.Calendario = c;
                                s.SC5.TipoCella = 1;
                                s.SC5.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC5.Colonna = 1;
                                if (s.SC5.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC5.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#5#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC5.CapienzaResidua = Math.Abs(CR);
                                s.SC5.HasBooking = HasBooking;
                                s.SC5.Info = info;
                                s.SC5.Booked = Booked;
                                break;
                            case 6:  //SAB
                                if (s.SC6.TipoCella == 1) //cella già occupata 
                                {
                                    HasConflict += "#" + s.SC6.Calendario.RisorsaAttivita.ID + "@" + c.RisorsaAttivita.ID;
                                    break;
                                }
                                else
                                {
                                    //ciclo sulla lista CelleSpente per vedere se questa cella era usata da altre attività
                                    foreach (string v in CelleSpente)
                                    {
                                        string[] cr = v.Split("#");
                                        if (cr[1] == "6" && Convert.ToInt32(cr[0]) == s.iOrarioCella)
                                        {
                                            HasConflict += "#" + c.RisorsaAttivita.ID + "@" + cr[2];
                                            break;
                                        }
                                    }
                                }
                                s.SC6.Calendario = c;
                                s.SC6.TipoCella = 1;
                                s.SC6.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC6.Colonna = 1;
                                if (s.SC6.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC6.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#6#" + c.RisorsaAttivita.ID);
                                    }
                                }
                                s.SC6.CapienzaResidua = Math.Abs(CR);
                                s.SC6.HasBooking = HasBooking;
                                s.SC6.Info = info;
                                s.SC6.Booked = Booked;
                                break;
                        }

                        break;
                    }

                }

            }

            //spengo le celle coperte da eventi precedenti
            foreach (string v in CelleSpente)
            {
                string[] cr = v.Split("#");

                foreach (StrutturaCalendario s in ListaStrutturaCalendari)
                {
                    if (Convert.ToInt32(cr[0]) == s.iOrarioCella) //ho trovato un elemento che inizia nella riga
                    {
                        switch (cr[1])
                        {
                            case "0":
                                s.SC0.TipoCella = -1;
                                break;
                            case "1":
                                s.SC1.TipoCella = -1;
                                break;
                            case "2":
                                s.SC2.TipoCella = -1;
                                break;
                            case "3":
                                s.SC3.TipoCella = -1;
                                break;
                            case "4":
                                s.SC4.TipoCella = -1;
                                break;
                            case "5":
                                s.SC5.TipoCella = -1;
                                break;
                            case "6":
                                s.SC6.TipoCella = -1;
                                break;
                        }
                        break;
                    }

                }
            }

            List<SelectListItem> ListaRisorseAttivitaR = new List<SelectListItem>();

            if (HasConflict == "" && !IsRefresh)
            {
                //aggiunta selezione completa
                SelectListItem itemx = new SelectListItem
                {
                    Text = "Tutti le Risorse-Attività",
                    Value = "000"
                };
                ListaRisorseAttivitaR.Add(itemx);
                IDRisorsaAttivitaR = "000";
            }

            string[] sConflitti = HasConflict.Split("#");


            var ListaRA = await RealmDataStore.ListaRisorseAttivita(esercente);
            foreach (RisorsaAttivita c in ListaRA)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Attivita.Descrizione + "-" + c.Risorsa.Descrizione,
                    Value = c.ID + ""
                };

                ListaRisorseAttivitaR.Add(item);

            }

            Calendario calendario = new Calendario
            {
                Capienza = 1
            };

            HelpPrenotazione helpPrenotazione = new HelpPrenotazione
            {
                ListaStrutturaCalendari = ListaStrutturaCalendari,
                Settimana = Settimana,
                IDSettimana = IDSettimana,
                Anno = Anno,
                pg = dtPrimoGiornoSettimana,
                Esercente = esercente.RagioneSociale,
                Calendario = calendario,
                IDSettimanaC = IDSettimana,
                ModeEdit = true,
                ModeCalendario = "Ins",
                HasConflict = HasConflict,
                ListaRisorseAttivitaR = ListaRisorseAttivitaR,
                IDRisorsaAttivitaR = IDRisorsaAttivitaR,
                IDEsercente = IDEsercente
            };

            if (HasConflict != "" && IsRefresh == true) HasConflict = "OK";

            if (HasConflict == "")
            {
                return View(helpPrenotazione);
            }
            else if (HasConflict == "OK")
            {
                return View(helpPrenotazione);
            }
            else
            {
                RisAtt = RisAtt.OrderByDescending(z => z.Totale).ToList();
                return RedirectToAction(nameof(CalendarioUtente), new { IDEsercente, IDSettimana, Anno, IDRisorsaAttivitaR = RisAtt[0].Elemento, IsRefresh = true });
            }

        }


        public async Task<IActionResult> InserisciAttivita(HelpAttivitaRisorse helpAttivitaRisorse)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpAttivitaRisorse.ModeAttivita == "Cancel") return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 1 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 1 });

            if (helpAttivitaRisorse.ModeAttivita == "Ins")
            {
                await RealmDataStore.InserisciAttivita(helpAttivitaRisorse.Esercente.ID, helpAttivitaRisorse.Attivita);
            }
            else if (helpAttivitaRisorse.ModeAttivita == "Upd")
            {
                await RealmDataStore.AggiornaAttivita(helpAttivitaRisorse.IDAttivita, helpAttivitaRisorse.Attivita);
            }
            return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 1 });
        }


        public async Task<IActionResult> InserisciRisorsaAttivita(HelpAttivitaRisorse helpAttivitaRisorse)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpAttivitaRisorse.ModeRisorsaAttivita == "Cancel") return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 4 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 4 });

            if (helpAttivitaRisorse.ModeRisorsaAttivita == "Ins")
            {
                await RealmDataStore.InserisciRisorsaAttivita(helpAttivitaRisorse.Esercente.ID, helpAttivitaRisorse.RisorsaAttivita, helpAttivitaRisorse.IDAttivita, helpAttivitaRisorse.IDRisorsa);
            }
            else if (helpAttivitaRisorse.ModeRisorsaAttivita == "Upd")
            {
                await RealmDataStore.AggiornaRisorsaAttivita(helpAttivitaRisorse.IDRisorsaAttivita, helpAttivitaRisorse.RisorsaAttivita, helpAttivitaRisorse.IDAttivita, helpAttivitaRisorse.IDRisorsa);
            }
            return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 4 });
        }


        public async Task<IActionResult> InserisciRisorsa(HelpAttivitaRisorse helpAttivitaRisorse)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpAttivitaRisorse.ModeRisorsa == "Cancel") return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 2 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 2 });

            if (helpAttivitaRisorse.ModeRisorsa == "Ins")
            {
                await RealmDataStore.InserisciRisorsa(helpAttivitaRisorse.Esercente.ID, helpAttivitaRisorse.Risorsa);
            }
            else if (helpAttivitaRisorse.ModeRisorsa == "Upd")
            {
                await RealmDataStore.AggiornaRisorsa(helpAttivitaRisorse.IDRisorsa, helpAttivitaRisorse.Risorsa);
            }
            return RedirectToAction(nameof(AttivitaRisorse), new { helpAttivitaRisorse.Esercente.ID, tab = 2 });
        }


        public async Task<IActionResult> EliminaAttivita(string ID, string IDAttivita)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaAttivita(IDAttivita);

            return RedirectToAction(nameof(AttivitaRisorse), new { ID, tab = 1 });
        }


        public async Task<IActionResult> EliminaRisorsa(string ID, string IDRisorsa)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaRisorsa(IDRisorsa);

            return RedirectToAction(nameof(AttivitaRisorse), new { ID, tab = 2 });
        }

        public async Task<IActionResult> CreaCalendario(int IDSettimana, int Anno)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.CreaCalendario(HttpContext.Session.GetString("IDesercente"), IDSettimana, Anno);

            return RedirectToAction(nameof(Calendario), new { IDSettimana, Anno });
        }


        public async Task<IActionResult> EliminaPianificazione(string ID)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaPianificazione(ID);

            return RedirectToAction(nameof(Pianificazione), new { });
        }

        public async Task<IActionResult> EliminaCalendario(string IDCalendarioU, int IDSettimana, int Anno)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaCalendario(IDCalendarioU);

            return RedirectToAction(nameof(Calendario), new { IDSettimana, Anno });
        }

        public async Task<IActionResult> EliminaPrenotazione(string IDCalendarioE, int IDSettimana, int Anno)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaPrenotazione(IDCalendarioE, HttpContext.Session.GetString("username"));

            return RedirectToAction(nameof(Calendario), new { IDSettimana, Anno });
        }


        public async Task<IActionResult> EliminaPrenotazioneUtente(string IDCalendarioE, int IDSettimana, int Anno, string IDEsercente)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaPrenotazione(IDCalendarioE, HttpContext.Session.GetString("username"));

            return RedirectToAction(nameof(CalendarioUtente), new { IDEsercente, IDSettimana, Anno });
        }




        public async Task<IActionResult> EliminaRisorsaAttivita(string ID, string IDRisorsaAttivita)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaRisorsaAttivita(IDRisorsaAttivita);

            return RedirectToAction(nameof(AttivitaRisorse), new { ID, tab = 4 });
        }

        [HttpPost]
        public async Task<IActionResult> Prenotazione(HelpCalendario helpCalendario)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpCalendario.ModePrenotazione == "Ins")
            {
                await RealmDataStore.InserisciPrenotazione(HttpContext.Session.GetString("IDesercente"), helpCalendario.IDCalendario, HttpContext.Session.GetString("username"), helpCalendario.Nota);
            }

            return RedirectToAction(nameof(Calendario), new { });
        }

        [HttpPost]
        public async Task<IActionResult> PrenotazioneUtente(HelpPrenotazione helpPrenotazione)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpPrenotazione.ModePrenotazione == "Ins")
            {
                await RealmDataStore.InserisciPrenotazione(helpPrenotazione.IDEsercente, helpPrenotazione.IDCalendario, HttpContext.Session.GetString("username"), helpPrenotazione.Nota);
            }

            return RedirectToAction(nameof(CalendarioUtente), new { helpPrenotazione.IDEsercente, helpPrenotazione.IDSettimana, helpPrenotazione.Anno });
        }


    }
}
