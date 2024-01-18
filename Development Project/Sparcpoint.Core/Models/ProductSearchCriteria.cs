using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public class ProductSearchCriteria
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public ProductSearchCriteria()
        {
            Metadata = new Dictionary<string, string>();
        }

        /// <summary>
        /// This property is used to build the query string for the search
        /// </summary>
        /// Eval <remarks>Fleshing out an idea to do a map reduce type algo,
        /// and have a sorted index to compare this against</remarks>
        public string QueryString
        {
            get
            {
                var queryString = string.Empty;

                if (!string.IsNullOrWhiteSpace(Name))
                {
                    queryString += $"name={Name}&";
                }

                if (!string.IsNullOrWhiteSpace(Category))
                {
                    queryString += $"category={Category}&";
                }

                if (Metadata != null && Metadata.Count > 0)
                {
                    foreach (var item in Metadata)
                    {
                        queryString += $"metadata.{item.Key}={item.Value}&";
                    }
                }

                return queryString.TrimEnd('&');
            }
        }   
    }
}
