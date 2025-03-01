namespace Awiz.Core
{
    public class Config
    {
        /// <summary>
        /// When set to true, the associations between classes are shown
        /// </summary>
        public bool EnableAssociations { get; set; }

        public AllowedLists Namespaces { get; set; } = new AllowedLists();
    }

    public class AllowedLists
    {
        public List<string> Blacklist { get; set; } = new List<string>();

        public List<string> Whitelist { get; set; } = new List<string>();
    }
}
