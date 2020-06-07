using System;
using System.Collections.Generic;
using CDQ.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CDQ.Models.Helper
{
    public class HelpUtente
    {
        public Utente Utente { get; set; }

        public string Message {get;set;}

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}
