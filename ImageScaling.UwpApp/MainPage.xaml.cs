using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ImageScaling.Algorithms;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageScaling.UwpApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadImage();
        }

        private async void LoadImage()
        {
            // File picker
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            var imageFile = await openPicker.PickSingleFileAsync();

            if (imageFile == null) return;

            // Get stream
            var originalBytesImage = await GetImageBytesFromFile(imageFile);

            // Set original image
            Original.ImageSource = await BytesToImage(originalBytesImage);

            var scale = float.Parse(ScaleTextBox.Text);
            var width = (int)Math.Floor(originalBytesImage.Width * scale);
            var height = (int)Math.Floor(originalBytesImage.Height * scale);

            // Scaler 1
            Scaler scaler1 = new NearestNeighborScaler();
            
            var newBytes1 = scaler1.ScaleImage(scale, originalBytesImage.Bytes, originalBytesImage.Width, originalBytesImage.Height);

            var newBytesImage1 = new BytesImage(newBytes1, width, height);

            ScaledImageBrush1.ImageSource = await BytesToImage(newBytesImage1);

            // Scaler 2
            Scaler scaler2 = new LinearInterpolationScaler();

            var newBytes2 = scaler2.ScaleImage(scale, originalBytesImage.Bytes, originalBytesImage.Width, originalBytesImage.Height);

            var newBytesImage2 = new BytesImage(newBytes2, width, height);

            ScaledImageBrush2.ImageSource = await BytesToImage(newBytesImage2);
        }

        private async Task<WriteableBitmap> BytesToImage(BytesImage image)
        {
            var scaledImage = new WriteableBitmap(image.Width, image.Height);
            using (var stream = scaledImage.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(image.Bytes, 0, image.Bytes.Length);
                return scaledImage;
            }
        }

        private async Task<BytesImage> GetImageBytesFromFile(StorageFile file)
        {
            var fileStream = await file.OpenAsync(FileAccessMode.Read);

            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(fileStream);

            var decoder = await BitmapDecoder.CreateAsync(fileStream);
            var transform = new BitmapTransform()
            {
                ScaledWidth = Convert.ToUInt32(bitmapImage.PixelWidth),
                ScaledHeight = Convert.ToUInt32(bitmapImage.PixelHeight)
            };

            var pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage
            );

            return new BytesImage(pixelData.DetachPixelData(), (int) decoder.PixelWidth, (int) decoder.PixelHeight);
        }

        private class BytesImage
        {
            public BytesImage(byte[] bytes, int width, int height)
            {
                Bytes = bytes;
                Width = width;
                Height = height;
            }

            public byte[] Bytes { get; }
            public int Width { get; }
            public int Height { get; }
        }

        private void SelectImageButton_OnClick(object sender, RoutedEventArgs e) => LoadImage();
    }
}
