using System;

namespace FoolishTech.FairPlay.Exceptions
{
    public class FPException: Exception
    {
        public string Context { get; private set; }

        public FPException(string context, string message): base(message)
        { 
            this.Context = context;
        }
        public FPException(string context, string message, Exception innerException): base(message, innerException) 
        { 
            this.Context = context;
        }
    }
}
