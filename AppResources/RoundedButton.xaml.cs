using CustomControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Diagnostics;

namespace Messenger_Y_Client_WPF
{
    public class RoundedButton : ContentControl
    {
        public RoundedButton()
        {
            Background = Brushes.White;
        }
        static RoundedButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundedButton), new FrameworkPropertyMetadata(typeof(RoundedButton)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            (GetTemplateChild("PART_border") as Border).MouseEnter += Button_MouseEnter;
            (GetTemplateChild("PART_border") as Border).MouseLeave += Button_MouseLeave;
        }
        public Brush HighlightBackground
        {
            get { return (Brush)GetValue(HighlightBackgroundProperty); }
            set { SetValue(HighlightBackgroundProperty, value); }
        }

        public CornerRadius cornerRadius
        {
            get { return (CornerRadius)GetValue(cornerRadiusProperty); }
            set { SetValue(cornerRadiusProperty, value); }
        }

        public Brush HoverOverColor
        {
            get { return (Brush)GetValue(HoverOverColorProperty); }
            set { SetHoverOverColor(value); }
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
        

        public static readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(RoundedButton), new PropertyMetadata(Brushes.DarkGray));

        public static readonly DependencyProperty cornerRadiusProperty =
            DependencyProperty.Register("cornerRadius", typeof(CornerRadius), typeof(RoundedButton), new PropertyMetadata(new CornerRadius(5)));

        public static readonly DependencyProperty HoverOverColorProperty =
            DependencyProperty.Register("HoverOverColor", typeof(Brush), typeof(RoundedButton), new PropertyMetadata(Brushes.LightGray));

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundedButton));

        public static readonly DependencyProperty ContentHorizontalAlignmentProperty =
            DependencyProperty.Register("ContentHorizontalAlignment", typeof(HorizontalAlignment), typeof(RoundedButton), new PropertyMetadata(HorizontalAlignment.Center));
        public static readonly DependencyProperty ContentVerticalAlignmentProperty =
            DependencyProperty.Register("ContentVerticalAlignment", typeof(VerticalAlignment), typeof(RoundedButton), new PropertyMetadata(VerticalAlignment.Center));


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


        public void SetHoverOverColor(Brush value)
        {
            SetValue(HoverOverColorProperty, value);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = HoverOverColor;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = Background;
        }
    }
    //Border border;
    //Grid grid;
    //Ellipse ellipse;
    //Storyboard animation;

    //public override void OnApplyTemplate()
    //{
    //    base.OnApplyTemplate();
    //    border = GetTemplateChild("PART_border") as Border;
    //    grid = border.Child as Grid;
    //    ellipse = grid.FindName("PART_ellipse") as Ellipse;
    //    animation = grid.FindResource("PART_animation") as Storyboard;
    //    if (animation != null || grid == null || ellipse == null || animation == null)
    //    {
    //        //throw new Exception("NULL");
    //    }
    //    this.AddHandler(MouseDownEvent, new RoutedEventHandler((sender, e) =>
    //    {
    //        var targetWidth = Math.Max(ActualWidth, ActualHeight) * 2;
    //        var mousePosition = (e as MouseButtonEventArgs).GetPosition(this);
    //        var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
    //        //set initial margin to mouse position
    //        ellipse.Margin = startMargin;
    //        //set the to value of the animation that animates the width to the target width
    //        (animation.Children[0] as DoubleAnimation).To = targetWidth;
    //        //set the to and from values of the animation that animates the distance relative to the container (grid)
    //        ThicknessAnimation test = (animation.Children[1] as ThicknessAnimation);
    //        (animation.Children[1] as ThicknessAnimation).From = startMargin;
    //        (animation.Children[1] as ThicknessAnimation).To = new Thickness(mousePosition.X - targetWidth / 2, mousePosition.Y - targetWidth / 2, 0, 0);
    //        ellipse.BeginStoryboard(animation);
    //    }), true);
    //}

}