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
        public async Task<IActionResult> EditStatus(string ID, bool ins, bool mod, bool del)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            Status status = null;
            if (mod || del)
            {
                status = await RealmDataStore.Status(ID);
            }
            else
            {
                status = new Status
                {
                    IsSistema = false,
                    User = HttpContext.Session.GetString("user")
                };
            }

            HelpStatus helpStatus = new HelpStatus
            {
                Status = status,
                Ins = ins,
                Mod = mod,
                Del = del
            };

            return View(helpStatus);
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

        [HttpGet]
        public async Task<IActionResult> EditCausaDebito(string ID, bool ins, bool mod, bool del)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            CausaDebito causaDebito = null;
            if (mod || del)
            {
                causaDebito = await RealmDataStore.CausaDebito(ID);
            }
            else
            {
                causaDebito = new CausaDebito
                {
                    IsSistema = false,
                    User = HttpContext.Session.GetString("user")
                };
            }

            HelpCausaDebito helpCausaDebito = new HelpCausaDebito
            {
                CausaDebito = causaDebito,
                Ins = ins,
                Mod = mod,
                Del = del
            };

            return View(helpCausaDebito);
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
                //await RealmDataStore.InserisciPianificazione(pianificazione, helpPianificazione.IDRisorsaAttivita, helpPianificazione.IDGiorno, helpPianificazione.IDOraInizio, helpPianificazione.IDOraFine, HttpContext.Session.GetString("IDesercente"));
            }
            else if (helpCalendario.ModeCalendario == "Upd")
            {
                //await RealmDataStore.AggiornaPianificazione(pianificazione, helpPianificazione.IDRisorsaAttivita, helpPianificazione.IDGiorno, helpPianificazione.IDOraInizio, helpPianificazione.IDOraFine, HttpContext.Session.GetString("IDesercente"));
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

        [HttpPost]
        public async Task<IActionResult> EditStatus(HelpStatus helpStatus)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (!ModelState.IsValid) return View(helpStatus);

            Status status = helpStatus.Status;

            if (helpStatus.Mod)
            {
                await RealmDataStore.AggiornaStatus(status);
            }
            else if (helpStatus.Ins)
            {
                await RealmDataStore.InserisciStatus(status);
            }
            else
            {
                await RealmDataStore.EliminaStatus(status);
            }

            return RedirectToAction(nameof(Status));
        }

        [HttpPost]
        public async Task<IActionResult> EditCausaDebito(HelpCausaDebito helpCausaDebito)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (!ModelState.IsValid) return View(helpCausaDebito);

            CausaDebito causaDebito = helpCausaDebito.CausaDebito;

            if (helpCausaDebito.Mod)
            {
                await RealmDataStore.AggiornaCausaDebito(causaDebito);
            }
            else if (helpCausaDebito.Ins)
            {
                await RealmDataStore.InserisciCausaDebito(causaDebito);
            }
            else
            {
                await RealmDataStore.EliminaCausaDebito(causaDebito);
            }

            return RedirectToAction(nameof(CauseDebito));
        }

        public async Task<IActionResult> Status()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            var lista = await RealmDataStore.ListaStatus(HttpContext.Session.GetString("user"));

            return View(lista);
        }

        public async Task<IActionResult> CauseDebito()
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            var lista = await RealmDataStore.ListaCauseDebito(HttpContext.Session.GetString("user"));

            return View(lista);
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

                //HttpContext.Session.SetString("provinciale", PRV);
                //HttpContext.Session.SetString("descrizioneProvinciale", agente.Nominativo);

                ////qui aggiorno la tabella Associato per inserire la Provincia di appartenenza
                //var listaAs1 = await RealmDataStore.ListaAssociati("-1");
                //foreach (Associato ax in listaAs1)
                //{
                //    await RealmDataStore.AggiornaProvincialeAssociato(ax.ID, HttpContext.Session.GetString("provinciale"));
                //}
                ////fine aggiornamento

                ////qui aggiorno la tabella Servizio per inserire la Provincia di appartenenza
                //var listaAs2 = await RealmDataStore.ListaServizi("-1");
                //foreach (Servizio ax in listaAs2)
                //{
                //    await RealmDataStore.AggiornaProvincialeServizio(ax.ID, HttpContext.Session.GetString("provinciale"));
                //}
                ////fine aggiornamento

                ////qui aggiorno la tabella Agente per inserire la Provincia di appartenenza e per riempire il campo Codice col il contenuto del campo ID
                //var listaAs3 = await RealmDataStore.ListaAgenti(false, "-1");
                //foreach (Agente ax in listaAs3)
                //{
                //    await RealmDataStore.AggiornaProvincialeAgente(ax.ID, HttpContext.Session.GetString("provinciale"));
                //}
                ////fine aggiornamento

                ////qui aggiorno la tabella Caricamento INAIL per inserire la Provincia di appartenenza
                //var listaC1 = await RealmDataStore.ListaCaricamentiINAIL("-1");
                //foreach (CaricamentoINAIL ax in listaC1)
                //{
                //    await RealmDataStore.AggiornaProvincialeCaricamentoINAIL(ax.ID, HttpContext.Session.GetString("provinciale"));
                //}
                ////fine aggiornamento

                ////qui aggiorno la tabella Caricamento INPS per inserire la Provincia di appartenenza
                //var listaC2 = await RealmDataStore.ListaCaricamentiINPS("-1");
                //foreach (CaricamentoINPS ax in listaC2)
                //{
                //    await RealmDataStore.AggiornaProvincialeCaricamentoINPS(ax.ID, HttpContext.Session.GetString("provinciale"));
                //}
                ////fine aggiornamento

                ////qui aggiorno la tabella Caricamento W430 per inserire la Provincia di appartenenza
                //var listaC3 = await RealmDataStore.ListaCaricamentiW430("-1");
                //foreach (CaricamentoW430 ax in listaC3)
                //{
                //    await RealmDataStore.AggiornaProvincialeCaricamentoW430(ax.ID, HttpContext.Session.GetString("provinciale"));
                //}
                ////fine aggiornamento

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

                if (nR == 4 && ruolo == "#CLI") return RedirectToAction(nameof(Proposte), new { utenti.Mail });

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



        public async Task<IActionResult> Proposta(string ID = null, int tab = 1)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            List<SelectListItem> ListaCauseDebito = new List<SelectListItem>();
            var listaTP = await RealmDataStore.ListaCauseDebito(HttpContext.Session.GetString("user"));
            foreach (CausaDebito c in listaTP)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaCauseDebito.Add(item);
            }

            List<SelectListItem> ListaStatus = new List<SelectListItem>();
            var listaS = await RealmDataStore.ListaStatus(1);
            foreach (Status tp in listaS)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = tp.Descrizione,
                    Value = tp.ID + ""
                };

                ListaStatus.Add(item);
            }

            List<SelectListItem> ListaTipiPratica = new List<SelectListItem>();
            var listaP = await RealmDataStore.ListaTipiPratica(1);
            foreach (TipoPratica tp in listaP)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = tp.Descrizione,
                    Value = tp.ID + ""
                };

                ListaTipiPratica.Add(item);
            }

            List<SelectListItem> ListaGradi = new List<SelectListItem>();
            var listaRP = await RealmDataStore.ListaGradi();
            foreach (Gradi gr in listaRP)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = gr.Descrizione,
                    Value = gr.ID + ""
                };

                ListaGradi.Add(item);
            }



            Proposte proposte = new Proposte
            {
                DataCreazione = DateTimeOffset.Now,
                DataUltimaModifica = DateTimeOffset.Now
            };

            IEnumerable<MasseDebitorie> ListaMasseDebitorie = null;
            IEnumerable<AttiDisposizione> ListaAttiDisposizione = null;
            IEnumerable<AnnotazioniAggiuntive> ListaAnnotazioniAggiuntive = null;
            IEnumerable<SpeseMese> ListaSpeseMese = null;
            IEnumerable<PatrimonioImmobiliare> ListaPatrimonioImmobiliare = null;
            IEnumerable<BeniMobiliRegistrati> ListaBeniMobiliRegistrati = null;
            IEnumerable<BeniMobili> ListaBeniMobili = null;
            IEnumerable<Ricorrenti> ListaRicorrenti = null;

            string TP_CauseIndebitamentoeDiligenza = await RealmDataStore.TestoPrecaricato("CauseIndebitamentoeDiligenza");
            string TP_ResocontoPagamenti5Anni = await RealmDataStore.TestoPrecaricato("ResocontoPagamenti5Anni");


            if (ID != null)
            {
                proposte = await RealmDataStore.Proposte(ID);

                ListaMasseDebitorie = await RealmDataStore.ListaMasseDebitorie(proposte);

                ListaAttiDisposizione = await RealmDataStore.ListaAttiDisposizione(proposte);

                ListaAnnotazioniAggiuntive = await RealmDataStore.ListaAnnotazioniAggiuntive(proposte);

                ListaSpeseMese = await RealmDataStore.ListaSpeseMese(proposte);

                ListaPatrimonioImmobiliare = await RealmDataStore.ListaPatrimonioImmobiliare(proposte);

                ListaBeniMobiliRegistrati = await RealmDataStore.ListaBeniMobiliRegistrati(proposte);

                ListaBeniMobili = await RealmDataStore.ListaBeniMobili(proposte);

                ListaRicorrenti = await RealmDataStore.ListaRicorrenti(proposte);
            }


            HelpProposta helpProposta = new HelpProposta
            {
                Proposte = proposte,
                IDStatus = proposte.Status == null ? "-1" : proposte.Status.ID,
                IDTipiPratica = proposte.TipoPratica == null ? "-1" : proposte.TipoPratica.ID,
                ListaMasseDebitorie = ListaMasseDebitorie,
                ListaAttiDisposizione = ListaAttiDisposizione,
                ListaAnnotazioniAggiuntive = ListaAnnotazioniAggiuntive,
                ListaSpeseMese = ListaSpeseMese,
                ListaPatrimonioImmobiliare = ListaPatrimonioImmobiliare,
                ListaBeniMobiliRegistrati = ListaBeniMobiliRegistrati,
                ListaBeniMobili = ListaBeniMobili,
                ListaStatus = ListaStatus,
                ListaTipiPratica = ListaTipiPratica,
                ListaCauseDebito = ListaCauseDebito,
                ListaGradi = ListaGradi,
                Ins = (ID == null),
                Tab = tab,
                TP_CauseIndebitamentoeDiligenza = TP_CauseIndebitamentoeDiligenza,
                TP_ResocontoPagamenti5Anni = TP_ResocontoPagamenti5Anni,
                ListaRicorrenti = ListaRicorrenti
            };

            return View(helpProposta);
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



        public async Task<IActionResult> Relazione(string ID = null, int tab = 1)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            List<SelectListItem> ListaCauseDebito = new List<SelectListItem>();
            var listaTP = await RealmDataStore.ListaCauseDebito(HttpContext.Session.GetString("user"));
            foreach (CausaDebito c in listaTP)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.Descrizione,
                    Value = c.ID + ""
                };

                ListaCauseDebito.Add(item);
            }

            List<SelectListItem> ListaStatus = new List<SelectListItem>();
            var listaS = await RealmDataStore.ListaStatus(1);
            foreach (Status tp in listaS)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = tp.Descrizione,
                    Value = tp.ID + ""
                };

                ListaStatus.Add(item);
            }


            List<SelectListItem> ListaTipiPratica = new List<SelectListItem>();
            var listaP = await RealmDataStore.ListaTipiPratica(1);
            foreach (TipoPratica tp in listaP)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = tp.Descrizione,
                    Value = tp.ID + ""
                };

                ListaTipiPratica.Add(item);
            }


            List<SelectListItem> ListaGradi = new List<SelectListItem>();
            var listaRP = await RealmDataStore.ListaGradi();
            foreach (Gradi gr in listaRP)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = gr.Descrizione,
                    Value = gr.ID + ""
                };

                ListaGradi.Add(item);
            }



            Relazioni relazioni = new Relazioni
            {
                DataCreazione = DateTimeOffset.Now,
                DataUltimaModifica = DateTimeOffset.Now
            };

            IEnumerable<MasseDebitorie> ListaMasseDebitorie = null;
            IEnumerable<AttiDisposizione> ListaAttiDisposizione = null;
            IEnumerable<AnnotazioniAggiuntive> ListaAnnotazioniAggiuntive = null;
            IEnumerable<SpeseMese> ListaSpeseMese = null;
            IEnumerable<PatrimonioImmobiliare> ListaPatrimonioImmobiliare = null;
            IEnumerable<BeniMobiliRegistrati> ListaBeniMobiliRegistrati = null;
            IEnumerable<BeniMobili> ListaBeniMobili = null;
            IEnumerable<Ricorrenti> ListaRicorrenti = null;

            string TP_CauseIndebitamentoeDiligenza = await RealmDataStore.TestoPrecaricato("CauseIndebitamentoeDiligenza");
            string TP_ResocontoPagamenti5Anni = await RealmDataStore.TestoPrecaricato("ResocontoPagamenti5Anni");


            if (ID != null)
            {
                relazioni = await RealmDataStore.Relazioni(ID);

                ListaMasseDebitorie = await RealmDataStore.ListaMasseDebitorieR(relazioni);

                ListaAttiDisposizione = await RealmDataStore.ListaAttiDisposizioneR(relazioni);

                ListaAnnotazioniAggiuntive = await RealmDataStore.ListaAnnotazioniAggiuntiveR(relazioni);

                ListaSpeseMese = await RealmDataStore.ListaSpeseMeseR(relazioni);

                ListaPatrimonioImmobiliare = await RealmDataStore.ListaPatrimonioImmobiliareR(relazioni);

                ListaBeniMobiliRegistrati = await RealmDataStore.ListaBeniMobiliRegistratiR(relazioni);

                ListaBeniMobili = await RealmDataStore.ListaBeniMobiliR(relazioni);

                ListaRicorrenti = await RealmDataStore.ListaRicorrentiR(relazioni);

            }


            HelpRelazione helpRelazione = new HelpRelazione
            {
                Relazioni = relazioni,
                IDStatus = relazioni.Status == null ? "-1" : relazioni.Status.ID,
                IDTipiPratica = relazioni.TipoPratica == null ? "-1" : relazioni.TipoPratica.ID,
                ListaMasseDebitorie = ListaMasseDebitorie,
                ListaAttiDisposizione = ListaAttiDisposizione,
                ListaAnnotazioniAggiuntive = ListaAnnotazioniAggiuntive,
                ListaSpeseMese = ListaSpeseMese,
                ListaPatrimonioImmobiliare = ListaPatrimonioImmobiliare,
                ListaBeniMobiliRegistrati = ListaBeniMobiliRegistrati,
                ListaBeniMobili = ListaBeniMobili,
                ListaStatus = ListaStatus,
                ListaTipiPratica = ListaTipiPratica,
                ListaCauseDebito = ListaCauseDebito,
                ListaGradi = ListaGradi,
                Ins = (ID == null),
                Tab = tab,
                TP_CauseIndebitamentoeDiligenza = TP_CauseIndebitamentoeDiligenza,
                TP_ResocontoPagamenti5Anni = TP_ResocontoPagamenti5Anni,
                ListaRicorrenti = ListaRicorrenti
            };

            return View(helpRelazione);
        }


        [HttpPost]
        public async Task<IActionResult> AggiornaProposta(HelpProposta helpProposta)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });


            if (!ModelState.IsValid) return View(helpProposta);

            await RealmDataStore.AggiornaProposta(helpProposta.Proposte, helpProposta.IDStatus, helpProposta.IDTipiPratica, helpProposta.Ins, HttpContext.Session.GetString("user"));

            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, Tab=1 });
        }

        [HttpPost]
        public async Task<IActionResult> AggiornaRelazione(HelpRelazione helpRelazione)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });


            if (!ModelState.IsValid) return View(helpRelazione);

            await RealmDataStore.AggiornaRelazione(helpRelazione.Relazioni, helpRelazione.IDStatus, helpRelazione.IDTipiPratica, helpRelazione.Ins, HttpContext.Session.GetString("user"));

            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, Tab = 1 });
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

        public async Task<IActionResult>GeneraProposta(string IDProposta)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "Modello Proposta Integrato.docx";
            var memory = new MemoryStream();
            string sFileOut = Path.GetTempPath() + sFileName;
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "etc", "docx", sFileName));
            file.CopyTo(sFileOut,true);
            Application app = new Microsoft.Office.Interop.Word.Application();
            Document doc = app.Documents.Open(sFileOut);
            app.Visible = true;
            string[] bookmark = new string[] { "TribunaleAdito", "SezioneTribunaleAdito", "TribunaleAdito2", "SezioneTribunaleAdito2", 
                "TribunaleAdito3", "SezioneTribunaleAdito3", "TribunaleAdito4", "SezioneTribunaleAdito4", "TribunaleAdito5", "SezioneTribunaleAdito5",
                "TribunaleAdito6", "SezioneTribunaleAdito6", "Oggetto", "LuogoData", "CauseIndebitamentoeDiligenza", "ResocontoPagamenti5Anni", 
                "PrimoRicorrente", "PrimoRicorrenteCF", "DomicilioPrimoRicorrente", "SecondoRicorrente", "SecondoRicorrenteCF", "DomicilioSecondoRicorrente",
                "PrimoRicorrenteA", "PrimoRicorrenteCFA", "DomicilioPrimoRicorrenteA", "SecondoRicorrenteA", "SecondoRicorrenteCFA", "DomicilioSecondoRicorrenteA",
                "PrimoRicorrenteB", "PrimoRicorrenteCFB", "DomicilioPrimoRicorrenteB", "SecondoRicorrenteB", "SecondoRicorrenteCFB", "DomicilioSecondoRicorrenteB",
                "PrimoRicorrenteC", "PrimoRicorrenteCFC", "DomicilioPrimoRicorrenteC", "SecondoRicorrenteC", "SecondoRicorrenteCFC", "DomicilioSecondoRicorrenteC",
                "PrimoRicorrenteD", "PrimoRicorrenteCFD", "DomicilioPrimoRicorrenteD", "SecondoRicorrenteD", "SecondoRicorrenteCFD", "DomicilioSecondoRicorrenteD",
                "PrimoRicorrenteE", "PrimoRicorrenteCFE", "DomicilioPrimoRicorrenteE", "SecondoRicorrenteE", "SecondoRicorrenteCFE", "DomicilioSecondoRicorrenteE",
                "Creditore", "RateProposte", "RataMax", "Nota", "CreditoreA", "CreditoreB", "DescrizionePIC", "DescrizioneBMRC",
                "DescrizioneBMC", "AttiDisposizioneD", "SpesaE"};


            Tables tables = doc.Tables;

            int appo = tables.Count;
            int righe = tables[1].Rows.Count;
            int bmk = doc.Bookmarks.Count;

            object missing = System.Reflection.Missing.Value;


            Proposte proposta = await RealmDataStore.Proposte(IDProposta);
            IEnumerable<MasseDebitorie> masseDebitorie = await RealmDataStore.ListaMasseDebitorie(proposta);
            IEnumerable<AnnotazioniAggiuntive> annotazioniAggiuntive = await RealmDataStore.ListaAnnotazioniAggiuntive(proposta);
            IEnumerable<SpeseMese> speseMese = await RealmDataStore.ListaSpeseMese(proposta);
            IEnumerable<PatrimonioImmobiliare> patrimonioImmobiliare = await RealmDataStore.ListaPatrimonioImmobiliare(proposta);
            IEnumerable<BeniMobiliRegistrati> beniMobiliRegistrati = await RealmDataStore.ListaBeniMobiliRegistrati(proposta);
            IEnumerable<BeniMobili> beniMobili = await RealmDataStore.ListaBeniMobili(proposta);
            IEnumerable<AttiDisposizione> attiDisposiziones = await RealmDataStore.ListaAttiDisposizione(proposta);

            string[,] Riparto1 = new string[masseDebitorie.Count(), 7];
            int r3 = 0;
            double[] Totali = new double[3];
            double importo = 0;
            foreach (MasseDebitorie md in masseDebitorie)
            {
                Riparto1[r3, 0] = md.Creditore;
                Riparto1[r3, 1] = md.Grado.Descrizione;
                Riparto1[r3, 2] = md.Importo.ToString("C2");
                Riparto1[r3, 3] = (md.PercentualeSoddisfo / 100).ToString("P");
                importo = (md.Importo * (md.PercentualeSoddisfo / 100));
                if (md.Grado.Aggregazione == 1)
                {
                    Riparto1[r3, 4] = importo.ToString("C2");
                    Totali[0] += importo;
                }
                else if (md.Grado.Aggregazione == 2)
                {
                    Riparto1[r3, 5] = importo.ToString("C2");
                    Totali[1] += importo;
                }
                else if (md.Grado.Aggregazione == 3)
                {
                    Riparto1[r3, 6] = importo.ToString("C2");
                    Totali[2] += importo;
                }
                r3 += 1;
            }



            for (int i = 0; i < bookmark.Length; i++)
            {
                Bookmark bm = doc.Bookmarks[bookmark[i]];
                Microsoft.Office.Interop.Word.Range range = bm.Range;
                switch (i)
                    {
                    case 0:
                    case 2:
                    case 4:
                    case 6:
                    case 8:
                    case 10:
                        range.Text = proposta.Tribunale;
                        break;
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case 11:
                        range.Text = proposta.SezioneTribunale;
                        break;
                    case 12:
                        range.Text = proposta.Oggetto;
                        break;
                    case 13:
                        range.Text = proposta.LuogoData;
                        break;
                    case 14:
                        range.Text = proposta.CauseIndebitamentoeDiligenza;
                        break;
                    case 15:
                        range.Text = proposta.ResocontoPagamenti5Anni;
                        break;
                    case 53:
                        range.Text = proposta.RateProposte.ToString();
                        break;
                    case 54:
                        range.Text = proposta.RataMax.ToString("C2");
                        break;
                    case 16:
                    case 22:
                    case 28:
                    case 34:
                    case 40:
                    case 46:
                        range.Text = ""; //proposta.PrimoRicorrente;
                        break;
                    case 17:
                    case 23:
                    case 29:
                    case 35:
                    case 41:
                    case 47:
                        range.Text = ""; //proposta.PrimoRicorrenteCF;
                        break;
                    case 18:
                    case 24:
                    case 30:
                    case 36:
                    case 42:
                    case 48:
                        range.Text = ""; //proposta.DomicilioPrimoRicorrente;
                        break;
                    case 19:
                    case 25:
                    case 31:
                    case 37:
                    case 43:
                    case 49:
                        range.Text = ""; //proposta.SecondoRicorrente;
                        break;
                    case 20:
                    case 26:
                    case 32:
                    case 38:
                    case 44:
                    case 50:
                        range.Text = ""; //proposta.SecondoRicorrenteCF;
                        break;
                    case 21:
                    case 27:
                    case 33:
                    case 39:
                    case 45:
                    case 51:
                        range.Text = ""; //proposta.DomicilioSecondoRicorrente;
                        break;
                    case 52: //tabella pagina 5 su Massa Debitoria Rilevata
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableMD = tables[1];

                        for (int nr = 1; nr <= masseDebitorie.Count()-1; nr++)
                        {
                            Row row = tableMD.Rows.Add(ref missing);
                        }
                        int r = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (MasseDebitorie md in masseDebitorie)
                        {
                            Row row = tableMD.Rows[r + 1];
                            row.Cells[1].Range.Text = md.Creditore;
                            row.Cells[2].Range.Text = md.CausaDebito.Descrizione;
                            row.Cells[3].Range.Text = md.Grado.Descrizione;
                            row.Cells[4].Range.Text = md.Importo.ToString("C2");
                            row.Cells[5].Range.Text = (md.PercentualeSoddisfo/100).ToString("P");
                            r += 1;
                        }
                        break;
                    case 55: //tabella pagina 6 su Annotazioni Aggiuntive
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableAnnotazioni = tables[2];

                        for (int nr = 1; nr <= annotazioniAggiuntive.Count() - 1; nr++)
                        {
                            Row row2 = tableAnnotazioni.Rows.Add(ref missing);
                        }
                        int r2 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (AnnotazioniAggiuntive md in annotazioniAggiuntive)
                        {
                            Row row2 = tableAnnotazioni.Rows[r2];
                            row2.Cells[1].Range.Text = md.Descrizione;
                            r2 += 1;
                        }
                        break;
                    case 56: //tabella pagina 7 su Riparto
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableRiparto1 = tables[3];
                        for (int nr = 1; nr <= masseDebitorie.Count(); nr++)
                        {
                            Row row3A = tableRiparto1.Rows.Add(ref missing);
                        }
                        //secondo passaggio: valorizzare i campi in tabella
                        for (int nr = 1; nr <= Riparto1.GetUpperBound(0) + 1; nr++)
                        {
                            Row row3 = tableRiparto1.Rows[nr + 1];
                            row3.Cells[1].Range.Text = Riparto1[nr - 1, 0];
                            row3.Cells[2].Range.Text = Riparto1[nr - 1, 1];
                            row3.Cells[3].Range.Text = Riparto1[nr - 1, 2];
                            row3.Cells[4].Range.Text = Riparto1[nr - 1, 3];
                            row3.Cells[5].Range.Text = Riparto1[nr - 1, 4];
                            row3.Cells[6].Range.Text = Riparto1[nr - 1, 5];
                            row3.Cells[7].Range.Text = Riparto1[nr - 1, 6];
                            r3 = nr;
                        }
                        //ultima riga: i totali
                        Row row3B = tableRiparto1.Rows[r3 + 2];
                        row3B.Cells[1].Range.Text = "TOTALE";
                        row3B.Cells[1].Range.Bold = -1;
                        row3B.Cells[5].Range.Text = Totali[0].ToString("C2");
                        row3B.Cells[6].Range.Text = Totali[1].ToString("C2");
                        row3B.Cells[7].Range.Text = Totali[2].ToString("C2");

                        //tabella 4 a pagina 7
                        Table tableRiparto2 = tables[4];
                        //secondo passaggio: valorizzare i campi in tabella
                        double TotRateNec = 0;
                        for (int nr = 1; nr <= 3; nr++)
                        {
                            Row row4 = tableRiparto2.Rows[nr + 1];
                            row4.Cells[2].Range.Text = Totali[nr-1].ToString("C2");
                            row4.Cells[3].Range.Text = proposta.RataMax.ToString("C2");
                            row4.Cells[4].Range.Text = (Totali[nr-1]/proposta.RataMax).ToString("F2");
                            TotRateNec += Totali[nr - 1] / proposta.RataMax;
                        }
                        Row row4T = tableRiparto2.Rows[5];
                        row4T.Cells[4].Range.Text = TotRateNec.ToString("F2");
                        break;
                    case 57: //tabella pagina 8 su Creditori
                        Table tableCreditori = tables[5];
                        for (int nr = 1; nr <= masseDebitorie.Count() - 1; nr++)
                        {
                            Row row = tableCreditori.Rows.Add(ref missing);
                        }
                        int r5 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (MasseDebitorie md in masseDebitorie)
                        {
                            Row row = tableCreditori.Rows[r5 + 1];
                            row.Cells[1].Range.Text = md.Creditore;
                            row.Cells[2].Range.Text = md.Grado.Descrizione;
                            row.Cells[3].Range.Text = md.Importo.ToString("C2");
                            r5 += 1;
                        }
                        break;
                    case 58: //tabella pagina 9 su Patrimonio Immobiliare
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tablePI = tables[6];

                        for (int nr = 1; nr <= patrimonioImmobiliare.Count() - 1; nr++)
                        {
                            Row row6 = tablePI.Rows.Add(ref missing);
                        }
                        int r6 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (PatrimonioImmobiliare md in patrimonioImmobiliare)
                        {
                            Row row6 = tablePI.Rows[r6 + 1];
                            row6.Cells[1].Range.Text = md.Descrizione;
                            row6.Cells[2].Range.Text = md.Superficie.ToString();
                            row6.Cells[3].Range.Text = md.ValoreOmiVan.ToString("C2");
                            row6.Cells[4].Range.Text = md.StimaPerIntero.ToString("C2");
                            r6 += 1;
                        }
                        break;
                    case 59: //tabella pagina 9 su Patrimonio Immobiliare
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableBMR = tables[7];

                        for (int nr = 1; nr <= beniMobiliRegistrati.Count() - 1; nr++)
                        {
                            Row row7 = tableBMR.Rows.Add(ref missing);
                        }
                        int r7 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (BeniMobiliRegistrati md in beniMobiliRegistrati)
                        {
                            Row row7 = tableBMR.Rows[r7 + 1];
                            row7.Cells[1].Range.Text = md.Descrizione;
                            row7.Cells[2].Range.Text = md.Targa;
                            row7.Cells[3].Range.Text = md.Stima.ToString("C2");
                            r7 += 1;
                        }
                        break;
                    case 60: //tabella pagina 9 su Patrimonio Immobiliare
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableBM = tables[8];

                        for (int nr = 1; nr <= beniMobili.Count() - 1; nr++)
                        {
                            Row row8 = tableBM.Rows.Add(ref missing);
                        }
                        int r8 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (BeniMobili md in beniMobili)
                        {
                            Row row8 = tableBM.Rows[r8+1];
                            row8.Cells[1].Range.Text = md.Descrizione;
                            row8.Cells[2].Range.Text = md.CostoStorico.ToString("C2");
                            row8.Cells[3].Range.Text = md.StimaPerIntero.ToString("C2");
                            r8 += 1;
                        }
                        break;
                    case 61: //tabella pagina 10 su Atti di Disposizione
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableAD = tables[9];

                        for (int nr = 1; nr <= attiDisposiziones.Count() - 1; nr++)
                        {
                            Row row9 = tableAD.Rows.Add(ref missing);
                        }
                        int r9 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (AttiDisposizione md in attiDisposiziones)
                        {
                            Row row9 = tableAD.Rows[r9];
                            row9.Cells[1].Range.Text = md.Descrizione;
                            r9 += 1;
                        }
                        break;
                    case 62://tabella pagina 11 su Massa Debitoria Rilevata
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableSM = tables[10];

                        for (int nr = 1; nr <= speseMese.Count() - 1; nr++)
                        {
                            Row row = tableSM.Rows.Add(ref missing);
                        }
                        int r10 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (SpeseMese md in speseMese)
                        {
                            Row row = tableSM.Rows[r10 + 1];
                            row.Cells[1].Range.Text = md.Descrizione;
                            row.Cells[2].Range.Text = md.Importo.ToString("C2");
                            r10 += 1;
                        }
                        break;

                }

                doc.Bookmarks.Add(bookmark[i], range);
            }



            doc.Save();
            //dispose all
            doc.Close();
            int res = System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
            int res1 = System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            app = null;
            GC.Collect();

            using (var stream = new FileStream(sFileOut, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Modello Proposta Integrato.docx");

        }


        public async Task<IActionResult> GeneraRelazione(string IDRelazione)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "Modello Relazione Integrato.docx";
            var memory = new MemoryStream();
            string sFileOut = Path.GetTempPath() + sFileName;
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "etc", "docx", sFileName));
            file.CopyTo(sFileOut, true);
            Application app = new Microsoft.Office.Interop.Word.Application();
            Document doc = app.Documents.Open(sFileOut);
            //app.Visible = true;
            string[] bookmark = new string[] { "TribunaleAdito", "SezioneTribunaleAdito", "TribunaleAdito2", "SezioneTribunaleAdito2",
                "TribunaleAdito3", "SezioneTribunaleAdito3", "TribunaleAdito4", "SezioneTribunaleAdito4", "TribunaleAdito5", "SezioneTribunaleAdito5",
                "TribunaleAdito6", "SezioneTribunaleAdito6", "Oggetto", "PrimoRicorrente", "PrimoRicorrenteCF", "DomicilioPrimoRicorrente", 
                "SecondoRicorrente", "SecondoRicorrenteCF", "DomicilioSecondoRicorrente", "PrimoRicorrenteA", "PrimoRicorrenteCFA", "DomicilioPrimoRicorrenteA", 
                "SecondoRicorrenteA", "SecondoRicorrenteCFA", "DomicilioSecondoRicorrenteA", "PrimoRicorrenteB", "PrimoRicorrenteCFB", "DomicilioPrimoRicorrenteB", 
                "SecondoRicorrenteB", "SecondoRicorrenteCFB", "DomicilioSecondoRicorrenteB", "AttoDiNomina", "LuogoData", "ProfessionistaNominato", 
                "IndirizzoProfessionista", "RecapitiProfessionista", "RedditoMedioMensile", "NumCompNucleoFam", "TotSpeseCorrentiMensili", "RataMax", "RataMax2", "RateProposte",
                "CauseIndebitamentoeDiligenza", "ResocontoPagamenti5Anni", "Creditore", "DescrizionePIC", "DescrizioneBMRC", "DescrizioneBMC"};


            Tables tables = doc.Tables;

            int appo = tables.Count;
            int righe = tables[1].Rows.Count;
            int bmk = doc.Bookmarks.Count;

            object missing = System.Reflection.Missing.Value;


            Relazioni relazione = await RealmDataStore.Relazioni(IDRelazione);
            IEnumerable<MasseDebitorie> masseDebitorie = await RealmDataStore.ListaMasseDebitorieR(relazione);
            IEnumerable<AnnotazioniAggiuntive> annotazioniAggiuntive = await RealmDataStore.ListaAnnotazioniAggiuntiveR(relazione);
            IEnumerable<SpeseMese> speseMese = await RealmDataStore.ListaSpeseMeseR(relazione);
            IEnumerable<PatrimonioImmobiliare> patrimonioImmobiliare = await RealmDataStore.ListaPatrimonioImmobiliareR(relazione);
            IEnumerable<BeniMobiliRegistrati> beniMobiliRegistrati = await RealmDataStore.ListaBeniMobiliRegistratiR(relazione);
            IEnumerable<BeniMobili> beniMobili = await RealmDataStore.ListaBeniMobiliR(relazione);
            IEnumerable<AttiDisposizione> attiDisposiziones = await RealmDataStore.ListaAttiDisposizioneR(relazione);

            string[,] Riparto1 = new string[masseDebitorie.Count(), 7];
            int r3 = 0;
            double[] Totali = new double[3];
            double importo = 0;
            double TotSpeseMese = 0;

            foreach (MasseDebitorie md in masseDebitorie)
            {
                Riparto1[r3, 0] = md.Creditore;
                Riparto1[r3, 1] = md.Grado.Descrizione;
                Riparto1[r3, 2] = md.Importo.ToString("C2");
                Riparto1[r3, 3] = (md.PercentualeSoddisfo / 100).ToString("P");
                importo = (md.Importo * (md.PercentualeSoddisfo / 100));
                if (md.Grado.Aggregazione == 1)
                {
                    Riparto1[r3, 4] = importo.ToString("C2");
                    Totali[0] += importo;
                }
                else if (md.Grado.Aggregazione == 2)
                {
                    Riparto1[r3, 5] = importo.ToString("C2");
                    Totali[1] += importo;
                }
                else if (md.Grado.Aggregazione == 3)
                {
                    Riparto1[r3, 6] = importo.ToString("C2");
                    Totali[2] += importo;
                }
                r3 += 1;
            }

            foreach (SpeseMese sm in speseMese)
            {
                TotSpeseMese += sm.Importo;
            }

            for (int i = 0; i < bookmark.Length; i++)
            {
                Bookmark bm = doc.Bookmarks[bookmark[i]];
                Microsoft.Office.Interop.Word.Range range = bm.Range;
                switch (i)
                {
                    case 0:
                    case 2:
                    case 4:
                    case 6:
                    case 8:
                    case 10:
                        range.Text = relazione.Tribunale;
                        break;
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case 11:
                        range.Text = relazione.SezioneTribunale;
                        break;
                    case 12:
                        range.Text = relazione.Oggetto;
                        break;
                    case 31:
                        range.Text = relazione.AttoNominaProfessionista;
                        break;
                    case 33:
                        range.Text = relazione.TitoloNominativoProfessionista;
                        break;
                    case 34:
                        range.Text = relazione.IndirizzoProfessionista;
                        break;
                    case 35:
                        range.Text = relazione.RecapitiProfessionista;
                        break;
                    case 32:
                        range.Text = relazione.LuogoData;
                        break;
                    case 36:
                        range.Text = relazione.RedditoMensile.ToString("C2");
                        break;
                    case 37:
                        range.Text = relazione.ComponentiNucleo.ToString();
                        break;
                    case 38:
                        range.Text = TotSpeseMese.ToString("C2");
                        break;
                    case 42:
                        range.Text = relazione.CauseIndebitamentoeDiligenza;
                        break;
                    case 43:
                        range.Text = relazione.ResocontoPagamenti5Anni;
                        break;
                    case 41:
                        range.Text = relazione.RateProposte.ToString();
                        break;
                    case 39:
                    case 40:
                        range.Text = relazione.RataMax.ToString("C2");
                        break;
                    case 13:
                    case 19:
                    case 25:
                        range.Text = ""; // relazione.PrimoRicorrente;
                        break;
                    case 14:
                    case 20:
                    case 26:
                        range.Text = ""; // relazione.PrimoRicorrenteCF;
                        break;
                    case 15:
                    case 21:
                    case 27:
                        range.Text = ""; // relazione.DomicilioPrimoRicorrente;
                        break;
                    case 16:
                    case 22:
                    case 28:
                        range.Text = ""; // relazione.SecondoRicorrente;
                        break;
                    case 17:
                    case 23:
                    case 29:
                        range.Text = ""; // relazione.SecondoRicorrenteCF;
                        break;
                    case 18:
                    case 24:
                    case 30:
                        range.Text = ""; // relazione.DomicilioSecondoRicorrente;
                        break;
                    case 44: //tabella pagina 5 su Massa Debitoria Rilevata
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableMD = tables[1];

                        for (int nr = 1; nr <= masseDebitorie.Count() - 1; nr++)
                        {
                            Row row = tableMD.Rows.Add(ref missing);
                        }
                        int r = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (MasseDebitorie md in masseDebitorie)
                        {
                            Row row = tableMD.Rows[r + 1];
                            row.Cells[1].Range.Text = md.Creditore;
                            row.Cells[2].Range.Text = md.CausaDebito.Descrizione;
                            row.Cells[3].Range.Text = md.Grado.Descrizione;
                            row.Cells[4].Range.Text = md.Importo.ToString("C2");
                            row.Cells[5].Range.Text = (md.PercentualeSoddisfo / 100).ToString("P");
                            r += 1;
                        }
                        break;
                    case 45: //tabella pagina 9 su Patrimonio Immobiliare
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tablePI = tables[2];

                        for (int nr = 1; nr <= patrimonioImmobiliare.Count() - 1; nr++)
                        {
                            Row row6 = tablePI.Rows.Add(ref missing);
                        }
                        int r6 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (PatrimonioImmobiliare md in patrimonioImmobiliare)
                        {
                            Row row6 = tablePI.Rows[r6 + 1];
                            row6.Cells[1].Range.Text = md.Descrizione;
                            row6.Cells[2].Range.Text = md.Superficie.ToString();
                            row6.Cells[3].Range.Text = md.ValoreOmiVan.ToString("C2");
                            row6.Cells[4].Range.Text = md.StimaPerIntero.ToString("C2");
                            r6 += 1;
                        }
                        break;
                    case 46: //tabella pagina 9 su Patrimonio Immobiliare
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableBMR = tables[3];

                        for (int nr = 1; nr <= beniMobiliRegistrati.Count() - 1; nr++)
                        {
                            Row row7 = tableBMR.Rows.Add(ref missing);
                        }
                        int r7 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (BeniMobiliRegistrati md in beniMobiliRegistrati)
                        {
                            Row row7 = tableBMR.Rows[r7 + 1];
                            row7.Cells[1].Range.Text = md.Descrizione;
                            row7.Cells[2].Range.Text = md.Targa;
                            row7.Cells[3].Range.Text = md.Stima.ToString("C2");
                            r7 += 1;
                        }
                        break;
                    case 47: //tabella pagina 9 su Patrimonio Immobiliare
                        //primo passaggio: creare tante righe della tabella per quante sono i record da inserire
                        //range.Text = "";
                        Table tableBM = tables[4];

                        for (int nr = 1; nr <= beniMobili.Count() - 1; nr++)
                        {
                            Row row8 = tableBM.Rows.Add(ref missing);
                        }
                        int r8 = 1;
                        //secondo passaggio: valorizzare i campi in tabella
                        foreach (BeniMobili md in beniMobili)
                        {
                            Row row8 = tableBM.Rows[r8 + 1];
                            row8.Cells[1].Range.Text = md.Descrizione;
                            row8.Cells[2].Range.Text = md.CostoStorico.ToString("C2");
                            row8.Cells[3].Range.Text = md.StimaPerIntero.ToString("C2");
                            r8 += 1;
                        }
                        break;

                }

                doc.Bookmarks.Add(bookmark[i], range);
            }



            doc.Save();
            //dispose all
            doc.Close();
            int res = System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
            int res1 = System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            app = null;
            GC.Collect();

            using (var stream = new FileStream(sFileOut, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Modello Relazione Integrato.docx");

        }



        [HttpGet]
        public async Task<IActionResult> Proposte(int anno = -1)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (anno == -1) anno = DateTime.Now.Year;

            List<SelectListItem> listaAnni = new List<SelectListItem>();
            for (int i = 2015; i <= DateTime.Now.Year + 1; i++)
            {
                SelectListItem si = new SelectListItem { Text = i + "", Value = i + "" };
                listaAnni.Add(si);
            }

            var listaProposte = await RealmDataStore.ListaProposte(anno, HttpContext.Session.GetString("user"));

            HelpProposte helpProposte = new HelpProposte
            {

                ListaAnni = listaAnni,
                Anno = anno,
                ListaProposte = listaProposte
            };

            return View(helpProposte);
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


        [HttpGet]
        public async Task<IActionResult> Calendario(int IDSettimana = -1, int Anno = -1, string Dir = "", bool IsHidden=false)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            DateTime dtPrimoGiornoSettimana = DateTime.Now;
            DateTime dtUltimoGiornoSettimana = dtPrimoGiornoSettimana.AddDays(6);


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

            IEnumerable<Calendario> ListaCalendari = await RealmDataStore.ListaCalendari(esercente, dtPrimoGiornoSettimana);
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
                //calcolo la capienza residua
                string Res = await RealmDataStore.CapienzaResidua(c.ID);
                string[] aCR = Res.Split("#");
                int CR = Convert.ToInt32(aCR[0]);
                bool HasBooking = CR < 0 ? false : true;
                string info = aCR[1];

                foreach (StrutturaCalendario s in ListaStrutturaCalendari)
                {
                    if (c.OraInizio == s.iOrarioCella) //ho trovato un elemento che inizia nella riga
                    {
                        //cerco la colonna giusto
                        switch ((int)c.Data.DayOfWeek)
                        {
                            case 0:  //DOM
                                s.SC0.Calendario = c;
                                s.SC0.TipoCella = 1;
                                s.SC0.Riga = (c.OraFine-c.OraInizio)/intSlot;
                                s.SC0.Colonna = 1;
                                if (s.SC0.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC0.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio+intSlot*(x-1) + "#0");
                                    }
                                }
                                s.SC0.CapienzaResidua = Math.Abs(CR);
                                s.SC0.HasBooking = HasBooking;
                                s.SC0.Info = info;
                                break;
                            case 1:  //LUN
                                s.SC1.Calendario = c;
                                s.SC1.TipoCella = 1;
                                s.SC1.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC1.Colonna = 1;
                                if (s.SC1.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC1.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#1");
                                    }
                                }
                                s.SC1.CapienzaResidua = Math.Abs(CR);
                                s.SC1.HasBooking = HasBooking;
                                s.SC1.Info = info;
                                break;
                            case 2:  //MAR
                                s.SC2.Calendario = c;
                                s.SC2.TipoCella = 1;
                                s.SC2.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC2.Colonna = 1;
                                if (s.SC2.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC2.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#2");
                                    }
                                }
                                s.SC2.CapienzaResidua = Math.Abs(CR);
                                s.SC2.HasBooking = HasBooking;
                                s.SC2.Info = info;
                                break;
                            case 3:  //MER
                                s.SC3.Calendario = c;
                                s.SC3.TipoCella = 1;
                                s.SC3.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC3.Colonna = 1;
                                if (s.SC3.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC3.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#3");
                                    }
                                }
                                s.SC3.CapienzaResidua = Math.Abs(CR);
                                s.SC3.HasBooking = HasBooking;
                                s.SC3.Info = info;
                                break;
                            case 4:  //GIO
                                s.SC4.Calendario = c;
                                s.SC4.TipoCella = 1;
                                s.SC4.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC4.Colonna = 1;
                                if (s.SC4.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC4.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#4");
                                    }
                                }
                                s.SC4.CapienzaResidua = Math.Abs(CR);
                                s.SC4.HasBooking = HasBooking;
                                s.SC4.Info = info;
                                break;
                            case 5:  //VEN
                                s.SC5.Calendario = c;
                                s.SC5.TipoCella = 1;
                                s.SC5.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC5.Colonna = 1;
                                if (s.SC5.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC5.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#5");
                                    }
                                }
                                s.SC5.CapienzaResidua = Math.Abs(CR);
                                s.SC5.HasBooking = HasBooking;
                                s.SC5.Info = info;
                                break;
                            case 6:  //SAB
                                s.SC6.Calendario = c;
                                s.SC6.TipoCella = 1;
                                s.SC6.Riga = (c.OraFine - c.OraInizio) / intSlot;
                                s.SC6.Colonna = 1;
                                if (s.SC6.Riga > 1)
                                {
                                    for (int x = 2; x <= s.SC6.Riga; x++)
                                    {
                                        CelleSpente.Add(c.OraInizio + intSlot * (x - 1) + "#6");
                                    }
                                }
                                s.SC6.CapienzaResidua = Math.Abs(CR);
                                s.SC6.HasBooking = HasBooking;
                                s.SC6.Info = info;
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
                IsHidden = IsHidden
            };

            return View(helpCalendario);
        }


        [HttpGet]
        public async Task<IActionResult> Relazioni(int anno = -1)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (anno == -1) anno = DateTime.Now.Year;

            List<SelectListItem> listaAnni = new List<SelectListItem>();
            for (int i = 2015; i <= DateTime.Now.Year + 1; i++)
            {
                SelectListItem si = new SelectListItem { Text = i + "", Value = i + "" };
                listaAnni.Add(si);
            }

            var listaRelazioni = await RealmDataStore.ListaRelazioni(anno, HttpContext.Session.GetString("user"));

            HelpRelazioni helpRelazioni = new HelpRelazioni
            {

                ListaAnni = listaAnni,
                Anno = anno,
                ListaRelazioni = listaRelazioni
            };

            return View(helpRelazioni);
        }


        [HttpPost]
        public async Task<IActionResult> Proposte(HelpProposte helpProposte)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            int anno = helpProposte.Anno;

            return RedirectToAction(nameof(Proposte), new { anno });
        }

        [HttpPost]
        public async Task<IActionResult> Relazioni(HelpRelazioni helpRelazioni)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            int anno = helpRelazioni.Anno;

            return RedirectToAction(nameof(Relazioni), new { anno });
        }

        public async Task<IActionResult> InserisciRicorrenti(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeRicorrenti == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 1 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeRicorrenti == "Ins")
            {
                await RealmDataStore.InserisciRicorrenti(helpProposta.Proposte.ID, helpProposta.Ricorrenti);
            }
            else if (helpProposta.ModeRicorrenti == "Upd")
            {
                await RealmDataStore.AggiornaRicorrenti(helpProposta.IDRicorrenti, helpProposta.Ricorrenti);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 1 });
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


        public async Task<IActionResult> InserisciRicorrentiR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeRicorrenti == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 1 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeRicorrenti == "Ins")
            {
                await RealmDataStore.InserisciRicorrentiR(helpRelazione.Relazioni.ID, helpRelazione.Ricorrenti);
            }
            else if (helpRelazione.ModeRicorrenti == "Upd")
            {
                await RealmDataStore.AggiornaRicorrenti(helpRelazione.IDRicorrenti, helpRelazione.Ricorrenti);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 1 });
        }


        public async Task<IActionResult> InserisciMasseDebitorie(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeMasseDebitorie == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 4 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeMasseDebitorie == "Ins")
            {
                await RealmDataStore.InserisciMasseDebitorie(helpProposta.Proposte.ID, helpProposta.MasseDebitorie, helpProposta.IDCauseDebito, helpProposta.IDGradi);
            }
            else if (helpProposta.ModeMasseDebitorie == "Upd")
            {
                await RealmDataStore.AggiornaMasseDebitorie(helpProposta.IDMasseDebitorie, helpProposta.MasseDebitorie, helpProposta.IDCauseDebito, helpProposta.IDGradi);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 4 });
        }

        public async Task<IActionResult> InserisciMasseDebitorieR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeMasseDebitorie == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 4 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeMasseDebitorie == "Ins")
            {
                await RealmDataStore.InserisciMasseDebitorieR(helpRelazione.Relazioni.ID, helpRelazione.MasseDebitorie, helpRelazione.IDCauseDebito, helpRelazione.IDGradi);
            }
            else if (helpRelazione.ModeMasseDebitorie == "Upd")
            {
                await RealmDataStore.AggiornaMasseDebitorie(helpRelazione.IDMasseDebitorie, helpRelazione.MasseDebitorie, helpRelazione.IDCauseDebito, helpRelazione.IDGradi);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 4 });
        }

        public async Task<IActionResult> InserisciSpeseMese(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeSpeseMese == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 7 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeSpeseMese == "Ins")
            {
                await RealmDataStore.InserisciSpeseMese(helpProposta.Proposte.ID, helpProposta.SpeseMese);
            }
            else if (helpProposta.ModeSpeseMese == "Upd")
            {
                await RealmDataStore.AggiornaSpeseMese(helpProposta.IDSpeseMese, helpProposta.SpeseMese);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 7 });
        }

        public async Task<IActionResult> InserisciSpeseMeseR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeSpeseMese == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 7 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeSpeseMese == "Ins")
            {
                await RealmDataStore.InserisciSpeseMeseR(helpRelazione.Relazioni.ID, helpRelazione.SpeseMese);
            }
            else if (helpRelazione.ModeSpeseMese == "Upd")
            {
                await RealmDataStore.AggiornaSpeseMese(helpRelazione.IDSpeseMese, helpRelazione.SpeseMese);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 7 });
        }


        public async Task<IActionResult> InserisciAttiDisposizione(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeAttiDisposizione == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 5 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeAttiDisposizione == "Ins")
            {
                await RealmDataStore.InserisciAttiDisposizione(helpProposta.Proposte.ID, helpProposta.AttiDisposizione);
            }
            else if (helpProposta.ModeAttiDisposizione == "Upd")
            {
                await RealmDataStore.AggiornaAttiDisposizione(helpProposta.IDAttiDisposizione, helpProposta.AttiDisposizione);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 5 });
        }

        public async Task<IActionResult> InserisciAttiDisposizioneR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeAttiDisposizione == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 5 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeAttiDisposizione == "Ins")
            {
                await RealmDataStore.InserisciAttiDisposizioneR(helpRelazione.Relazioni.ID, helpRelazione.AttiDisposizione);
            }
            else if (helpRelazione.ModeAttiDisposizione == "Upd")
            {
                await RealmDataStore.AggiornaAttiDisposizione(helpRelazione.IDAttiDisposizione, helpRelazione.AttiDisposizione);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 5 });
        }

        public async Task<IActionResult> InserisciPatrimonioImmobiliare(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModePatrimonioImmobiliare == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 8 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModePatrimonioImmobiliare == "Ins")
            {
                await RealmDataStore.InserisciPatrimonioImmobiliare(helpProposta.Proposte.ID, helpProposta.PatrimonioImmobiliare);
            }
            else if (helpProposta.ModePatrimonioImmobiliare == "Upd")
            {
                await RealmDataStore.AggiornaPatrimonioImmobiliare(helpProposta.IDPatrimonioImmobiliare, helpProposta.PatrimonioImmobiliare);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 8 });
        }

        public async Task<IActionResult> InserisciPatrimonioImmobiliareR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModePatrimonioImmobiliare == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 8 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModePatrimonioImmobiliare == "Ins")
            {
                await RealmDataStore.InserisciPatrimonioImmobiliareR(helpRelazione.Relazioni.ID, helpRelazione.PatrimonioImmobiliare);
            }
            else if (helpRelazione.ModePatrimonioImmobiliare == "Upd")
            {
                await RealmDataStore.AggiornaPatrimonioImmobiliare(helpRelazione.IDPatrimonioImmobiliare, helpRelazione.PatrimonioImmobiliare);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 8 });
        }

        public async Task<IActionResult> InserisciBeniMobiliRegistrati(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeBeniMobiliRegistrati == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 9 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeBeniMobiliRegistrati == "Ins")
            {
                await RealmDataStore.InserisciBeniMobiliRegistrati(helpProposta.Proposte.ID, helpProposta.BeniMobiliRegistrati);
            }
            else if (helpProposta.ModeBeniMobiliRegistrati == "Upd")
            {
                await RealmDataStore.AggiornaBeniMobiliRegistrati(helpProposta.IDBeniMobiliRegistrati, helpProposta.BeniMobiliRegistrati);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 9 });
        }

        public async Task<IActionResult> InserisciBeniMobiliRegistratiR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeBeniMobiliRegistrati == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 9 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeBeniMobiliRegistrati == "Ins")
            {
                await RealmDataStore.InserisciBeniMobiliRegistratiR(helpRelazione.Relazioni.ID, helpRelazione.BeniMobiliRegistrati);
            }
            else if (helpRelazione.ModeBeniMobiliRegistrati == "Upd")
            {
                await RealmDataStore.AggiornaBeniMobiliRegistrati(helpRelazione.IDBeniMobiliRegistrati, helpRelazione.BeniMobiliRegistrati);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 9 });
        }

        public async Task<IActionResult> InserisciBeniMobili(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeBeniMobili == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 10 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeBeniMobili == "Ins")
            {
                await RealmDataStore.InserisciBeniMobili(helpProposta.Proposte.ID, helpProposta.BeniMobili);
            }
            else if (helpProposta.ModeBeniMobili == "Upd")
            {
                await RealmDataStore.AggiornaBeniMobili(helpProposta.IDBeniMobili, helpProposta.BeniMobili);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 10 });
        }

        public async Task<IActionResult> InserisciBeniMobiliR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeBeniMobili == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 10 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeBeniMobili == "Ins")
            {
                await RealmDataStore.InserisciBeniMobiliR(helpRelazione.Relazioni.ID, helpRelazione.BeniMobili);
            }
            else if (helpRelazione.ModeBeniMobili == "Upd")
            {
                await RealmDataStore.AggiornaBeniMobili(helpRelazione.IDBeniMobili, helpRelazione.BeniMobili);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 10 });
        }

        public async Task<IActionResult> InserisciAnnotazioniAggiuntive(HelpProposta helpProposta)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpProposta.ModeAnnotazioniAggiuntive == "Cancel") return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 6 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID });

            if (helpProposta.ModeAnnotazioniAggiuntive == "Ins")
            {
                await RealmDataStore.InserisciAnnotazioniAggiuntive(helpProposta.Proposte.ID, helpProposta.AnnotazioniAggiuntive);
            }
            else if (helpProposta.ModeAnnotazioniAggiuntive == "Upd")
            {
                await RealmDataStore.AggiornaAnnotazioniAggiuntive(helpProposta.IDAnnotazioniAggiuntive, helpProposta.AnnotazioniAggiuntive);
            }
            return RedirectToAction(nameof(Proposta), new { helpProposta.Proposte.ID, tab = 6 });

        }

        public async Task<IActionResult> InserisciAnnotazioniAggiuntiveR(HelpRelazione helpRelazione)
        {

            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            if (helpRelazione.ModeAnnotazioniAggiuntive == "Cancel") return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 6 });

            if (!ModelState.IsValid) return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID });

            if (helpRelazione.ModeAnnotazioniAggiuntive == "Ins")
            {
                await RealmDataStore.InserisciAnnotazioniAggiuntiveR(helpRelazione.Relazioni.ID, helpRelazione.AnnotazioniAggiuntive);
            }
            else if (helpRelazione.ModeAnnotazioniAggiuntive == "Upd")
            {
                await RealmDataStore.AggiornaAnnotazioniAggiuntive(helpRelazione.IDAnnotazioniAggiuntive, helpRelazione.AnnotazioniAggiuntive);
            }
            return RedirectToAction(nameof(Relazione), new { helpRelazione.Relazioni.ID, tab = 6 });

        }



        public async Task<IActionResult> RelazioneDaProposta(HelpRelazioni helpRelazioni)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.RelazioneDaProposta(helpRelazioni.IDPropostaRiferimento, HttpContext.Session.GetString("user"));

            return RedirectToAction(nameof(Relazioni), new { helpRelazioni.Anno });
        }

        public async Task<IActionResult> EliminaMasseDebitorie(string ID, string IDMasseDebitorie, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaMasseDebitorie(IDMasseDebitorie);

            if (bProp) 
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 4 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 4 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaAttiDisposizione(string ID, string IDAttiDisposizione, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaAttiDisposizione(IDAttiDisposizione);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 5 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 5 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaSpeseMese(string ID, string IDSpeseMese, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaSpeseMese(IDSpeseMese);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 7 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 7 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaAnnotazioniAggiuntive(string ID, string IDAnnotazioniAggiuntive, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaAnnotazioniAggiuntive(IDAnnotazioniAggiuntive);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 6 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 6 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaPatrimonioImmobiliare(string ID, string IDPatrimonioImmobiliare, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaPatrimonioImmobiliare(IDPatrimonioImmobiliare);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 8 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 8 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaBeniMobiliRegistrati(string ID, string IDBeniMobiliRegistrati, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaBeniMobiliRegistrati(IDBeniMobiliRegistrati);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 9 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 9 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaBeniMobili(string ID, string IDBeniMobili, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaBeniMobili(IDBeniMobili);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 10 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 10 }); //relazioni
            }
        }

        public async Task<IActionResult> EliminaRicorrenti(string ID, string IDRicorrenti, bool bProp = true)
        {
            if (!CheckUser()) return RedirectToAction(nameof(Login));
            if (CheckRoleAgente()) return RedirectToAction(nameof(SchedaAgente), new { ID = HttpContext.Session.GetString("idagente") });

            await RealmDataStore.EliminaRicorrenti(IDRicorrenti);

            if (bProp)
            {
                return RedirectToAction(nameof(Proposta), new { ID, tab = 1 });
            }
            else
            {
                return RedirectToAction(nameof(Relazione), new { ID, tab = 1 }); //relazioni
            }
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

    }
}
