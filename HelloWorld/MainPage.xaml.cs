using System.Threading.Tasks;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld {

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage: Page {

        public MainPage() {
            this.InitializeComponent();
            var Container = new Canvas();
            Container.Margin = new Thickness(50);
            Background.Children.Add(Container);
            CreateSmiley(Container);
            AddBitmap(Container);
        }

        private async void AddBitmap(Canvas canvas) {
            Background.Children.Add(await RenderBitmap(canvas));
        }

        private void CreateSmiley(Canvas Container) {
            var Face = CreateFace();
            var LeftEye = CreateEye();
            var RightEye = CreateEye();
            var Mouth = CreateMouth();
            Canvas.SetLeft(Face, 10);
            Canvas.SetLeft(LeftEye, 150);
            Canvas.SetLeft(RightEye, 60);
            Canvas.SetLeft(Mouth, 45);
            Canvas.SetTop(Face, 10);
            Canvas.SetTop(LeftEye, 65);
            Canvas.SetTop(RightEye, 65);
            Canvas.SetTop(Mouth, 130);
            Container.Children.Add(Face);
            Container.Children.Add(LeftEye);
            Container.Children.Add(RightEye);
            Container.Children.Add(Mouth);
        }



        private Ellipse CreateEye() {
            return new Ellipse() {
                Fill = new SolidColorBrush(Colors.Black),
                Height = 30,
                Width = 20
            };
        }

        private Ellipse CreateFace() {
            return new Ellipse() {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.Yellow),
                Height = 200,
                Width = 200
            };
        }

        private Path CreateMouth() {
            var MouthBezierSegment = new BezierSegment() {
                Point1 = new Point(50, 70),
                Point2 = new Point(100, 50),
                Point3 = new Point(140, 0)
            };
            var MouthPathFigure = new PathFigure() {
                StartPoint = new Point(0, 0),
                Segments = { MouthBezierSegment }
            };
            var MouthPathGeometry = new PathGeometry() {
                Figures = { MouthPathFigure }
            };
            return new Path() {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 5,
                Data = MouthPathGeometry
            };
        }

        private async Task<Image> RenderBitmap(Canvas canvas) {
            var image = new Image();
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(canvas);
            image.Source = renderTargetBitmap;
            return image;
        }

        private async Task CreateSaveBitmapAsync(Canvas canvas) {
            if (canvas != null) {
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(canvas);

                var picker = new FileSavePicker();
                picker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null) {
                    var pixels = await renderTargetBitmap.GetPixelsAsync();

                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                        var encoder = await
                            BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                        byte[] bytes = pixels.ToArray();
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                             BitmapAlphaMode.Ignore,
                                             (uint)canvas.Width, (uint)canvas.Height,
                                             96, 96, bytes);

                        await encoder.FlushAsync();
                    }
                }
            }
        }
    }
}