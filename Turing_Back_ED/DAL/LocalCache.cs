using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.DAL
{
    public class LocalCache
    {
        public readonly Dictionary<string, object> Session = new Dictionary<string, object>();
    }
}
