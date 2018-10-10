using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.BusinessLogic
{
    public interface ISayHelloService
    {
        string SayHello();
    }

    public class SayHelloService : ISayHelloService
    {
        public string SayHello()
        {
            return "Hello";
        }
    }
}