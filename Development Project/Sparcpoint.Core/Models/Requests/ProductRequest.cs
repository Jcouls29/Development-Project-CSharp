using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.Requests
{
    public class ProductRequest
    {
        //EVAL: Omitting non-logical search items such as image URIs and instanceId...
        //could add if there was/is a use case for it. 

        //EVAL: should add additional search functionality as interface (pagination, sorting, etc...)
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValidSkus { get; set; }

        public List<string> Categories { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}