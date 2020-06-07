using System;
using System.Threading.Tasks;
using CDQ.Services;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using CDQ.Models;

namespace CDQ.Hubs
{
    public class DataListHub : Hub
    {
        //public async Task SearchAssociato(string txt,string provinciale)
        //{
        //    var lista = await RealmDataStore.ListaAssociati(txt, null, provinciale);
        //    var ids = new List<string>() { };
        //    var denominazioni = new List<string>() { };
        //    var codiceFiscali = new List<string>() { };
        //    var cognomi = new List<string>() { };
        //    var nomi = new List<string>() { };
        //    var nSoci = new List<string>() { };

        //    int n = 0;

        //    foreach (Associato associato in lista)
        //    {
        //        n++;
        //        ids.Add(associato.ID ?? "");
        //        denominazioni.Add(associato.Denominazione ?? "");
        //        codiceFiscali.Add(associato.CodiceFiscale ?? "");
        //        cognomi.Add(associato.Cognome ?? "");
        //        nomi.Add(associato.Nome ?? ""); ;
        //        nSoci.Add(associato.NSocio ?? "");
        //    }
        //    await Clients.Caller.SendAsync("SearchAssociatoResult", n, ids,denominazioni, codiceFiscali,cognomi,nomi,nSoci);            
        //}

        //public async Task SearchAgente(string txt, string provinciale)
        //{
        //    var lista = await RealmDataStore.ListaAgenti(txt, provinciale);
        //    var ids = new List<string>() { };
        //    var denominazioni = new List<string>() { };
        //    var codici = new List<string>() { };

        //    int n = 0;

        //    foreach (Agente agente in lista)
        //    {
        //        n++;
        //        ids.Add(agente.ID ?? "");
        //        denominazioni.Add(agente.Nominativo ?? "");
        //        codici.Add(agente.Codice ?? "");
                
        //    }
        //    await Clients.Caller.SendAsync("SearchAgenteResult", n, ids, denominazioni, codici);
        //}

        //public async Task SearchAteco(string txt)
        //{
        //    var lista = await RealmDataStore.ListaAteco(txt);
        //    var ids = new List<string>() { };
        //    var descrizioni = new List<string>() { };
        //    var rischi = new List<string>() { };

        //    int n = 0;

        //    foreach (TipoAteco tipoAteco in lista)
        //    {
        //        n++;
        //        ids.Add(tipoAteco.Codice);
        //        descrizioni.Add(tipoAteco.Descrizione);
        //        rischi.Add(tipoAteco.Rischio);

        //    }
        //    await Clients.Caller.SendAsync("SearchAtecoResult", n, ids, descrizioni, rischi);
        //}

    }
}
