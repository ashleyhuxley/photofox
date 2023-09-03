namespace PhotoFox.Model
{
    public class Folder
    {
        public Folder(string name, string parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        public string Name { get; set; }
        public string Parent { get; set; }
    }
}
