using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger_Y_Client_WPF
{
    public class CloseButton : ContentControl
    {
        static CloseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CloseButton), new FrameworkPropertyMetadata(typeof(CloseButton)));
        }
        public CloseButton()
        {
            HorizontalAlignment = HorizontalAlignment.Right;
            VerticalAlignment = VerticalAlignment.Top;
            Width = 35;
            Height = 35;
            Content = "X";
            FontSize = 14;
            Background = Brushes.White;
        }
        public CornerRadius cornerRadius
        {
            get { return (CornerRadius)GetValue(cornerRadiusProperty); }
            set { SetValue(cornerRadiusProperty, value); }
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty cornerRadiusProperty =
            DependencyProperty.Register("cornerRadius", typeof(CornerRadius), typeof(CloseButton), new PropertyMetadata(new CornerRadius(0, 5, 0, 5)));
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CloseButton), new PropertyMetadata("X"));

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CloseButton));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        // Raise the custom Click event
        protected void OnClick()
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        // Handle the MouseUp event to raise the Click event
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            OnClick();
        }
    }
}