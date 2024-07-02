using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Messenger_Y_Client_WPF
{
    public class UIMessages
    {
        public int chatID;

        public List<UIMessageBlock> _UIMessageBlocks;
        public StackPanel MessagesUIElement;

        public bool isBlockCompleted;

        public UIMessages()
        {
            isBlockCompleted = true;
            chatID = -1;
            _UIMessageBlocks = new List<UIMessageBlock>();
            MessagesUIElement = new StackPanel();
            MessagesUIElement.Height = double.NaN;

            MessagesUIElement.Name = "MessagesStackPanel" + chatID;
        }
        public UIMessages(int chatID_)
        {
            isBlockCompleted = true;
            chatID = chatID_;
            _UIMessageBlocks = new List<UIMessageBlock>();
            MessagesUIElement = new StackPanel();
            MessagesUIElement.Height = double.NaN;


            MessagesUIElement.Name = "MessagesStackPanel" + chatID;
        }
        public UIMessages(ChatModel chat, Grid MessagesScrollViewGrid_)
        {
            isBlockCompleted = true;
            chatID = chat.id;
            _UIMessageBlocks = new List<UIMessageBlock>();
            MessagesUIElement = new StackPanel();
            MessagesUIElement.Height = double.NaN;

            MessagesUIElement.Name = "MessagesStackPanel" + chatID;

            if (chat.messages.Count > 0)
            {
                SetMessagesAsync(chat, MessagesScrollViewGrid_);//Visual improvements and fixes.
            }
        }
        async void SetMessagesAsync(ChatModel chat, Grid MessagesScrollViewGrid_)
        {
            isBlockCompleted = false;
            foreach (MessageModel message in chat.messages)
            {
                ChatMember senderInfo = chat.members.Find(x => x.UserId == message.SenderID);
                if (senderInfo == null)
                {
                    senderInfo = new ChatMember { IsOnline = false, Rank = MemberRank.Kicked, UserId = -1, Username = "Kicked user" };
                }
                UIMessageBlock UIMessageBlock_temp = new UIMessageBlock(message, senderInfo, MessagesScrollViewGrid_);
                _UIMessageBlocks.Add(UIMessageBlock_temp);
                MessagesUIElement.Children.Add(UIMessageBlock_temp.mainMessageBlock);
                //await Task.Delay(50);
            }
            isBlockCompleted = true;
        }
        public async Task AddMessageAsync(MessageModel message, ChatMember chatMember, Grid MessagesScrollViewGrid_)
        {
            if (message == null)
            {
                return;
            }
            UIMessageBlock UIMessageBlock_temp = new UIMessageBlock(message, chatMember, MessagesScrollViewGrid_);
            _UIMessageBlocks.Add(UIMessageBlock_temp);
            MessagesUIElement.Children.Add(UIMessageBlock_temp.mainMessageBlock);
            return;
        }
    }



    public class UIMessagesBlockPlainText
    {
        public int chatID;
        public string chatName;
        public string messagesBlock;
        public bool isBlockCompleted = false;
        public UIMessagesBlockPlainText(int chatID_, string chatName_, List<MessageModel> messages)
        {
            messagesBlock = "";
            chatID = chatID_;
            chatName = chatName_;
            SetMessageBlockAsync(messages);
        }
        public UIMessagesBlockPlainText(int chatID_, string chatName_)
        {
            messagesBlock = "";
            chatID = chatID_;
            chatName = chatName_;
            isBlockCompleted = true;
        }
        public async Task SetMessageBlockAsync(List<MessageModel> messages)
        {
            isBlockCompleted = false;
            await Task.Delay(200);
            foreach (MessageModel message in messages)
            {
                string message_str = $"[{message.SentTime}] {message.SenderName}: {message.Content}";
                await AddMessageAsync(message_str);
            }
            isBlockCompleted = true;
        }
        public void AddMessage(string message)
        {
            messagesBlock += $"\n{message}\n";
        }
        public async Task AddMessageAsync(string message)
        {
            messagesBlock += $"\n{message}\n";
        }
    }


    public class UIMessageBlock
    {
        public int chatID;
        public int senderID;
        public string senderName;
        public DateTime dateTimeSent;

        public Grid mainMessageBlock;


        public Border messageBlockBorder;

        public Grid messageInfoGrid;
        public TextBlock senderNameTextBlock;
        public TextBlock sentTimeTextBlock;

        public TextBlock messageTextBlock;

        public int fontSize = 18;
        int minWidth = 50;
        int minHeight = 40;
        int maxWidth = 900;
        public UIMessageBlock()
        {
            chatID = -1;
            senderID = -1;
            senderName = string.Empty;
            dateTimeSent = DateTime.MinValue;

            mainMessageBlock = new Grid();
            messageBlockBorder = new Border();
            messageInfoGrid = new Grid();
            senderNameTextBlock = new TextBlock();
            sentTimeTextBlock = new TextBlock();
            messageTextBlock = new TextBlock();
        }
        public UIMessageBlock(MessageModel message, ChatMember senderInfo, Grid MessagesScrollViewGrid_)
        {
            chatID = message.ChatID;
            senderID = message.SenderID;
            senderName = message.SenderName;
            dateTimeSent = message.SentTime;

            mainMessageBlock = new Grid();
            messageBlockBorder = new Border();
            messageInfoGrid = new Grid();
            senderNameTextBlock = new TextBlock();
            sentTimeTextBlock = new TextBlock();
            messageTextBlock = new TextBlock();


            mainMessageBlock.Margin = new Thickness(15, 0, 0, 15);
            mainMessageBlock.VerticalAlignment = VerticalAlignment.Bottom;
            mainMessageBlock.HorizontalAlignment = HorizontalAlignment.Left;
            mainMessageBlock.Height = double.NaN;
            mainMessageBlock.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainMessageBlock.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //Grid.SetRow(mainMessageBlock, messageIndex);

            messageBlockBorder.BorderBrush = Brushes.White;
            messageBlockBorder.BorderThickness = new Thickness(1);
            messageBlockBorder.CornerRadius = new CornerRadius(6);
            messageBlockBorder.Background = Brushes.Black;
            messageBlockBorder.Opacity = 0.9;
            messageBlockBorder.VerticalAlignment = VerticalAlignment.Bottom;
            messageBlockBorder.HorizontalAlignment = HorizontalAlignment.Left;
            messageBlockBorder.Height = double.NaN;
            messageBlockBorder.Name = "ID" + message.id;
            Grid.SetRow(messageBlockBorder, 1);

            messageInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            messageInfoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            senderNameTextBlock.Text = senderInfo.Username;
            senderNameTextBlock.FontSize = fontSize;
            senderNameTextBlock.Foreground = GetColorForRank(senderInfo.Rank); /////////////////ADD COLORS
            senderNameTextBlock.Margin = new Thickness(5, 5, 5, 5);
            senderNameTextBlock.VerticalAlignment = VerticalAlignment.Top;
            senderNameTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(senderNameTextBlock, 0);

            string sentTimeString = message.SentTime.ToString("yyyy dd MMMM hh:mm:ss tt");
            sentTimeTextBlock.Text = sentTimeString;
            sentTimeTextBlock.FontSize = fontSize;
            sentTimeTextBlock.Foreground = Brushes.LightGray;
            sentTimeTextBlock.Margin = new Thickness(5, 5, 5, 5);
            sentTimeTextBlock.VerticalAlignment = VerticalAlignment.Top;
            sentTimeTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(sentTimeTextBlock, 0);

            messageTextBlock.Margin = new Thickness(5, 5, 5, 5);
            messageTextBlock.TextWrapping = TextWrapping.Wrap;
            messageTextBlock.Text = message.Content;
            messageTextBlock.Foreground = Brushes.White;
            messageTextBlock.FontSize = fontSize;
            Grid.SetRow(messageTextBlock, 1);

            messageInfoGrid.Children.Add(senderNameTextBlock);
            messageInfoGrid.Children.Add(sentTimeTextBlock);
            messageInfoGrid.Children.Add(messageTextBlock);

            messageBlockBorder.Child = messageInfoGrid;
            minWidth = Convert.ToInt32((sentTimeString.Length + senderInfo.Username.Length) * (fontSize / 1.5));
            maxWidth = Convert.ToInt32(Math.Round(MessagesScrollViewGrid_.ActualWidth * 0.6));
            int messageLength = message.Content.Length * (fontSize + 1) + 20;
            if (messageLength > minWidth)
            {
                messageBlockBorder.Width = maxWidth;
            }
            else
            {
                //Debug.WriteLine(minWidth);
                messageBlockBorder.Width = minWidth;
            }
            mainMessageBlock.Children.Add(messageBlockBorder);
        }
        SolidColorBrush GetColorForRank(MemberRank rank) ///////ADD TO STATIC METHOD IN STATIC CLASS
        {
            switch (rank)
            {
                case MemberRank.Creator:
                    return Brushes.Purple;
                case MemberRank.Administrator:
                    return Brushes.Red;
                case MemberRank.Member:
                    return Brushes.LightBlue;
                case MemberRank.Spectator:
                    return Brushes.Chocolate;
                case MemberRank.Kicked:
                    return Brushes.Red;
                default:
                    break;
            }
            return Brushes.Black;
        }
    }
}
