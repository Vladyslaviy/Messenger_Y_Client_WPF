using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Messenger_Y_Client_WPF
{
    public class MenuWindow : ContentControl
    {
        ImageBrush imageBackground;
        public static ImageSourceConverter tempISVS = new ImageSourceConverter();
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Grid temp = ((GetTemplateChild("MainBorder") as Border).Child as Grid);
            imageBackground = (((temp.Children[0] as Border).Background as ImageBrush));

            //ImageSource imageSource1 = LoadImageFromPath("pack://application:,,,/Images/Background4.png");
            imageBackground.ImageSource = GetDefaultImageSource();

            if (imageSource_safe != null)
            {
                imageSource = imageSource_safe;
            }
        }
        public MenuWindow()
        {
            //imageSource = GetDefaultImageSource();

        }
        static MenuWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuWindow), new FrameworkPropertyMetadata(typeof(MenuWindow)));
        }

        public CornerRadius cornerRadius
        {
            get { return (CornerRadius)GetValue(cornerRadiusProperty); }
            set { SetValue(cornerRadiusProperty, value); }
        }

        public ImageSource imageSource
        {
            get => (ImageSource)GetValue(imageSourceProperty);
            set => SetValue_(value);
        }
        public ImageSource imageSource_safe;
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty cornerRadiusProperty =
            DependencyProperty.Register("cornerRadius", typeof(CornerRadius), typeof(MenuWindow), new PropertyMetadata(new CornerRadius(5)));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(MenuWindow), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty imageSourceProperty =
            DependencyProperty.Register("imageSource", typeof(ImageSource), typeof(MenuWindow), new PropertyMetadata(default(ImageSource)));


        private static ImageSource GetDefaultImageSource()
        {
            return new BitmapImage(new Uri("pack://application:,,,/Messenger_Y_Client_WPF;component/Images/Background4.png", UriKind.Absolute));
        }
        private static ImageSource GetImageSource(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }

        public void SetValue_(ImageSource iS)
        {
            try
            {
                DependencyObject dependencyObject = GetTemplateChild("MainBorder");
                Grid temp = ((dependencyObject as Border).Child as Grid);
                imageBackground = (((temp.Children[0] as Border).Background as ImageBrush));

                //ImageSource imageSource1 = LoadImageFromPath("pack://application:,,,/Images/Background4.png");
                imageBackground.ImageSource = iS;
            }
            catch (Exception e)
            {

            }


        }
        public void Test()
        {
            Debug.WriteLine("TEST");
        }
    }
}