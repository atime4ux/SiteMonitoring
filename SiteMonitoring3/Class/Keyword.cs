using System.Collections.Generic;
using Newtonsoft.Json;

namespace SiteMonitoring3
{
    public class Keyword
    {
        public string requireKeywords { get; set; }
        public string optionKeywords { get; set; }
        public string exceptKeywords { get; set; }

        public Keyword(string require, string option, string except)
        {
            this.requireKeywords = require;
            this.optionKeywords = option;
            this.exceptKeywords = except;
        }

        public List<Keyword> GetListFromJson(string json)
        {
            List<Keyword> result = JsonConvert.DeserializeObject<List<Keyword>>(json);
            return result;
        }
    }
}
