using System;
using CDQ.Models;

namespace CDQ.Models.Helper
{
    public class HelpStatus
    {
        public Status Status { get; set; }

        public bool Ins { get; set; } = false;
        public bool Mod { get; set; } = false;
        public bool Del { get; set; } = false;


    }
}
