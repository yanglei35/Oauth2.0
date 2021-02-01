using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YLOAuth2.Common
{
    public class MyException :Exception
    {

        public MyException(string message) : base(message)
        {

        }

        public MyException(string message, Exception innerException) : base(message, innerException)
        {

        }


        public override string ToString()
        {
            return base.Message.ToString();
        }
    }
}