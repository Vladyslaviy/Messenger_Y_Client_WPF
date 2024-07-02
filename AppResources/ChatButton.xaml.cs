using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger_Y_Client_WPF
{
    public class ChatButton : ContentControl
    {
        static ChatButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatButton), new FrameworkPropertyMetadata(typeof(ChatButton)));
        }
        public ChatButton()
        {
            Background = Brushes.White;
        }

        public override void OnApplyTemplate()
        {
            BorderThickness = new Thickness(1);
            BorderBrush = Brushes.Black;
            Margin = new Thickness(5, 5, 0, 0);
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Width = 250;
            Height = 70;
            Opacity = 0.8;
        }
        public CornerRadius cornerRadius
        {
            get { return (CornerRadius)GetValue(cornerRadiusProperty); }
            set { SetValue(cornerRadiusProperty, value); }
        }

        public string ChatName
        {
            get { return (string)GetValue(ChatNameProperty); }
            set { SetValue(ChatNameProperty, value); }
        }
        public string LastMessage
        {
            get { return (string)GetValue(LastMessageProperty); }
            set { SetValue(LastMessageProperty, value); }
        }
        public double ChatNameFontSize
        {
            get { return (double)GetValue(ChatNameFontSizeProperty); }
            set { SetValue(ChatNameFontSizeProperty, value); }
        }
        public double LastMessageFontSize
        {
            get { return (double)GetValue(LastMessageFontSizeProperty); }
            set { SetValue(LastMessageFontSizeProperty, value); }
        }
        public HorizontalAlignment ContentHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(ContentVerticalAlignmentProperty); }
            set { SetValue(ContentVerticalAlignmentProperty, value); }
        }
        public VerticalAlignment ContentVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(ContentVerticalAlignmentProperty); }
            set { SetValue(ContentVerticalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty cornerRadiusProperty =
            DependencyProperty.Register("cornerRadius", typeof(CornerRadius), typeof(ChatButton), new PropertyMetadata(new CornerRadius(6)));

        public static readonly DependencyProperty ChatNameProperty =
            DependencyProperty.Register("ChatName", typeof(string), typeof(ChatButton), new PropertyMetadata("ChatExample"));
        public static readonly DependencyProperty LastMessageProperty =
            DependencyProperty.Register("LastMessage", typeof(string), typeof(ChatButton), new PropertyMetadata("No messages yet..."));
        public static readonly DependencyProperty ChatNameFontSizeProperty =
            DependencyProperty.Register("ChatNameFontSize", typeof(double), typeof(ChatButton), new PropertyMetadata(20.0));
        public static readonly DependencyProperty LastMessageFontSizeProperty =
            DependencyProperty.Register("LastMessageFontSize", typeof(double), typeof(ChatButton), new PropertyMetadata(18.0));
        public static readonly DependencyProperty ContentHorizontalAlignmentProperty =
            DependencyProperty.Register("ContentHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChatButton), new PropertyMetadata(HorizontalAlignment.Left));
        public static readonly DependencyProperty ContentVerticalAlignmentProperty =
            DependencyProperty.Register("ContentVerticalAlignment", typeof(VerticalAlignment), typeof(ChatButton), new PropertyMetadata(VerticalAlignment.Top));

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ChatButton));

        public void SetLastMessage(MessageModel LastMessage_)
        {
            if (LastMessage_.Content.Length > 40)
            {
                LastMessage_.Content = LastMessage_.Content.Substring(0, 40);
            }
            string lastMessage = $"{LastMessage_.SenderName}: {LastMessage_.Content}";
            this.LastMessage = lastMessage;
        }

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