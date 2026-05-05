using System.Collections.Generic;

namespace Interview.Web.Models.Requests
{
    public class GetCountByAttributesRequest
    {
        public Dictionary<string, string> Attributes { get; set; }
    }
}
