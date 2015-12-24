namespace Mizer
{
    public class Set
    {
        public string Block { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string URL { get; set; }

        public Set(string block, string name, string code, string url)
        {
            Block = block;
            Name = name;
            Code = code;
            URL = url;
        }
    }

    public class Card
    {
    }
}