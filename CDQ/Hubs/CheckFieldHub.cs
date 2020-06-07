using System;
using System.Threading.Tasks;
using CDQ.Services;
using Microsoft.AspNetCore.SignalR;

namespace CDQ.Hubs
{
    public class CheckFieldHub : Hub
    {
        public async Task CheckCF(string cf)
        {
            bool cfDouble=await RealmDataStore.CheckCFDuplicato(cf);
            await Clients.Caller.SendAsync("CheckCFResult", cfDouble);
        }

        public async Task CheckRelazioneDaProposta(string IDRelazione, string IDSafeCode, bool chiamante)
        {
            byte tipo = await RealmDataStore.CheckRelazioneDaProposta(IDRelazione, IDSafeCode);
            await Clients.Caller.SendAsync("CheckRPResult", tipo, chiamante);
        }

        public async Task ValuesFromCF(string cf, byte tipo)
        {
            string Valori = await RealmDataStore.ValoriDaCF(cf.ToUpper());

            string[] words = Valori.Split(";");
            string sComune = words[0];
            string sProvincia = words[1];
            string sData = words[2];
            string sSesso = words[3];
            if (sComune == "") tipo = 10; //valore che indica che il risultato non può essere usato
            await Clients.Caller.SendAsync("ValuesFromCFResult", sData, sComune, sProvincia, sSesso, tipo);
        }
    }
}
