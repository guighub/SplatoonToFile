using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace FileToSplatoon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            int imageWidth = 320;
            int imageHeight = 120;
            var openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.FileName = "PNG Image";                    // Default file name
            openFile.DefaultExt = ".png";                       // Default file extension
            openFile.Filter = "PNG Image File (.png)|*.png";    // Filter files by extension
            bool? openedFile = openFile.ShowDialog();

            if (openedFile == true)
            {
                FileStream fileStream = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(fileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                BitmapSource newImage = new TransformedBitmap(bitmapSource, new ScaleTransform(imageWidth / bitmapSource.PixelWidth, imageHeight / bitmapSource.PixelHeight));
                
                int stride = newImage.PixelWidth * (newImage.Format.BitsPerPixel / 8);
                byte[] bitArray = new byte[imageWidth * imageHeight]; // Bits of file stored in bytes (1 = 11111111)
                byte[] fileBytes = new byte[bitArray.Length / 8]; // The decoded file's bytes
                newImage.CopyPixels(bitArray, stride, 0);

                for (int bit = 0; bit < bitArray.Length; bit++)
                {
                    if (InvertCheckBox.IsChecked ?? false)
                    {
                        if (bitArray[bit] > 0x7F)
                        {
                            fileBytes[bit / 8] |= (byte)(1 << (bit % 8));
                        }
                    }
                    else
                    {
                        if (bitArray[bit] < 0x7F)
                        {
                            fileBytes[bit / 8] |= (byte)(1 << (bit % 8));
                        }
                    }
                }

                FileStream outFile = new FileStream(openFile.FileName + ".dat", FileMode.Create);
                outFile.Write(fileBytes);
                LogText.Content = "Complete!";
                LogText.Foreground = Brushes.Green;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
