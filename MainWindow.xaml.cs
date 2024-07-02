using CustomControlLibrary;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Messenger_Y_Client_WPF
{

    public struct ConnectionProperties
    {
        public string server_ip { get; set; }
        public int receivingPort { get; set; }
        public int sendingPort { get; set; }
        public string pingMessage { get; set; }
    }
    public partial class MainWindow : Window
    {

        private string server_ip;
        private int receivingPort;
        private int sendingPort;

        TcpClient clientSen;
        TcpClient clientRec;
        NetworkStream streamSen;
        NetworkStream streamRec;
        byte[] lastMessage;
        bool status_on;

        byte[] pingMessage;

        bool isLoggedin;
        UserModel user;


        ChatModel currentChat = new ChatModel();
        List<ChatButton> chatsButtons = new List<ChatButton>();
        List<UIMessagesBlockPlainText> UIChatsMessageBoxes_text;
        List<UIMessages> UIChatsMessages;


        List<ChatMember> newChatMembersList;


        List<CheckBox> newChatMembers = new List<CheckBox>();
        List<CheckBox> chatNewMembers = new List<CheckBox>();


        public MainViewModel mainViewModel { get; set; }


        public MainWindow()//
        {
            string resourceName = "Messenger_Y_Client_WPF.Connections.json";

            string jsonContent;
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                jsonContent = reader.ReadToEnd();
            }
            ConnectionProperties connectionProperties = JsonConvert.DeserializeObject<ConnectionProperties>(jsonContent);

            server_ip = connectionProperties.server_ip;
            receivingPort = connectionProperties.receivingPort;
            sendingPort = connectionProperties.sendingPort;

            mainViewModel = new MainViewModel();
            DataContext = mainViewModel;

            //UIChatsMessageBoxes_text = new List<UIMessagesBlockPlainText>();
            UIChatsMessages = new List<UIMessages>();
            pingMessage = Encoding.Unicode.GetBytes(connectionProperties.pingMessage);

            isLoggedin = false;


            InitializeComponent();

        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetBackgrounds();
            Start();
            AddMemberListLB.Items.Clear();
            MainGrid.Visibility = Visibility.Collapsed;
            RegisterGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
            var storyboard = (Storyboard)FindResource("ShowErrorMessage");
            storyboard.Completed += ShowErrorMessage_Completed;
            SendingMessageTB.TextChanged += MessageTypingChanged;
            //AddEmptySpaceClickEventHandlers(ChatProps);

        }
        private void AddEmptySpaceClickEventHandlers(Panel panel)
        {
            foreach (UIElement element in panel.Children)
            {
                Debug.WriteLine((element as FrameworkElement).Name);
                if ((element as FrameworkElement).Name != AddMemberBtn.Name)
                {
                    if (element is Panel childPanel)
                    {
                        if ((element as Panel).Name != "AvoidedParent")
                        {
                            element.MouseUp += EmptySpace_MouseUp;
                        }
                        AddEmptySpaceClickEventHandlers(childPanel);
                        continue;
                    }
                    element.MouseUp += EmptySpace_MouseUp;
                }

            }
        }
        private void SetBackgrounds()
        {
            ChatPropertiesGrid.imageSource_safe = new BitmapImage(new Uri("pack://application:,,,/Images/Black_white_background_1.png", UriKind.Absolute));
        }
        private void SendingMessageTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (SendingMessageTB.Foreground == Brushes.LightGray)
            {
                if (SendingMessageTB.SelectedText != string.Empty)
                {
                    SendingMessageTB.SelectionLength = 0;
                }
                if (SendingMessageTB.CaretIndex != 0)
                {
                    SendingMessageTB.CaretIndex = 0;
                }
            }
        }
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SendingMessageTB.Foreground != Brushes.LightGray)
            {
                SendMessageStart(SendingMessageTB.Text);
            }
        }

        int maxHeight = 46 * 6;

        private void SendingMessageTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input_message = SendingMessageTB.Text;
            if (SendingMessageTB.Foreground == Brushes.LightGray && SendingMessageTB.Text != "Type a message...")
            {
                SendingMessageTB.Foreground = Brushes.White;
                SendingMessageTB.Text = SendingMessageTB.Text.Remove(SendingMessageTB.Text.IndexOf("Type a message..."), "Type a message...".Length);
                SendingMessageTB.CaretIndex = 1;
            }
            if (SendingMessageTB.Text == string.Empty)
            {
                SendingMessageTB.Text = "Type a message...";
                SendingMessageTB.Foreground = Brushes.LightGray;
                SendingMessageTB.CaretIndex = 0;
            }
            if (SendingMessageTB.ActualWidth != 0)
            {
                if (input_message.Length * (SendingMessageTB.FontSize / 2) > SendingMessageTB.ActualWidth)
                {
                    //Debug.WriteLine("LARGE"+SendingMessageTB.ActualWidth);
                    int rowCount = 1 + Convert.ToInt32(Math.Round(input_message.Length * (SendingMessageTB.FontSize / 1.5) / SendingMessageTB.ActualWidth, MidpointRounding.ToEven));
                    double changedHeight = 46 + SendingMessageTB.FontSize / 1.5 * rowCount;
                    if (changedHeight < maxHeight)
                    {
                        SendingMessageTB.Height = changedHeight;
                    }
                    else
                    {
                        SendingMessageTB.Height = maxHeight;
                    }
                }
                else
                {
                    //Debug.WriteLine(SendingMessageTB.ActualWidth);
                    SendingMessageTB.Height = 46;
                }
            }
        }

        private void SendingMessageTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SendingMessageTB.Foreground == Brushes.LightGray)
            {
                SendingMessageTB.CaretIndex = 0;
            }
            if (e.LeftButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {

            }
        }
        private void SendingMessageTB_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (SendingMessageTB.Foreground != Brushes.LightGray)
            {
                if (e.Key == Key.Enter)
                {
                    SendMessageStart(SendingMessageTB.Text);
                }
            }
        }
        async void SendMessageStart(string message_s)
        {
            if (currentChat.id != -1)
                if (!string.IsNullOrEmpty(message_s) && !string.IsNullOrWhiteSpace(message_s))
                {
                    MessageModel message = new MessageModel
                    {
                        ChatID = currentChat.id,
                        Content = message_s,
                        SenderID = user.id,
                        SenderName = user.username,
                        SentTime = DateTime.Now,
                    };
                    await SendData(new DataChannel(message));
                    SendingMessageTB.Text = "";
                }
        }


        async Task AddMessageAsync(MessageModel message)
        {
            int chatIndex = user.chats.FindIndex(x => x.id == message.ChatID);
            user.chats[chatIndex].messages.Add(message);
            int UI_Index = UIChatsMessages.FindIndex(x => x.chatID == message.ChatID);//"ID"
            await UIChatsMessages[UI_Index].AddMessageAsync(message, user.chats[chatIndex].members.Find(m => m.UserId == message.SenderID), MessagesScrollViewGrid);
            MessagesScrollView.ScrollToEnd();
            if (currentChat.id == message.ChatID)
            {
                //await MessagesBlockWriteLine(message_str);
            }
            int UI_ChatButtonIndex = chatsButtons.FindIndex(x => x.Name == "ID" + message.ChatID);
            chatsButtons[UI_ChatButtonIndex].SetLastMessage(message);
        }

        async Task AddMessageInUIPlainText(MessageModel message)
        {
            user.chats.Find(x => x.id == message.ChatID).messages.Add(message);
            string message_str = $"[{message.SentTime}] {message.SenderName}: {message.Content}";
            UIChatsMessageBoxes_text.Find(x => x.chatID == message.ChatID).AddMessage(message_str);
            if (currentChat.id == message.ChatID)
            {
                await MessagesBlockWriteLinePlainText(message_str);
            }
        }
        async Task AddMessageToUIAsync()
        {

        }
        async Task MessagesBlockWriteLinePlainText(string message)
        {
            MessagesTB.Text += $"\n{message}\n";

            MessagesTB.ScrollToEnd();
        }

        async void Start()
        {
            newChatMembersList = new List<ChatMember>();
            await Task.Delay(200);

            status_on = true;
            //MessagesBlockWriteLine("Messanger started");
            Connector();
        }
        async void Connector()
        {
            while (true)
            {
                Byte[] data;
                clientSen = new TcpClient();
                clientRec = new TcpClient();
                while (!clientSen.Connected)
                {
                    try
                    {
                        await clientSen.ConnectAsync(server_ip, sendingPort);
                        await clientRec.ConnectAsync(server_ip, receivingPort);
                    }
                    catch (Exception)
                    {

                    }
                    await Task.Delay(200);
                }
                // = System.Text.Encoding.Unicode.GetBytes(message);

                streamSen = clientSen.GetStream();
                streamRec = clientRec.GetStream();
                //MessagesBlockWriteLine($"Connected to [{clientRec.Client.RemoteEndPoint as IPEndPoint}]");
                //stream.Write(data, 0, data.Length);
                MessageListener();

                while (clientSen.Connected)
                {
                    try
                    {
                        int counter = 0;
                        while (counter < 3)
                        {
                            if (streamSen.CanWrite)
                            {
                                await streamSen.WriteAsync(pingMessage);
                                break;
                            }
                            else
                            {
                                await Task.Delay(5000);
                                counter++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("EXCEPTION:" + e.Message);
                        clientSen.Close();
                        clientRec.Close();
                        clientSen = null;
                        clientRec = null;
                        break;
                    }
                    await Task.Delay(3000);
                }
                //MessagesBlockWriteLine($"Disconnected");
                await Task.Delay(200);
            }
        }
        async void MessageSender(string messageTS)
        {
            if (clientSen != null && clientSen.Connected)
            {
                try
                {
                    string message = messageTS;
                    //string message = await Console.In.ReadLineAsync();
                    while (message != null && message != "")
                    {
                        if (streamSen.CanWrite)
                        {
                            byte[] message_encoded = Encoding.Unicode.GetBytes(message + '\n');
                            lastMessage = message_encoded;
                            await streamSen.WriteAsync(message_encoded);
                            break;
                        }
                        await Task.Delay(200);
                    }
                }
                catch (Exception)
                {
                    clientSen = null;
                }
            }
        }
        async Task SendData(DataChannel dataChannel)
        {
            if (clientSen != null && clientSen.Connected)
            {
                try
                {
                    string JSONserialized = JsonConvert.SerializeObject(dataChannel);
                    byte[] message_encoded = Encoding.Unicode.GetBytes(JSONserialized + '\n');
                    //string message = await Console.In.ReadLineAsync();
                    while (true)
                    {
                        if (streamSen.CanWrite)
                        {
                            await streamSen.WriteAsync(message_encoded);
                            lastMessage = message_encoded;
                            break;
                        }
                        await Task.Delay(1);
                    }
                }
                catch (Exception)
                {
                    clientSen = null;
                }
            }
        }

        async void MessageListener()
        {
            byte[] streamData;
            string receivedData = "";


            string unterminatedString = "";

            while (clientRec.Connected && clientRec != null)
            {
                await Task.Delay(200);
                if (user != null)
                {

                }
                try
                {
                    if (streamRec.CanRead)
                    {
                        if (user == null)
                        {
                            streamData = new byte[1024000 * 1]; //Initializing 1 Mb Buffer size
                        }
                        else
                            streamData = new byte[1024 * 12]; //12 Kb Buffer size

                        await streamRec.ReadAsync(streamData, 0, streamData.Length);
                        await streamRec.FlushAsync();

                        receivedData = Encoding.Unicode.GetString(streamData);

                        //Debug.WriteLine("AAAAAAAAAAAAAAAAreceivedData=" + receivedData + "AAAAAA");
                        List<DataChannel> dataChannels = new List<DataChannel>();
                        string[] receivedData_list = receivedData.Split('\n');
                        foreach (string receivedData_T in receivedData_list)
                        {
                            if (!string.IsNullOrEmpty(receivedData_T) && !string.IsNullOrWhiteSpace(receivedData_T))
                            {
                                try
                                {
                                    DataChannel dataChannel_ = JsonConvert.DeserializeObject<DataChannel>(receivedData_T);
                                    if (dataChannel_ != null)
                                    {
                                        dataChannels.Add(dataChannel_);
                                    }
                                }
                                catch (Exception)
                                {
                                    //Debug.WriteLine($"unterminatedString 1 entry:{unterminatedString}");
                                    if (unterminatedString == "")
                                    {
                                        unterminatedString += receivedData_T;
                                        continue;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            //Debug.WriteLine($"new try:{unterminatedString + receivedData_T}");
                                            DataChannel dataChannel_ = JsonConvert.DeserializeObject<DataChannel>(unterminatedString + receivedData_T);
                                            if (dataChannel_ != null)
                                            {
                                                dataChannels.Add(dataChannel_);
                                            }
                                            unterminatedString = "";
                                        }
                                        catch (Exception)
                                        {
                                            //Debug.WriteLine($"unterminatedString 2 entry:{unterminatedString}");
                                            unterminatedString += receivedData_T;
                                            continue;
                                        }
                                    }
                                }
                            }

                        }

                        foreach (DataChannel dataChannel in dataChannels)
                        {
                            if (!isLoggedin)
                            {
                                if (dataChannel.GetDataType() == DataType.LoginFeedback)
                                {
                                    UserModel LoginFeedback = dataChannel.GetData();
                                    if (LoginFeedback.loginErrorMessage == "")
                                    {
                                        user = LoginFeedback;
                                        foreach (ChatModel chat in user.chats)
                                        {
                                            UIChatsMessages.Add(new UIMessages(chat, MessagesScrollViewGrid));
                                            AddChatButton(chat);
                                        }
                                        await ShowNoSelectedChat();
                                        //await setCurrentChat(user.chats[0]);
                                        //ShowChat(currentChat.id);

                                        foreach (FriendInfo friend in user.friends)
                                        {
                                            AddFriendToUI(friend);
                                        }

                                        ShowSuccessfullMessage("Login Succesfull!");

                                        isLoggedin = true;
                                        LoginGrid.Visibility = Visibility.Collapsed;
                                        MainGrid.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        ShowErrorMessage(LoginFeedback.loginErrorMessage);
                                    }
                                }
                                if (dataChannel.GetDataType() == DataType.RegistrationFeedback)
                                {
                                    string registerFeedback = dataChannel.GetData();
                                    if (registerFeedback == "Registered!")
                                    {
                                        RegisterGrid.Visibility = Visibility.Collapsed;
                                        LoginGrid.Visibility = Visibility.Visible;
                                        ShowSuccessfullMessage("Account Created");
                                    }
                                    else ShowErrorMessage(registerFeedback); ;
                                }
                                continue;
                            }
                            if (dataChannel.GetDataType() == DataType.FriendSearch)
                            {
                                FriendInfo addFriendInfo = dataChannel.GetData();
                                if (addFriendInfo.FriendID != -1)
                                {
                                    ShowSuccessfullMessage($"Friend {addFriendInfo.FriendUsername} successfully added!");

                                    AddFriendToUI(addFriendInfo);

                                    user.friends.Add(addFriendInfo);
                                }
                                else
                                {
                                    ShowErrorMessage($"User '{addFriendInfo.FriendUsername}' was not found.");
                                }
                            }

                            if (dataChannel.GetDataType() == DataType.Message)
                            {
                                await AddMessageAsync(dataChannel.GetData());
                                await Task.Delay(2);
                                continue;
                            }

                            if (dataChannel.GetDataType() == DataType.NewChat)
                            {
                                ChatModel newChat = dataChannel.GetData();
                                if (newChat.id == -1)
                                {
                                    ShowErrorMessage("Error occured during chat creating.");
                                    continue;
                                }
                                if (newChat.members.Find(x => x.UserId == user.id).Rank == MemberRank.Creator)
                                {
                                    ShowErrorMessage("Chat created succesfully!");
                                }
                                if (newChat.messages.Count > 0)
                                {
                                    UIChatsMessages.Add(new UIMessages(newChat, MessagesScrollViewGrid));
                                    //UIChatsMessageBoxes_text.Add(new UIMessagesBlockPlainText(newChat.id, newChat.name, newChat.messages));
                                }
                                else
                                    UIChatsMessages.Add(new UIMessages(newChat.id));
                                user.chats.Add(newChat);
                                AddChatButton(newChat);
                                if (user.chats.Count == 1)
                                {
                                    await setCurrentChat(user.chats[0]);
                                    await ShowChat(currentChat.id);
                                }
                            }

                            if (dataChannel.GetDataType() == DataType.ChatsUpdate)
                            {
                                List<ChatModel> updatedChats = dataChannel.GetData();
                                ChatsPropertiesUpdate(updatedChats);
                            }

                            if (dataChannel.GetDataType() == DataType.ChatNameChange)
                            {
                                ChatModel chatNewName = dataChannel.GetData();
                                if (chatNewName.id != -1)
                                {
                                    if (currentChat.id == chatNewName.id)
                                    {
                                        currentChat.ChangeName(chatNewName.name);
                                    }
                                    user.chats[user.chats.FindIndex(x => x.id == chatNewName.id)].ChangeName(chatNewName.name);
                                    ShowSuccessfullMessage("Name changed!");


                                    SetChatButtonName(chatNewName.id, chatNewName.name);

                                    mainViewModel.CurrentChatNameContent = currentChat.name;

                                    CurrentChatNameTB.Visibility = Visibility.Visible;
                                    ChangeChatNameTB.Visibility = Visibility.Collapsed;
                                    isChatNameChanging = !isChatNameChanging;
                                    ChangeChatNameBtn.Content = "Change name";
                                }
                                else
                                {
                                    ShowErrorMessage(chatNewName.name);
                                }
                            }
                            if (dataChannel.GetDataType() == DataType.ChatMemberRankChange)
                            {
                                ChatModel chatChangedMember = dataChannel.GetData();
                                if (chatChangedMember.id != -1)
                                {
                                    if (chatChangedMember.members.Count == 1)
                                    {
                                        int chatsIndex = user.chats.FindIndex(x => x.id == chatChangedMember.id);
                                        if (chatChangedMember.members[0].Rank == MemberRank.Kicked)
                                        {
                                            if (user.id == chatChangedMember.members[0].UserId)
                                            {
                                                await RemoveChat(chatChangedMember.id);
                                                continue;
                                            }
                                            else
                                            {
                                                user.chats[chatsIndex].members.RemoveAt(user.chats[chatsIndex].members.FindIndex(x => x.UserId == chatChangedMember.members[0].UserId));
                                            }
                                        }
                                        else
                                        {
                                            user.chats[chatsIndex].members[user.chats[chatsIndex].members.FindIndex(x => x.UserId == chatChangedMember.members[0].UserId)].Rank = chatChangedMember.members[0].Rank;
                                        }
                                        if (currentChat.id == chatChangedMember.id)
                                        {
                                            currentChat.members = user.chats[chatsIndex].members;
                                            await CheckIfSpectator();
                                        }
                                        await UpdateCurrentChatMembersList();
                                        await UpdateAddNewMembersListUI();
                                    }
                                }
                            }

                            if (dataChannel.GetDataType() == DataType.NewChatMembers)
                            {
                                ChatModel newMembersChat = dataChannel.GetData();
                                if (newMembersChat.members.Count > 0)
                                {
                                    int chat_index = user.chats.FindIndex(x => x.id == newMembersChat.id);
                                    user.chats[chat_index].members.AddRange(newMembersChat.members);
                                    if (currentChat.id == newMembersChat.id)
                                    {
                                        currentChat.members = user.chats[chat_index].members;
                                        await UpdateCurrentChatMembersList();
                                        await UpdateAddNewMembersListUI();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(streamRec.ReadTimeout);
                    }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine(receivedData);
                    throw e;
                    //Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAM" + e.Message);
                    await Task.Delay(streamRec.ReadTimeout);
                    continue;
                }
                await Task.Delay(200);
            }
        }


        void AddFriendToUI(FriendInfo friend)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Content = friend.FriendUsername;
            listBoxItem.Style = (Style)FindResource("StyledListBoxItem");
            FriendsListLB.Items.Add(listBoxItem);

            ListBoxItem listBoxItem1 = new ListBoxItem();
            CheckBox checkBox = new CheckBox();
            checkBox.Content = friend.FriendUsername;
            checkBox.Name = "N" + Convert.ToString(friend.FriendID);
            checkBox.Checked += FriendCB_Checked;
            checkBox.Unchecked += FriendCB_Unchecked;
            listBoxItem1.Content = checkBox;
            listBoxItem1.MouseUp += ListBoxItemCheckBoxChange;
            listBoxItem1.Style = (Style)FindResource("StyledListBoxItem");
            newChatMembers.Add(checkBox);
            CreateChatFriendsListLB.Items.Add(listBoxItem1);
        }
        async Task RemoveChat(int chatID)
        {
            int chatButtonIndex = chatsButtons.FindIndex(x => x.Name == "ID" + chatID);
            ChatsGrid.Children.Remove(chatsButtons[chatButtonIndex]);
            chatsButtons.RemoveAt(chatButtonIndex);

            user.chats.RemoveAt(user.chats.FindIndex(x => x.id == chatID));
            UIChatsMessages.RemoveAt(UIChatsMessages.FindIndex(x => x.chatID == chatID));

            if (currentChat.id == chatID)
            {
                currentChat = new ChatModel();
            }
            await ShowNoSelectedChat();
            return;
        }
        void ShowErrorMessage(string message)
        {
            ErrorMessageBlock.Background = Brushes.Red;
            ShowMessage(message);
        }
        void ShowSuccessfullMessage(string message)
        {
            ErrorMessageBlock.Background = Brushes.Green;
            ShowMessage(message);
        }
        void ShowMessage(string message)
        {
            var storyboard = (Storyboard)FindResource("ShowErrorMessage");
            storyboard.Stop();

            ErrorMessageTB.Text = message;
            ErrorMessageBlock.Visibility = Visibility.Visible;

            storyboard.Begin();
        }
        async Task ShowNoSelectedChat()
        {
            CurrentChatOnlineCountTB.Visibility = Visibility.Collapsed;
            ChatPropertiesBtn.Visibility = Visibility.Collapsed;
            MessageSendingGrid.Visibility = Visibility.Collapsed;
            AddChatMemberGrid.Visibility = Visibility.Collapsed;
            ChatPropertiesGrid.Visibility = Visibility.Collapsed;
            MessagesScrollView.Visibility = Visibility.Collapsed;
            MessagesScrollView.Content = new Grid();
            if (user.chats.Count == 0)
            {
                mainViewModel.CurrentChatNameContent = "No chats available.";
                MessagesTB.Text = "\nNo chats available.";
            }
            else
            {
                mainViewModel.CurrentChatNameContent = "";
                MessagesTB.Text = "\nPlease select a chat.";
            }
            return;
        }
        async Task UpdateCurrentChatMembersList()
        {
            CurrentChatMembersLB.Items.Clear();
            if (currentChat.id == -1 || currentChat.members.Count == 0)
            {
                return;
            }
            currentChat.members.Sort((x, y) => y.IsOnline.CompareTo(x.IsOnline));
            currentChat.members.Sort((x, y) => x.Rank.CompareTo(y.Rank));
            mainViewModel.CurrentChatNameContent = currentChat.name;
            if (!isChatNameChanging)
            {
                ChangeChatNameTB.Text = currentChat.name;
            }
            int i = 0;
            foreach (ChatMember member in currentChat.members)
            {
                ChatMemberItem chatMemberItem = new ChatMemberItem();
                chatMemberItem.MouseUp += ChatMember_MouseUp;
                chatMemberItem.Name = "ID" + member.UserId;

                chatMemberItem.MemberName = member.Username;

                string rankString = member.Rank.ToString();

                chatMemberItem.Rank = rankString;

                if (member.UserId == user.id)
                {
                    chatMemberItem.isYou = true;

                    if (rankString == MemberRank.Creator.ToString())
                    {
                        ChangeChatNameBtn.Visibility = Visibility.Visible;
                    }
                    if (rankString == MemberRank.Administrator.ToString())
                    {
                        ChangeChatNameBtn.Visibility = Visibility.Visible;
                    }
                    if (rankString == MemberRank.Member.ToString())
                    {
                        ChangeChatNameBtn.Visibility = Visibility.Collapsed;
                    }
                    if (rankString == MemberRank.Spectator.ToString())
                    {
                        ChangeChatNameBtn.Visibility = Visibility.Collapsed;
                    }
                }

                if (rankString == MemberRank.Creator.ToString())
                {
                    chatMemberItem.RankColumnForeground = Brushes.Purple;
                }
                if (rankString == MemberRank.Administrator.ToString())
                {
                    chatMemberItem.RankColumnForeground = Brushes.Red;
                }
                if (rankString == MemberRank.Spectator.ToString())
                {
                    chatMemberItem.RankColumnForeground = Brushes.Chocolate;
                }

                string statusString = "Offline";
                if (member.IsOnline)
                {
                    chatMemberItem.StatusColumnForeground = Brushes.Blue;
                    statusString = "Online";
                }
                chatMemberItem.Status = statusString;

                if (member.UserId == contextMenuSelectedUserID)
                {
                    chatMemberItem.IsSelect = true;
                }
                CurrentChatMembersLB.Items.Add(chatMemberItem);
            }
            return;
        }
        async Task setCurrentChat(ChatModel chatModel)
        {
            if (IsTypingCalled)
            {
                await SendIsTypingUpdate(false);
                IsTypingCalled = false;
                IsTypingCalled_First = true;
                IsTypingCalled_Again = false;
            }
            HideChatMembersContextMenu();
            contextMenuSelectedUserID_Previous = -1;
            currentChat = chatModel;
            await UpdateCurrentChatMembersList();
            await UpdateAddNewMembersListUI();
            return;
        }

        async Task UpdateAddNewMembersListUI()
        {

            AddMemberListLB.Items.Clear();
            chatNewMembers.Clear();
            newChatMembersList.Clear();
            if (currentChat.id == -1 || currentChat.members.Count == 0)
            {
                return;
            }
            foreach (FriendInfo friend in user.friends)
            {
                if (currentChat.members.FindIndex(x => x.UserId == friend.FriendID) != -1)
                {
                    continue;
                }
                ListBoxItem listBoxItem2 = new ListBoxItem();
                CheckBox checkBox1 = new CheckBox();
                checkBox1.Content = friend.FriendUsername;
                checkBox1.Name = "N" + Convert.ToString(friend.FriendID);
                checkBox1.Checked += FriendCB_Checked;
                checkBox1.Unchecked += FriendCB_Unchecked;
                listBoxItem2.Content = checkBox1;
                chatNewMembers.Add(checkBox1);
                listBoxItem2.Style = (Style)FindResource("StyledListBoxItem");
                AddMemberListLB.Items.Add(listBoxItem2);
            }
            return;
        }

        async void ChatsPropertiesUpdate(List<ChatModel> updatedChats_)
        {
            if (currentChat.id != -1)
            {
                if (updatedChats_.Count > 0)
                {
                    for (int i = 0; i < user.chats.Count; i++)
                    {
                        ChatModel updatedChat = updatedChats_.Find(x => x.id == user.chats[i].id);
                        if (updatedChat == null)
                        {
                            await RemoveChat(user.chats[i].id);
                            continue;
                        }
                        if (!user.chats[i].name.Equals(updatedChat.name))
                        {
                            user.chats[i].name = updatedChat.name;
                            SetChatButtonName(user.chats[i].id, user.chats[i].name);
                            mainViewModel.CurrentChatNameContent = currentChat.name;
                        }
                        user.chats[i].members = updatedChat.members;
                        List<ChatMember> typingChatMembers = updatedChat.members.FindAll(x => x.IsTyping);
                        if (typingChatMembers.Count > 0)
                        {
                            string typingMembersString = "";
                            typingChatMembers.Sort((x, y) => x.Rank.CompareTo(y.Rank));
                            for (int i1 = 0; i1 < typingChatMembers.Count; i1++)
                            {
                                typingMembersString += $"{typingChatMembers[i1].Username}";
                                if (i1 != typingChatMembers.Count-1)
                                {
                                    typingMembersString += ", ";
                                }
                            }
                            typingMembersString += " typing...";
                            SetChatButtonLastMessage(user.chats[i].id, typingMembersString);
                        }
                        else
                        {
                            if (user.chats[i].messages.Count > 0)
                            {
                                SetChatButtonLastMessage(user.chats[i].id, user.chats[i].messages.Last());
                            }
                            else
                            {
                                SetChatButtonLastMessage(user.chats[i].id, "No messages yet...");
                            }
                        }
                        if (currentChat.id == user.chats[i].id)
                        {
                            currentChat.members = user.chats[i].members;
                            await UpdateOnlineUI();
                        }
                    }

                }
                await UpdateCurrentChatMembersList();
            }
            return;
        }


        async Task UpdateOnlineUI()
        {
            int currentChatOnlineCount = 0;
            currentChatOnlineCount = currentChat.members.FindAll(x => x.IsOnline == true).Count;
            CurrentChatOnlineCountTB.Text = currentChatOnlineCount + " Online";
            return;
        }
        async Task ShowChat(int chatID)
        {
            ChangeChatNameTB.Visibility = Visibility.Collapsed;
            ChangeChatNameBtn.Visibility = Visibility.Collapsed;
            CurrentChatNameTB.Visibility = Visibility.Visible;
            ChatPropertiesBtn.Visibility = Visibility.Visible;
            MessageSendingGrid.Visibility = Visibility.Visible;
            MessagesScrollView.Visibility = Visibility.Visible;
            ChangeChatNameBtn.Content = "Change name";
            isChatNameChanging = false;

            await setCurrentChat(user.chats.Find(x => x.id == chatID));
            await CheckIfSpectator();
            CurrentChatOnlineCountTB.Visibility = Visibility.Visible;
            int currentChatOnlineCount = 0;
            currentChatOnlineCount = currentChat.members.FindAll(x => x.IsOnline == true).Count;
            CurrentChatOnlineCountTB.Text = currentChatOnlineCount + " Online";
            if (currentChat.messages == null)
            {
                currentChat.messages = new List<MessageModel>();
            }
            mainViewModel.CurrentChatNameContent = currentChat.name;
            int UI_Index = UIChatsMessages.FindIndex(x => x.chatID == chatID);
            while (!UIChatsMessages[UI_Index].isBlockCompleted)
            {
                await Task.Delay(200);
            }
            //Debug.WriteLine(UIChatsMessages[UI_Index]._UIMessageBlocks.Count);
            MessagesScrollView.Content = UIChatsMessages[UI_Index].MessagesUIElement;
            //Debug.WriteLine(GetStringFromXamlDebug(MessagesScrollView));
            MessagesScrollView.ScrollToEnd();
            await UpdateOnlineUI();
        }
        //async Task ShowChat(string chatName)
        //{
        //    ChangeChatNameTB.Visibility = Visibility.Collapsed;
        //    ChangeChatNameBtn.Visibility = Visibility.Collapsed;
        //    CurrentChatNameTB.Visibility = Visibility.Visible;
        //    ChatPropertiesBtn.Visibility = Visibility.Visible;
        //    MessageSendingGrid.Visibility = Visibility.Visible;
        //    ChangeChatNameBtn.Content = "Change name";
        //    isChatNameChanging = false;

        //    await setCurrentChat(user.chats.Find(x => x.name == chatName));
        //    await CheckIfSpectator();
        //    CurrentChatOnlineCountTB.Visibility = Visibility.Visible;
        //    int currentChatOnlineCount = 0;
        //    currentChatOnlineCount = currentChat.members.FindAll(x => x.IsOnline == true).Count;
        //    CurrentChatOnlineCountTB.Text = currentChatOnlineCount + " Online";
        //    if (currentChat.messages == null)
        //    {
        //        currentChat.messages = new List<MessageModel>();
        //    }
        //    mainViewModel.CurrentChatNameContent = currentChat.name;
        //    MessagesTB.Text = UIChatsMessageBoxes_text.Find(x => x.chatName == chatName).messagesBlock;
        //    MessagesTB.ScrollToEnd();
        //    await UpdateOnlineUI();
        //}

        async Task ShowChatLong(string chatName)//
        {
            await setCurrentChat(user.chats.Find(x => x.name == chatName));
            MessagesTB.Text = string.Empty;
            if (currentChat.messages == null)
            {
                currentChat.messages = new List<MessageModel>();
            }
            foreach (MessageModel message in currentChat.messages)
            {
                await Task.Delay(1);
                //await MessagesBlockWriteLine($"[{message.SentTime}] {message.SenderName}: {message.Content}");
            }
        }

        async Task CheckIfSpectator()
        {
            if (currentChat.id != -1)
                if (currentChat.members.Find(x => x.UserId == user.id).Rank == MemberRank.Spectator)
                {
                    MessageSendingGrid.Visibility = Visibility.Collapsed;
                    AddMemberBtn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageSendingGrid.Visibility = Visibility.Visible;
                    AddMemberBtn.Visibility = Visibility.Visible;
                }
            return;
        }

        void AddChatButton(ChatModel chat)
        {
            ChatButton chatButton = new ChatButton();
            chatButton.Name = "ID" + chat.id;
            chatButton.Visibility = Visibility.Visible;
            chatButton.ChatName = chat.name;
            if (chat.messages.Count > 0)
            {
                MessageModel lastMessageModel = chat.messages[chat.messages.Count - 1];
                chatButton.SetLastMessage(lastMessageModel);
            }
            chatButton.MouseUp += Chat_MouseUp;
            ChatsGrid.Children.Add(chatButton);
            chatsButtons.Add(chatButton);
        }
        private void SetChatButtonName(int chatID, string newChatName)
        {
            ChatButton chatButton = chatsButtons.Find(x => x.Name == "ID" + chatID);
            if (chatButton != null)
            {
                chatButton.ChatName = newChatName;
            }
        }
        private void SetChatButtonLastMessage(int chatID, string newLastMessage)
        {
            ChatButton chatButton = chatsButtons.Find(x => x.Name == "ID" + chatID);
            if (chatButton != null)
            {
                chatButton.LastMessage = newLastMessage;
            }
        }
        private void SetChatButtonLastMessage(int chatID, MessageModel newLastMessage)
        {
            ChatButton chatButton = chatsButtons.Find(x => x.Name == "ID" + chatID);
            if (chatButton != null)
            {
                chatButton.SetLastMessage(newLastMessage);
            }
        }
        public static T CloneElement<T>(T element) where T : UIElement
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // Serialize the element to XAML
            string xaml = XamlWriter.Save(element);

            // Parse the XAML to create a new element
            using (StringReader stringReader = new StringReader(xaml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    return (T)XamlReader.Load(xmlReader);
                }
            }
        }
        public string GetStringFromXamlDebug(object grid)
        {
            return XamlWriter.Save(grid);
        }
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    return (T)child;
                }
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginInfo loginInfo = new LoginInfo
            {
                Username = LoginTB.Text,
                Password = PasswordTB.Text,
            };
            try
            {
                DataChannel channel = new DataChannel(loginInfo);
                string dataChannel = JsonConvert.SerializeObject(channel);
                MessageSender(JsonConvert.SerializeObject(new DataChannel(loginInfo)));
            }
            catch (Exception i)
            {

                throw i;
            }
        }

        private void RegisterBackBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegisterGrid.Visibility = Visibility.Visible;
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string registerFeedback = string.Empty;
            if (RPasswordTB.Text != RPasswordRepeatTB.Text)
            {
                registerFeedback = "Passwords do not match.";
            }
            if (RPasswordTB.Text.Length <= 6 || RPasswordTB.Text.Length > 20 || RPasswordTB.Text.IndexOfAny(new char[] { ' ', '\\', '/', '.', '$', '@', '|' }) != -1)
            {
                registerFeedback = "Improper password. Password must be 8 to 20 characters long and not contain any spaces and any of these characters('\\', '/', '.', '$', '@', '|')";
            }
            if (RLoginTB.Text.Length <= 3 || RLoginTB.Text.Length > 15 || RLoginTB.Text.IndexOfAny(new char[] { ' ', '\\', '/', '.', '$', '@', '|' }) != -1)
            {
                registerFeedback = "Improper username. Username must be 3 to 15 characters long and not contain any spaces and any of these characters('\\', '/', '.', '$', '@', '|')";
            }
            if (EmailTB.Text.Length <= 5 || EmailTB.Text.Length > 30 || !EmailTB.Text.Contains('@') || !EmailTB.Text.Contains('.') || EmailTB.Text.IndexOfAny(new char[] { ' ', '\\', '/', '$', '|' }) != -1)
            {
                registerFeedback = "Improper Email adress.";
            }

            if (registerFeedback == string.Empty)
            {
                RegistrationInfo registerInfo = new RegistrationInfo
                {
                    Email = EmailTB.Text,
                    Password = RPasswordTB.Text,
                    Username = RLoginTB.Text,
                };
                MessageSender(JsonConvert.SerializeObject(new DataChannel(registerInfo)));
            }
            else
            {
                ShowErrorMessage(registerFeedback);
            }
            //MessageSender($"|REG|EML={EmailTB.Text}|USRNM={RLoginTB.Text}|PSSWRD={RPasswordTB.Text}|END|");
        }

        private void LogInBackBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        private async void Chat_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChatButton customButton = (ChatButton)sender;
            //Debug.WriteLine($"border.Name.Substring(2)={border.Name.Substring(2)}");
            await ShowChat(Convert.ToInt32(customButton.Name.Substring(2)));
            //MessagesBlockWriteLine(border.Name);
        }

        private void FriendsBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                CloseCreateChatGrid();
                ChatPropertiesGrid.Visibility = Visibility.Collapsed;
                if (FriendsGrid.Visibility == Visibility.Visible)
                {
                    FriendsGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    FriendsGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private void CloseFriendsGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FriendsGrid.Visibility = Visibility.Collapsed;
            if (e.LeftButton == MouseButtonState.Released)
            {
            }
        }

        private void CloseFriendsGridBtn_Click(object sender, RoutedEventArgs e)
        {
            FriendsGrid.Visibility = Visibility.Collapsed;
        }

        private void AddFriendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FriendUsernameSearchTB.Text) && !string.IsNullOrWhiteSpace(FriendUsernameSearchTB.Text))
            {
                if (FriendUsernameSearchTB.Text == user.username)
                {
                    ShowErrorMessage("You cannot add yourself as a friend.");
                }
                else
                if (user.friends != null)
                    if (user.friends.FindIndex(x => x.FriendUsername == FriendUsernameSearchTB.Text) == -1)
                    {
                        FriendInfo addFriendInfo = new FriendInfo
                        {
                            FriendUsername = FriendUsernameSearchTB.Text,
                            UserID = user.id
                        };
                        DataChannel channel = new DataChannel(addFriendInfo);
                        SendData(channel);
                    }
                    else
                    {
                        ShowErrorMessage("User already in friends.");
                    }
            }
            else
            {
                ShowErrorMessage("Incorrect input.");
            }
        }

        private void FriendCB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (newChatMembersList.Count > 0)
            {
                int id = Convert.ToInt32((sender as CheckBox).Name.Substring(1));
                newChatMembersList.RemoveAt(newChatMembersList.FindIndex(x => x.UserId == id));
            }
            //Debug.WriteLine((sender as CheckBox).Content.ToString());
        }
        private void FriendCB_Checked(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as CheckBox).Name.Substring(1));
            //Debug.WriteLine(id);
            newChatMembersList.Add(new ChatMember { Username = (sender as CheckBox).Content.ToString(), UserId = id, Rank = MemberRank.Member });

            //Debug.WriteLine((sender as CheckBox).Content.ToString());
        }

        private void CreateChatBtn_Click(object sender, RoutedEventArgs e)
        {
            string newChatName = NewChatNameTB.Text.Trim();
            if (!string.IsNullOrEmpty(newChatName) && !string.IsNullOrWhiteSpace(newChatName) && newChatName.IndexOfAny(new char[] { '\\', '/', '.', '$', '@', '|' }) == -1)
            {
                if (newChatMembersList.Count > 0)
                {
                    List<ChatMember> chatMembers = new List<ChatMember>(newChatMembersList)
                    {
                        new ChatMember { UserId = user.id, Username = user.username, Rank = MemberRank.Creator, IsOnline = true }
                    };
                    DataChannel channel = new DataChannel(new ChatModel { name = newChatName, members = chatMembers });
                    SendData(channel);
                }
                else
                {
                    ShowErrorMessage("Please select at least 1 friend.");
                }
            }
            else
            {
                ShowErrorMessage("Improper chat name. Chat name must be 3 to 15 characters long and not contain any spaces and any of these characters('\\', '/', '.', '$', '@', '|')");
            }
        }

        private void NewChatNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            int firstIndex = NewChatNameTB.Text.IndexOf(' ');
            if (firstIndex == 0)
            {
                NewChatNameTB.Text = NewChatNameTB.Text.Remove(firstIndex);

                int caretIndex = NewChatNameTB.CaretIndex;
                NewChatNameTB.CaretIndex = caretIndex > NewChatNameTB.Text.Length ? NewChatNameTB.Text.Length : caretIndex;

            }

        }
        private void OpenChatCreateBtn_Click(object sender, MouseButtonEventArgs e)
        {
            GridSpace_MouseUp(sender, e);
            FriendsGrid.Visibility = Visibility.Collapsed;
            ChatPropertiesGrid.Visibility = Visibility.Collapsed;
            if (CreateChatGrid.Visibility == Visibility.Visible)
            {
                CloseCreateChatGrid();
            }
            else
            {
                CreateChatGrid.Visibility = Visibility.Visible;
            }
        }

        void CloseCreateChatGrid()
        {
            CreateChatGrid.Visibility = Visibility.Collapsed;
            for (int i = 0; i < newChatMembers.Count; i++)
            {
                newChatMembers[i].IsChecked = false;
            }
        }

        private void CloseCreateChatBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseCreateChatGrid();
        }

        private void CloseChatPropBtn_Click(object sender, RoutedEventArgs e)
        {
            ChatPropertiesGrid.Visibility = Visibility.Collapsed;
        }

        string spamTest = "SPAM-TEST";
        int spamTestCount = 0;
        bool IsSpamTest = false;
        private async void SpamTestBtn_Click(object sender, RoutedEventArgs e)
        {
            IsSpamTest = !IsSpamTest;
            while (IsSpamTest)
            {
                await Task.Delay(2);
                SendMessageStart(spamTest + spamTestCount++);
            }
        }

        private void ChatPropertiesBtn_Click(object sender, RoutedEventArgs e)
        {
            FriendsGrid.Visibility = Visibility.Collapsed;
            CloseCreateChatGrid();
            if (ChatPropertiesGrid.Visibility == Visibility.Visible)
            {
                ChatPropertiesGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                ChatPropertiesGrid.Visibility = Visibility.Visible;
            }
            ChatPropertiesGrid.Visibility = Visibility.Visible;
        }


        bool isChatNameChanging = false;
        private void ChangeChatNameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isChatNameChanging)
            {
                CurrentChatNameTB.Visibility = Visibility.Collapsed;
                ChangeChatNameTB.Visibility = Visibility.Visible;
                isChatNameChanging = !isChatNameChanging;
                ChangeChatNameBtn.Content = "Submit";
            }
            else
            {
                string chatNewName = ChangeChatNameTB.Text;
                if (!string.IsNullOrEmpty(chatNewName) && !string.IsNullOrWhiteSpace(chatNewName) && chatNewName.IndexOfAny(new char[] { '\\', '/', '.', '$', '@', '|' }) == -1)
                {
                    DataChannel channel = new DataChannel(new ChatModel { id = currentChat.id, name = chatNewName }, 0);
                    SendData(channel);
                }
                else
                {
                    ShowErrorMessage("Improper chat name. Chat name must be 3 to 15 characters long and not contain any spaces and any of these characters('\\', '/', '.', '$', '@', '|')");
                }


                ///////////// MOVE

            }
        }



        Grid membersContextMenu = new Grid();
        int currentMemberIndex = -1;
        int selectedMemberIndex = -1;

        private void EmptySpace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("EMPTY SPACE CLICK");
            if (e.LeftButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {
                ChatMembersContextMenu.Visibility = Visibility.Collapsed;
                contextMenuSelectedUserID = -1;
                UpdateCurrentChatMembersList();
            }

            CloseAddChatMemberGrid();
        }

        int contextMenuSelectedUserID = -1;
        int contextMenuSelectedUserID_Previous = -1;
        ChatMemberItem selectedMemberItem;
        private void ChatMember_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Right)
            {
                ChatMemberItem chatMemberItem = (sender as ChatMemberItem);

                if (ChatMembersContextMenu.Visibility == Visibility.Visible)
                {
                    ChatMembersContextMenu.Visibility = Visibility.Collapsed;
                    chatMemberItem.IsSelect = false;
                }
                int selectedUserID = Convert.ToInt32(chatMemberItem.Name.Substring(2));

                if (user.id != selectedUserID)
                {
                    currentMemberIndex = currentChat.members.FindIndex(x => x.UserId == user.id);
                    selectedMemberIndex = currentChat.members.FindIndex(x => x.UserId == selectedUserID);

                    MemberRank currentMemberRank = currentChat.members[currentMemberIndex].Rank;
                    MemberRank selectedMemberRank = currentChat.members[selectedMemberIndex].Rank;
                    ChatMembersContextMenu.Visibility = Visibility.Visible;


                    if (currentMemberRank == MemberRank.Creator || currentMemberRank == MemberRank.Administrator)
                    {
                        contextMenuSelectedUserID = selectedUserID;
                        UnselectPrevious();
                        contextMenuSelectedUserID_Previous = contextMenuSelectedUserID;
                        selectedMemberItem = chatMemberItem;

                        if (selectedMemberRank == MemberRank.Creator)
                        {
                            HideChatMembersContextMenu();
                        }
                        if (selectedMemberRank == MemberRank.Administrator)
                        {
                            ContextMenuBtn_1.Visibility = Visibility.Collapsed;
                            ContextMenuBtn_2.Visibility = Visibility.Collapsed;
                            ContextMenuBtn_3.Visibility = Visibility.Visible;
                            ContextMenuBtn_4.Visibility = Visibility.Visible;
                            ContextMenuBtn_5.Visibility = Visibility.Visible;
                        }
                        if (selectedMemberRank == MemberRank.Member)
                        {
                            ContextMenuBtn_1.Visibility = Visibility.Visible;
                            ContextMenuBtn_2.Visibility = Visibility.Collapsed;
                            ContextMenuBtn_3.Visibility = Visibility.Collapsed;
                            ContextMenuBtn_4.Visibility = Visibility.Visible;
                            ContextMenuBtn_5.Visibility = Visibility.Visible;
                        }
                        if (selectedMemberRank == MemberRank.Spectator)
                        {
                            ContextMenuBtn_1.Visibility = Visibility.Visible;
                            ContextMenuBtn_2.Visibility = Visibility.Visible;
                            ContextMenuBtn_3.Visibility = Visibility.Collapsed;
                            ContextMenuBtn_4.Visibility = Visibility.Collapsed;
                            ContextMenuBtn_5.Visibility = Visibility.Visible;
                        }
                    }
                    if (currentMemberRank == MemberRank.Member || currentMemberRank == MemberRank.Spectator)
                    {
                        HideChatMembersContextMenu();
                    }

                    Point clickPosition = e.GetPosition(ChatPropertiesGrid);
                    ChatMembersContextMenu.Margin = new Thickness(clickPosition.X, clickPosition.Y, 0, 0);
                }
                else
                {
                    contextMenuSelectedUserID = -1;
                }
                if (contextMenuSelectedUserID != -1)
                {
                    chatMemberItem.IsSelect = true;
                }
                else
                {
                    UnselectPrevious();
                }
            }

        }
        private void UnselectPrevious()
        {
            if (contextMenuSelectedUserID_Previous != -1)
            {
                selectedMemberItem.IsSelect = false;
                int index = CurrentChatMembersLB.Items.IndexOf(selectedMemberItem);
                if (index != -1)
                {

                    ChatMemberItem chatMemberItem = (CurrentChatMembersLB.Items.GetItemAt(index) as ChatMemberItem);
                    if (chatMemberItem != null)
                    {
                        chatMemberItem.IsSelect = false;
                        contextMenuSelectedUserID_Previous = -1;
                    }
                }
            }

        }
        private void HideChatMembersContextMenu()
        {
            ChatMembersContextMenu.Visibility = Visibility.Collapsed;
            contextMenuSelectedUserID = -1;
            contextMenuSelectedUserID_Previous = -1;
        }
        private void ContextMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = (sender as RoundedButton).Name;
            bool isKicked = false;
            MemberRank memberRank = MemberRank.Spectator;
            switch (buttonName)
            {
                case "ContextMenuBtn_1":
                    memberRank = MemberRank.Administrator;
                    break;
                case "ContextMenuBtn_2":
                    memberRank = MemberRank.Member;
                    break;
                case "ContextMenuBtn_3":
                    memberRank = MemberRank.Member;
                    break;
                case "ContextMenuBtn_4":
                    memberRank = MemberRank.Spectator;
                    break;
                case "ContextMenuBtn_5":
                    memberRank = MemberRank.Kicked;
                    isKicked = true;
                    break;
                default:
                    break;
            }
            ChatMember chatMember = currentChat.members[selectedMemberIndex];
            chatMember.Rank = memberRank;
            SendData(new DataChannel(new ChatModel(currentChat.id, currentChat.name, chatMember), 1));
            HideChatMembersContextMenu();
        }


        private void GridSpace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                (ChatPropertiesGrid.Content as Grid).Children.Remove(membersContextMenu);
            }
        }
        void CloseAddChatMemberGrid()
        {
            AddChatMemberGrid.Visibility = Visibility.Collapsed;
            for (int i = 0; i < chatNewMembers.Count; i++)
            {
                chatNewMembers[i].IsChecked = false;
            }
        }
        private void AddChatMemberBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddChatMemberGrid.Visibility == Visibility.Visible)
            {
                CloseAddChatMemberGrid();
            }
            else
            {
                AddChatMemberGrid.Visibility = Visibility.Visible;
            }
        }
        private void AddNewMembersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (newChatMembersList.Count > 0)
            {
                SendData(new DataChannel(new ChatModel(currentChat.id, currentChat.name, newChatMembersList), 2));
            }
        }

        private void ChangeChatNameTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ChangeChatNameBtn_Click(new object(), new RoutedEventArgs());
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var ripple = button.Template.FindName("ripple", button) as Ellipse;
            if (ripple == null) return;

            var rippleTransform = button.Template.FindName("rippleTransform", button) as TranslateTransform;
            if (rippleTransform == null) return;

            var buttonGrid = button.Template.FindName("buttonGrid", button) as Grid;
            if (buttonGrid == null) return;

            var position = e.GetPosition(button);
            ripple.Width = ripple.Height = 0;
            ripple.Opacity = 0.5;  // Initial opacity before animation

            // Center the ripple at the click position
            rippleTransform.X = position.X - (ripple.Width / 2);
            rippleTransform.Y = position.Y - (ripple.Height / 2);

            // Clip to the button's border
            buttonGrid.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, button.ActualWidth, button.ActualHeight),
                RadiusX = 15,
                RadiusY = 15
            };

            var animation = new Storyboard();
            var opacityAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(opacityAnimation, ripple);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Ellipse.OpacityProperty));

            var widthAnimation = new DoubleAnimation
            {
                From = 0,
                To = button.ActualWidth * 2,  // Ensure it covers the button
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(widthAnimation, ripple);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Ellipse.WidthProperty));

            var heightAnimation = new DoubleAnimation
            {
                From = 0,
                To = button.ActualHeight * 2,  // Ensure it covers the button
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(heightAnimation, ripple);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Ellipse.HeightProperty));

            animation.Children.Add(opacityAnimation);
            animation.Children.Add(widthAnimation);
            animation.Children.Add(heightAnimation);

            animation.Begin();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as RippleEffectDecorator).Background = Brushes.LightGray;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as RippleEffectDecorator).Background = Brushes.White;
        }

        private void SendBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                if (SendingMessageTB.Foreground != Brushes.LightGray)
                {
                    SendMessageStart(SendingMessageTB.Text);
                }
        }
        private void SendingMessageTB_GotFocus(object sender, RoutedEventArgs e)
        {
            //ChangeCaretColor(sender as TextBox, Colors.White);
        }

        private void ShowErrorMessage_Completed(object sender, EventArgs e)
        {
            ErrorMessageBlock.Visibility = Visibility.Collapsed;
        }

        private void ListBoxItemCheckBoxChange(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)sender;
            CheckBox checkBox = (CheckBox)listBoxItem.Content;
            checkBox.IsChecked = !checkBox.IsChecked;
        }

        bool IsTypingCalled = false;
        bool IsTypingCalled_First = true;
        bool IsTypingCalled_Again = false;
        private async void MessageTypingChanged(object sender, TextChangedEventArgs e)
        {
            IsTypingCalled_Again = true;
            if(!IsTypingCalled)
            while (true)
            {
                if (IsTypingCalled_First)
                {
                    await SendIsTypingUpdate(true);
                    IsTypingCalled_First = false;
                }
                IsTypingCalled_Again = false;
                IsTypingCalled = true;
                await Task.Delay(3000);
                if (!IsTypingCalled_Again)
                {
                    Debug.WriteLine("FALSE");
                    IsTypingCalled = false;
                    await SendIsTypingUpdate(false);
                    IsTypingCalled_First = true;
                    break;
                }
            }
        }
        private async Task SendIsTypingUpdate(bool isTyping_)
        {
            DataChannel dataChannel = new DataChannel(
                            new IsTypingUpdate
                            {
                                isTyping = isTyping_,
                                typingChatID = currentChat.id,
                                typingUserID = user.id
                            }
                        );
            await SendData(dataChannel);
        }
    }
    public class ScrollingTextBox : TextBox
    {

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        protected override async void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            CaretIndex = Text.Length;
            ScrollToEnd();
        }

    }
}