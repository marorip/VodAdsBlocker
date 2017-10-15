using System.Collections.Generic;
using System.Xml.Serialization;

namespace VodAdsBlocker.Modules
{
    public class VodFilters
    {
        [XmlElement("Filter")]
        public List<Filter> Filters { get; set; }
    }

    public class Filter
    {
        public string Query { get; set; } 
        public string Response { get; set; } 
    }
}
