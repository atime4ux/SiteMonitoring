namespace LibSiteMonitoring.Model
{
    public class Keyword
    {
        public string requireKeywords { get; set; }
        public string optionKeywords { get; set; }
        public string exceptKeywords { get; set; }
        public int minPrice { get; set; }
        public int maxPrice { get; set; }

        public Keyword(string require, string option, string except, int min, int max)
        {
            this.requireKeywords = require;
            this.optionKeywords = option;
            this.exceptKeywords = except;
            this.minPrice = min;
            this.maxPrice = max;
        }
    }
}
