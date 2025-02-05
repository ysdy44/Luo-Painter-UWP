namespace OptionTypeGenerator
{
    public class Node : List<Node>
    {
        public static Node Empty { get; } = new Node();

        public bool IsEmpty => string.IsNullOrEmpty(this.Title);

        private Node()
        {
        }

        public Node(string title, Bitwise bitwise = Bitwise.B8,
            bool IsItemClickEnabled = false,
            bool ExistIcon = false,
            bool ExistThumbnail = false,
            bool HasMenu = false,
            bool HasPreview = false,
            bool HasDifference = false,
            bool WithState = false,
            bool WithTransform = false)
        {
            this.Title = title;
            this.Bitwise = bitwise;

            this.IsItemClickEnabled = IsItemClickEnabled;
            this.ExistIcon = ExistIcon;
            this.ExistThumbnail = ExistThumbnail;
            this.HasMenu = HasMenu;
            this.HasPreview = HasPreview;
            this.HasDifference = HasDifference;
            this.WithState = WithState;
            this.WithTransform = WithTransform;
        }

        public string Title { get; }
        public Bitwise Bitwise { get; }

        public bool IsItemClickEnabled { get; }
        public bool ExistIcon { get; }
        public bool ExistThumbnail { get; }
        public bool HasMenu { get; }
        public bool HasPreview { get; }
        public bool HasDifference { get; }
        public bool WithState { get; }
        public bool WithTransform { get; }
    }
}