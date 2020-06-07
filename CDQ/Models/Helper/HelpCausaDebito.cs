using System;
using CDQ.Models;

namespace CDQ.Models.Helper
{
    public class HelpCausaDebito
    {
        public CausaDebito CausaDebito { get; set; }

        public bool Ins { get; set; } = false;
        public bool Mod { get; set; } = false;
        public bool Del { get; set; } = false;

    }
}
