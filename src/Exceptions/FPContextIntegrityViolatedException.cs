using System;

namespace FoolishTech.FairPlay.Exceptions
{
    public sealed class FPContextIntegrityViolatedException: FPException
    {
        public FPContextIntegrityViolatedException(string context, string message, Exception ex = null) : base(context, message, ex) { }
    }
}
