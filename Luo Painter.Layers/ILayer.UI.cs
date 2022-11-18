using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public partial interface ILayer : ILayerRender, INotifyPropertyChanged, ICommand, IDisposable
    {

        /// <summary>
        /// <see cref="TextBlock.Text" />
        /// </summary>
        string UIBlendMode { get; }

        /// <summary>
        /// <see cref="ColumnDefinition.MinWidth" />
        /// </summary>
        double UIDepth { get; }

        /// <summary>
        /// <see cref="UIElement.Opacity" />
        /// </summary>
        double UIVisibility { get; }

        /// <summary>
        /// <see cref="RotateTransform.Angle" />
        /// </summary>
        double UIIsExpand { get; }

    }
}