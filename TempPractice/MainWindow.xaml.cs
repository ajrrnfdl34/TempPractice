using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TempPractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BitmapSource ImageSource { get; set; }

        public MainWindow()
        {
            var bitmap = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Pbgra32, null);

            var destTopLeftPixelX = 0;
            var destTopLeftPixelY = 0;
            var destBottomRightPixelX = 20;
            var destBottomRightPixelY = 20;
            var destRectPixelWidth = destBottomRightPixelX - destTopLeftPixelX;
            var destRectPixelHeight = destBottomRightPixelY - destTopLeftPixelY;
            var destRectPixel = new Int32Rect(destTopLeftPixelX, destTopLeftPixelY, destRectPixelWidth, destRectPixelHeight);

            Shape shape = new Ellipse();
            shape.Width = destRectPixelWidth;
            shape.Height = destRectPixelHeight;
            shape.Fill = Brushes.Red;
            //shape.Stroke = Brushes.Yellow;
            //shape.StrokeThickness = 2;

            // Render.
            shape.Measure(new Size(destRectPixelWidth, destRectPixelHeight));
            shape.Arrange(new Rect(0, 0, destRectPixelWidth, destRectPixelHeight));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(System.Windows.Media.Brushes.Transparent, null, new System.Windows.Rect(0, 0, destRectPixelWidth, destRectPixelHeight));
                drawingContext.DrawGeometry(shape.Fill, new Pen(shape.Stroke, shape.StrokeThickness), shape.RenderedGeometry);
            }

            var renderTargetBitmap = new RenderTargetBitmap(destRectPixelWidth, destRectPixelHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format);
            renderTargetBitmap.Render(drawingVisual);

            var srcBoundaryRect = new Int32Rect(0, 0, destRectPixelWidth, destRectPixelHeight);
            const int PIXEL_BYTE_SIZE = 4;
            var destStride = bitmap.BackBufferStride;
            var destBufferSize = destStride * bitmap.PixelHeight;
            var destTopLeftPixelPtr = bitmap.BackBuffer + destStride * destTopLeftPixelY + PIXEL_BYTE_SIZE * destTopLeftPixelX;

            renderTargetBitmap.CopyPixels(srcBoundaryRect, destTopLeftPixelPtr, destBufferSize, destStride);

            try
            {
                bitmap.Lock();
                bitmap.AddDirtyRect(destRectPixel);
            }
            finally
            {
                bitmap.Unlock();
            }

            ImageSource = bitmap;

            InitializeComponent();
            DataContext = this;
        }
    }
}