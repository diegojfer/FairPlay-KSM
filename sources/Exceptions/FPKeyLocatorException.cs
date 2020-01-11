using System;

namespace FoolishTech.FairPlay.Exceptions
{
    public sealed class FPKeyLocatorException: FPException
    {
        public FPKeyLocatorException(string context, string message, Exception ex = null) : base(context, message, ex) { }
    }
}
