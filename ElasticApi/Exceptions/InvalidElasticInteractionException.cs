using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticApi.Exceptions
{
    public class InvalidElasticInteractionException :Exception
    {
        public InvalidElasticInteractionException(string message) 
            :base(message){}
    }
}