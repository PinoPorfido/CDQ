using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CDQ.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpSchedaAgente{ 

        //public Agente Agente { get; set; }

        //public IEnumerable<string> ElencoAssociatiServizio{ get; set; }
        //public List<ListaServiziAgente> ListaServizi { get; set; }
        //public IEnumerable<Ripartizione> ListaRipartizioni { get; set; }
        //public IEnumerable<PagamentoAgente> ListaPagamentiAgente { get; set; }

        //public IEnumerable<ConsulenzaServizio> ListaServiziTotali { get; set; }
        //public IEnumerable<Ripartizione> ListaRipartizioniTotali { get; set; }
        //public IEnumerable<PagamentoAgente> ListaPagamentiAgenteTotali { get; set; }

        //public IEnumerable<Agente> ListaSubAgenti{ get; set; }
        //public IEnumerable<Associato> ListaAssociati { get; set; }
        //public List<LinkAngente> ListaLink { get; set; }

        public List<SelectListItem> ListaMesi { get; set; }
        public List<SelectListItem> ListaAnni { get; set; }

        public int Anno { get; set; }
        public int Mese { get; set; }
        public string DescMese { get; set; }
        //public PagamentoAgente PagamentoAgente { get; set; }
        public string MovimentiCollegati { get; set; }


        public bool NuovoPagamento { get; set; }
        public bool EliminaPagamento { get; set; }
        public string IDPagamentoAgente { get; set; }

        public bool ErroreValidazione { get; set; }
        public bool IsAdmin { get; set; }

        public bool IsPosizioniAperte { get; set; }


        public int Tab { get; set; }

        [DataType(DataType.Currency)]
        public double TotaleRipartizioni
        {
            get
            {
                //if (ListaRipartizioni == null) return 0;

                //double totale = 0;
                //foreach(Ripartizione rip in ListaRipartizioni)
                //{
                //    totale += rip.Importo;
                //}
                return 0;
            }
        }

        //[DataType(DataType.Currency)]
        //public double TotaleServizi
        //{
        //    get
        //    {
        //        if (ListaServizi == null) return 0;

        //        double totale = 0;
        //        foreach (ListaServiziAgente ser in ListaServizi)
        //        {
        //            totale += ser.ConsulenzaServizio != null ? ser.ConsulenzaServizio.ImportoNetto : 0;
        //            totale += ser.SpesaServizio!=null ? ser.SpesaServizio.Importo : 0;
        //        }
        //        return totale;
        //    }
        //}

        [DataType(DataType.Currency)]
        public double TotalePagamentiAgente
        {
            get
            {
                //if (ListaPagamentiAgente == null) return 0;

                //double totale = 0;
                //foreach (PagamentoAgente pag in ListaPagamentiAgente)
                //{
                //    totale += pag.Importo;
                //}
                return 0;
            }
        }


        [DataType(DataType.Currency)]
        public double TotaleRipartizioniTotali
        {
            get
            {
                //if (ListaRipartizioniTotali == null) return 0;

                //double totale = 0;
                //foreach (Ripartizione rip in ListaRipartizioniTotali)
                //{
                //    totale += rip.Importo;
                //}
                return 0;
            }
        }

        [DataType(DataType.Currency)]
        public double TotaleServiziTotali
        {
            get
            {
                //if (ListaServiziTotali == null) return 0;

                //double totale = 0;
                //foreach (ConsulenzaServizio ser in ListaServiziTotali)
                //{
                //    totale += ser.ImportoNetto;
                //}
                return 0;
            }
        }

        [DataType(DataType.Currency)]
        public double TotalePagamentiAgenteTotali
        {
            get
            {
                //if (ListaPagamentiAgenteTotali == null) return 0;

                //double totale = 0;
                //foreach (PagamentoAgente pag in ListaPagamentiAgenteTotali)
                //{
                //    totale += pag.Importo;
                //}
                return 0;
            }
        }

        [DataType(DataType.Currency)]
        public double SaldoTotale
        {
            get
            {
                return TotaleRipartizioniTotali + TotaleServiziTotali - TotalePagamentiAgenteTotali;
            }
        }
    }
}
