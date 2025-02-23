namespace Awiz.Core
{
    public class Config
    {
        public AllowedLists Namespaces { get; set; } = new AllowedLists();
    }

    public class AllowedLists
    {
        public List<string> Blacklist { get; set; } = new List<string>();

        public List<string> Whitelist { get; set; } = new List<string>();
    }
}
