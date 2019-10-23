using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace QrScaner
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BarcodeWriter writer = CreateQr();

            //Bitmap map = writer.Write(UrlTextBox.Text);
            //QrImage.Source = Change2Image(map);
            //map.Dispose();
        }

        private void Scaner_Click(object sender, RoutedEventArgs e)
        {
            QrImage.Source = new BitmapImage(new Uri("Scanner.png", UriKind.Relative)); 
            foreach (Screen screen in Screen.AllScreens)
            {
                using (Bitmap fullImage = new Bitmap(screen.Bounds.Width,
                                                screen.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(fullImage))
                    {
                        g.CopyFromScreen(screen.Bounds.X,
                                         screen.Bounds.Y,
                                         0, 0,
                                         fullImage.Size,
                                         CopyPixelOperation.SourceCopy);
                    }
                    int maxTry = 10;
                    for (int i = 0; i < maxTry; i++)
                    {
                        int marginLeft = (int)((double)fullImage.Width * i / 2.5 / maxTry);
                        int marginTop = (int)((double)fullImage.Height * i / 2.5 / maxTry);
                        Rectangle cropRect = new Rectangle(marginLeft, marginTop, fullImage.Width - marginLeft * 2, fullImage.Height - marginTop * 2);
                        Bitmap target = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);

                        double imageScale = (double)screen.Bounds.Width / (double)cropRect.Width;
                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
                                            cropRect,
                                            GraphicsUnit.Pixel);
                        }
                        var source = new BitmapLuminanceSource(target);
                        var bitmap = new BinaryBitmap(new HybridBinarizer(source));
                        QRCodeReader reader = new QRCodeReader();
                        var result = reader.decode(bitmap);
                        if (result != null)
                        {
                            //QrImage.Source = Change2Image(target); 
                            UrlTextBox.Text = result.ToString();
                        }
                    }
                }
            }
        }


        private void Create_Click(object sender, RoutedEventArgs e)
        {
            BarcodeWriter writer = CreateQr();

            Bitmap map = writer.Write(UrlTextBox.Text);
            QrImage.Source = Change2Image(map);
            map.Dispose();
        }

        /// <summary>
        /// QrCreater
        /// </summary>
        /// <returns></returns>
        private static BarcodeWriter CreateQr()
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            options.DisableECI = true;
            //设置内容编码
            options.CharacterSet = "UTF-8";
            //设置二维码的宽度和高度
            options.Width = 399;
            options.Height = 399;
            //设置二维码的边距,单位不是固定像素
            options.Margin = 1;
            writer.Options = options;
            return writer;
        }

        /// <summary>
        /// Change bitmap to image source
        /// </summary>
        /// <param name="target">bitmap object</param>
        /// <returns>image source</returns>
        private static BitmapImage Change2Image(Bitmap target)
        {
            Bitmap b = new Bitmap(target);
            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以
            ms.Close();
            //Convert it to BitmapImage
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(bytes);
            image.EndInit();
            return image;
        }

    }
}
