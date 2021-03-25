using System;
using System.Runtime.Serialization;

namespace RStein.HDO
{
  public class HdoException : Exception
  {
    public HdoException()
    {
    }

    protected HdoException(SerializationInfo info,
                StreamingContext context) : base(info, context)
    {
    }

    public HdoException(string message) : base(message)
    {
    }

    public HdoException(string message,
             Exception innerException) : base(message, innerException)
    {
    }
  }
}