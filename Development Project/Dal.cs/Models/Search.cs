using System.Collections.Generic;

namespace Dal.Models
{
    public class Search
    {
        public List<Products> Products { get; set; }
        public List<SearchCriteria> SearchCriteria { get; set; }
        public string CriteriaValue;
        public string SelectedCriteria;
    }
}
