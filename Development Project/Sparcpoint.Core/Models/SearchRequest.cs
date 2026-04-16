using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Web.Models
{
    public class SearchRequest
    {
        public int Take { get; private set; }

        // TODO
        public int Skip { get; private set; }

        public string Search { get; private set; }

        public Dictionary<string, string> Filters { get; private set; }

        [JsonConstructor]
        public SearchRequest(int skip, int take, string search)
        {
            this.Skip = skip;
            this.Take = take;
            this.Search = search;
            this.Filters = ParseFilters();
        }

        public string GetFilterValueAsString(string filterName)
        {
            var key = filterName.ToLower();
            if (Filters.ContainsKey(key))
            {
                var filterValue = this.Filters[key];
                return filterValue;
            }
            return null;
        }

        private Dictionary<string, string> ParseFilters()
        {
            var filters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(this.Search))
            {
                return filters;
            }

            var filterValues = this.Search
                .Split(new[] { " AND " }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            foreach (var a in filterValues)
            {
                var filter = a;
                
                if (filter.StartsWith("(") && filter.EndsWith(")"))
                {
                    filter = filter.Substring(1, filter.Length - 2);
                }

                var parts = filter.Split(new[] { ':' }, 2);
                
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().ToLower();
                    var rawValue = parts[1].Trim();
                    var unescapedValue = UnescapeSearchValue(rawValue);

                    filters.Add(key, unescapedValue);
                }
            }

            return filters;
        }

        public Dictionary<string, string> ParseAttrsFilters()
        {
            var attrs = this.GetFilterValueAsString("attrs");

            if (string.IsNullOrEmpty(attrs))
            {
                return new Dictionary<string, string>();
            }

            return attrs.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Split(new[] { '=' }, 2))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
        }

        private static string UnescapeSearchValue(string searchValue) => searchValue.Replace(@"\(", "(").Replace(@"\)", ")");
    }
}