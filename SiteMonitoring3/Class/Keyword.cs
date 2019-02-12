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
    }
}
