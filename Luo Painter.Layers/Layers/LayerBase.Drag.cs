namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase
    {

        public double Depth
        {
            get => this.depth;
            set
            {
                this.depth = value;
                this.OnPropertyChanged(nameof(Depth)); // Notify 
            }
        }
        private double depth;

        public bool IsExist
        {
            get => this.isExist;
            set
            {
                this.isExist = value;
                this.OnPropertyChanged(nameof(IsExist)); // Notify 
            }
        }
        private bool isExist = true;

        public bool IsExpand
        {
            get => this.isExpand;
            set
            {
                this.isExpand = value;
                this.OnPropertyChanged(nameof(IsExpand)); // Notify 

                foreach (ILayer child in this.Children)
                {
                    child.Exist(value);
                }
            }
        }
        private bool isExpand = true;


        public void Arrange(double depth)
        {
            if (this.Depth == depth) return;
            this.Depth = depth;

            this.ArrangeChildren(depth + 40);
        }
        private void ArrangeChildren(double depth)
        {
            foreach (ILayer child in this.Children)
            {
                child.Arrange(depth);
            }
        }

        public void Exist(bool isExist)
        {
            if (this.IsExist == isExist) return;
            this.IsExist = isExist;

            if (this.IsExpand)
            {
                foreach (ILayer child in this.Children)
                {
                    child.Exist(isExist);
                }
            }
            else
            {
                foreach (ILayer child in this.Children)
                {
                    child.Exist(false);
                }
            }
        }

    }
}