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
using Newtonsoft.Json.Linq;

namespace Messenger_Y_Client_WPF
{
    public class ChatMemberItem : ContentControl
    {
        public ChatMemberItem()
        {
            Background = Brushes.White;
            IsSelect = false;
        }
        static ChatMemberItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatMemberItem), new FrameworkPropertyMetadata(typeof(ChatMemberItem)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            (GetTemplateChild("PART_border") as Border).MouseEnter += Button_MouseEnter;
            (GetTemplateChild("PART_border") as Border).MouseLeave += Button_MouseLeave;
        }

        public bool IsSelect
        {
            get { return (bool)GetValue(IsSelectProperty); }
            set { SetIsSelected(value); }
        }

        public CornerRadius cornerRadius
        {
            get { return (CornerRadius)GetValue(cornerRadiusProperty); }
            set { SetValue(cornerRadiusProperty, value); }
        }

        public Brush HoverOverColor
        {
            get { return (Brush)GetValue(HoverOverColorProperty); }
            set { SetValue(HoverOverColorProperty, value); }
        }
        public Brush MainBackground
        {
            get { return (Brush)GetValue(MainBackgroundProperty); }
            set { SetValue(MainBackgroundProperty, value); }
        }



        public bool isYou
        {
            get { return (bool)GetIsYou(); }
            set { SetIsYou(value); }
        }
        public Visibility isYouVisibility
        {
            get { return (Visibility)GetValue(isYouVisibilityProperty); }
            set { SetValue(isYouVisibilityProperty, value); }
        }
        public string MemberName
        {
            get { return (string)GetValue(MemberNameProperty); }
            set { SetValue(MemberNameProperty, value); }
        }
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public string Rank
        {
            get { return (string)GetValue(RankProperty); }
            set { SetValue(RankProperty, value); }
        }

        public static readonly DependencyProperty IsSelectProperty =
            DependencyProperty.Register("IsSelect", typeof(bool), typeof(ChatMemberItem), new PropertyMetadata(false));

        public static readonly DependencyProperty cornerRadiusProperty =
            DependencyProperty.Register("cornerRadius", typeof(CornerRadius), typeof(ChatMemberItem), new PropertyMetadata(new CornerRadius(5)));

        public static readonly DependencyProperty HoverOverColorProperty =
            DependencyProperty.Register("HoverOverColor", typeof(Brush), typeof(ChatMemberItem), new PropertyMetadata(Brushes.LightGray));
        public static readonly DependencyProperty MainBackgroundProperty =
            DependencyProperty.Register("MainBackground", typeof(Brush), typeof(ChatMemberItem), new PropertyMetadata(Brushes.White));


        public static readonly DependencyProperty isYouVisibilityProperty =
            DependencyProperty.Register("isYouVisibility", typeof(Visibility), typeof(ChatMemberItem), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty MemberNameProperty =
            DependencyProperty.Register("MemberName", typeof(string), typeof(ChatMemberItem), new PropertyMetadata("Name"));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(ChatMemberItem), new PropertyMetadata("Status"));

        public static readonly DependencyProperty RankProperty =
            DependencyProperty.Register("Rank", typeof(string), typeof(ChatMemberItem), new PropertyMetadata("Rank"));

        public Brush IsYouColumnForeground
        {
            get { return (Brush)GetValue(IsYouColumnForegroundProperty); }
            set { SetValue(IsYouColumnForegroundProperty, value); }
        }
        public Brush RankColumnForeground
        {
            get { return (Brush)GetValue(RankColumnForegroundProperty); }
            set { SetValue(RankColumnForegroundProperty, value); }
        }
        public Brush StatusColumnForeground
        {
            get { return (Brush)GetValue(StatusColumnForegroundProperty); }
            set { SetValue(StatusColumnForegroundProperty, value); }
        }

        public static readonly DependencyProperty IsYouColumnForegroundProperty =
            DependencyProperty.Register("IsYouColumnForeground", typeof(Brush), typeof(ChatMemberItem), new PropertyMetadata(Brushes.DodgerBlue));

        public static readonly DependencyProperty StatusColumnForegroundProperty =
            DependencyProperty.Register("StatusColumnForeground", typeof(Brush), typeof(ChatMemberItem), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty RankColumnForegroundProperty =
            DependencyProperty.Register("RankColumnForeground", typeof(Brush), typeof(ChatMemberItem), new PropertyMetadata(Brushes.Black));

        public Brush CurrentBackground
        {
            get { return (Brush)GetValue(CurrentBackgroundProperty); }
            set { SetValue(CurrentBackgroundProperty, value); }
        }
        public static readonly DependencyProperty CurrentBackgroundProperty =
            DependencyProperty.Register("CurrentBackground", typeof(Brush), typeof(ChatMemberItem), new PropertyMetadata(Brushes.White));
        public void SetIsYou(bool value) 
        { 
            if (value)
            {
                isYouVisibility = Visibility.Visible;
            }
            else
            {
                isYouVisibility = Visibility.Collapsed;
            }
        }
        public bool GetIsYou()
        {
            if (isYouVisibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Unselect()
        {
            IsSelect = false;
        }
        public void SetIsSelected(bool value)
        {
            SetValue(IsSelectProperty, value);
            if (value)
            {
                SetBackground(HoverOverColor);
            }
            else
            {
                SetBackground(MainBackground);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            CurrentBackground = HoverOverColor;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsSelect)
            {
                CurrentBackground = MainBackground;
            }
        }
        public void SetBackground(Brush brush)
        {
            CurrentBackground = brush;
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