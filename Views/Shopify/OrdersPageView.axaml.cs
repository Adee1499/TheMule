using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace TheMule.Views.Shopify
{
    public partial class OrdersPageView : UserControl
    {
        private Point? _startPosition;
        private Point _imageOffset;

        public OrdersPageView() {
            InitializeComponent();
        }

        private void OnImagePointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(canvas) is { } point)
            {
                // Capture the current position when the user clicks on the canvas
                _startPosition = point.Position;
                _imageOffset = new Point(Canvas.GetLeft(movableImage), Canvas.GetTop(movableImage));
                Debug.WriteLine(_imageOffset);
            }
        }

        private void OnImagePointerMoved(object sender, PointerEventArgs e)
        {
            if (_startPosition.HasValue)
            {
                if (e.GetCurrentPoint(canvas) is { } point)
                {
                    // Calculate the new position of the image based on the pointer movement
                    var newPosition = new Point(
                        _imageOffset.X + point.Position.X - _startPosition.Value.X,
                        _imageOffset.Y + point.Position.Y - _startPosition.Value.Y);

                    // Move the image to the new position within the canvas
                    Canvas.SetLeft(movableImage, newPosition.X);
                    Canvas.SetTop(movableImage, newPosition.Y);
                }
            }
        }

        private void OnImagePointerReleased(object sender, PointerReleasedEventArgs e)
        {
            _startPosition = null;
        }

        private void OnRenderButtonClick(object sender, RoutedEventArgs e)
        {
            var pixelSize = new PixelSize(3319 * 2, 3761 * 2);
            var size = new Size(canvas.Width, canvas.Height);
            using (RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize, new Vector(1200, 1200)))
            {
                canvas.Measure(size);
                canvas.Arrange(new Rect(size));
                bitmap.Render(canvas);
                bitmap.Save("output.png");
            }
        }
    }
}
