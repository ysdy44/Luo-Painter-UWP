﻿using System;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase
    {

        public int Depth
        {
            get => this.depth;
            set
            {
                this.depth = value;
                this.OnPropertyChanged(nameof(Depth)); // Notify 
            }
        }
        private int depth;

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
            }
        }
        private bool isExpand;

        private bool StartingIsExpand;
        public void CacheIsExpand()
        {
            this.StartingIsExpand = this.IsExpand;
            if (this.IsExpand)
            {
                this.IsExpand = false;
                foreach (ILayer child in this.Children)
                {
                    child.Exist(false);
                }
            }
        }
        public void ApplyIsExpand()
        {
            if (this.StartingIsExpand)
            {
                this.IsExpand = true;
                foreach (ILayer child in this.Children)
                {
                    child.Exist(true);
                }
            }
        }

        public void Arrange(int depth)
        {
            if (depth is 0)
            {
                this.Depth = 0;

                this.ArrangeChildren(1);
            }
            else if (this.Depth == depth) return;
            else
            {
                this.Depth = depth;

                this.ArrangeChildren(depth + 1);
            }
        }
        private void ArrangeChildren(int depth)
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

        //@Command
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => this.Children.Count is 0 is false;
        public void Execute(object parameter)
        {
            if (this.IsExpand)
            {
                this.IsExpand = false;
                foreach (ILayer child in this.Children)
                {
                    child.Exist(true);
                }
            }
            else
            {
                this.IsExpand = true;
                foreach (ILayer child in this.Children)
                {
                    child.Exist(false);
                }
            }
        }

    }
}