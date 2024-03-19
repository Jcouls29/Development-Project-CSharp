using System.Collections.Generic;

namespace Interview.Web.DTO
{
    public class SearchCriteriaDto
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public List<GeneralDetailDto> GeneralDetails { get; set; }
        public string Description { get; set; }
    }

    public class GeneralDetailDto
    {
        public string Key { get; set;}
        public string Value { get; set;}
    }
}
