using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CDQ.Models;
using CDQ.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpProposta
    {
        public List<SelectListItem> ListaStatus { get; set; }
        public List<SelectListItem> ListaCauseDebito { get; set; }
        public List<SelectListItem> ListaGradi { get; set; }
        public List<SelectListItem> ListaTipiPratica { get; set; }
        public bool Ins { get; set; }
        public int Tab { get; set; }

        public string TP_CauseIndebitamentoeDiligenza { get; set; }
        public string TP_ResocontoPagamenti5Anni { get; set; }

        public Ricorrenti Ricorrenti { get; set; }
        public MasseDebitorie MasseDebitorie { get; set; }
        public SpeseMese SpeseMese { get; set; }
        public PatrimonioImmobiliare PatrimonioImmobiliare { get; set; }
        public string IDMasseDebitorie { get; set; }
        public string IDRicorrenti { get; set; }
        public string IDAttiDisposizione { get; set; }
        public string IDAttiDisposizioneProposta { get; set; }
        public string IDAnnotazioniAggiuntive { get; set; }
        public string IDAnnotazioniAggiuntiveProposta { get; set; }
        public string IDSpeseMese { get; set; }
        public string IDASpeseMeseProposta { get; set; }
        public string IDPatrimonioImmobiliare { get; set; }
        public string IDPatrimonioImmobiliareProposta { get; set; }
        public string IDBeniMobiliRegistrati { get; set; }
        public string IDBeniMobiliRegistratiProposta { get; set; }
        public string IDBeniMobili { get; set; }
        public string IDBeniMobiliProposta { get; set; }      
        //
        public string IDCauseDebito { get; set; }
        public string IDStatus { get; set; }
        public string IDGradi { get; set; }
        public string IDTipiPratica { get; set; }


        public Proposte Proposte { get; set; }

        public IEnumerable<MasseDebitorie> ListaMasseDebitorie;
        public IEnumerable<SpeseMese> ListaSpeseMese;
        public IEnumerable<PatrimonioImmobiliare> ListaPatrimonioImmobiliare;
        public IEnumerable<Ricorrenti> ListaRicorrenti;

        [Display(Name = "Spese/Consulenze")]
        [DataType(DataType.Currency)]
        public double TotaleSpeseConsulenze { get; set; }


        public string ModeMasseDebitorie { get; set; }
        public string ModeRicorrenti { get; set; }
        public string ModeAttiDisposizione { get; set; }
        public string ModeAnnotazioniAggiuntive { get; set; }
        public string ModeSpeseMese { get; set; }
        public string ModePatrimonioImmobiliare { get; set; }
        public string ModeBeniMobiliRegistrati { get; set; }
        public string ModeBeniMobili { get; set; }
    }
}
