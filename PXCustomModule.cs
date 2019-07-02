using System;
using PX.Data;

namespace PLNSC
{
    public class PXCustomModule
    {
        public const string SC = "SC";

        public class sc : Constant<string>
        {
            public sc() : base("SC")
            {
            }
        }

        public class sc_ : Constant<string>
        {
            public sc_() : base("SC%")
            {
            }
        }
    }
}