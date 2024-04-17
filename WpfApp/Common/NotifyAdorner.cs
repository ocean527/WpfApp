using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WpfApp.Common
{
    public class NotifyAdorner : Adorner
    {
        private VisualCollection _visuals;
        private Canvas _canvas;
        private Image _image;
        private TextBlock _toolTip;

        public NotifyAdorner(UIElement adornedElement, string errorMessage) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);

            _image = new Image()
            {
                Width = 16,
                Height = 16,
                Source = new BitmapImage(new Uri("/warning.png", UriKind.RelativeOrAbsolute))
            };

            _toolTip = new TextBlock() { Text = errorMessage };
            _image.ToolTip = _toolTip;

            _canvas = new Canvas();
            _canvas.Children.Add(_image);
            _visuals.Add(_canvas);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        public void ChangeToolTip(string errorMessage)
        {
            _toolTip.Text = errorMessage;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _canvas.Arrange(new Rect(finalSize));
            _image.Margin = new Thickness(finalSize.Width + 3, 0, 0, 0);

            return base.ArrangeOverride(finalSize);
        }
    }
}
