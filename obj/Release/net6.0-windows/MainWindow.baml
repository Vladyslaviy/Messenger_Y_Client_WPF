   M S B A M L   `   `   `  ���� Q  MMessenger_Y_Client_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:Messenger_Y_Client_WPFMessenger_Y_Client_WPF  '    !Messanger_Y_Client_WPF.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6local$clr-namespace:Messenger_Y_Client_WPF     -     � Title$  Messenger Y��$	��800��$
��1600�� � 	Resources    RoundedTextBox �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding ��Name��%�   &	        �� " ��.���  � �� �� " ��.R��� #��z��#��z��$��1q�$ 10��$ 5q�._��� $ PART_ContentHost��.��� .+��� $A��;pack://application:,,,/images/black_white_background_2.jpeg��$��UniformToFill=�� -   	 �Name$	 MainGrid��$y�Visible=� 0   *Messenger_Y_Client_WPF.PercentageConverter  PercentageConverter��%   &	         .+�� -   $��0,0,0,0q�$��50��$
��Left=�$	��Top=�.+��� -   
���   �$��1q�$��0,0,0,0q�$��50��$��50��$	��Top=�$
��Left=�._��� $4��.pack://application:,,,/Icons/friends_icon3.jpg��$��50��$��50��$
��Fill=�$	��Top=�$
��Left=�� -   $��	53,5,0,60q�$
��Left=�$	��Top=�.+��� $
��Auto=�.���� -   
 ��Name$
 	ChatsGrid��$��0,0,0,5q�$	��157��$
��Left=�$	��Top=�.+��� -    ��Name$ ChatButtonExample��$y�	Collapsed=�$��1q�
���OO/�$��5,5,0,0q�$��40��$	��Top=�$
��Left=�$	��150��._�� 
�������.+��� $��1q�
���P��$��35��$��Center=�$��Center=�$	��130��._��� -    ��Name$ ChatExampleTB��$
��Wrap=�$��EXAMPLE��$��18(�$��Center=�
���   �$��Center=�� -	   $	 CurrentChatBar��$��	250,5,0,0q�$	��Top=�$��40��.+��� $��24(�$��40��$	��Top=�$��	0,0,115,0q����� *CurrentChatNameContent+ �� UpdateSourceTrigger$ PropertyChanged=��� -
   $ CurrentChatOnlineCountTB��$��Online��$��15(�$��	0,0,0,-25q�$��Bottom=�$
��Left=��� -    ��Name$ ChatPropertiesBtn��
������� J� Opacity$ 1Q�
���   �$y�Visible=�$��Chat Properties��$��0,-7,0,0q�$��Right=�$	��Top=�$��45��$	��110��� -   $��250,67,10,10q� � RowDefinitions �� $��* ��� $
��Auto �.+��� -    ��Name$ 
MessagesTB��$y�	Collapsed=�$��0��$�� ��$��0,10,0,5q�
������  �� TextWrapping$
 Wrap=� �� 
IsReadOnly$
 True��.�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     Color$ White��   	Direction$ 0Q�   ShadowDepth$ 0Q�   Opacity$ 1Q�.��� -   $	 MessagesScrollViewGrid��$��0��$��0,10,0,5q�
������  ��%   &	         .+��� -   $ MessagesScrollView��
������ .���� -   $
 MessagesStackPanel��$
��auto��.+�� -   $	 MessageTemplate��$��	10,0,0,10q�$��Bottom=�$
��Left=�$
��auto�� �� $
��Auto ��� $
��Auto �.+��� $��1��
�������$��1q�$ 6��
���   �$	 0.9Q�$
��1000��$
��auto��$��Bottom=�$
��Left=�._��  �� $��* ��� $
��Auto �.+��� $��TestAAAAAAAAAAAAA��$��0��$��18(�
�������$��5,5,5,5q�$	��Top=�$
��Left=��� $��TestTTTTTTTTTT��$��0��$��18(�
�������$��5,5,5,5q�$	��Top=�$��Right=��� $��1��$��5,5,5,5q�$
��Wrap=�$��a��
�������$��18(�� -   $	 MessageTemplate1��$��1��$��	10,0,0,10q�$��Bottom=�$
��Left=�$
��auto�� �� $��* ��� $
��Auto �.+��� $��1��
�������$��1q�$ 6��
���   �$	 0.9Q�$
��auto��$��Bottom=�$
��Left=����� *ActualWidth+ �� ElementName$ MessagesStackPanel�� �� 	Converter# [  �� ConverterParameter$ 65��._��  �� $��* ��� $
��Auto � � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $
��Test��$��0��$��0��$��18(�
�������$��5,5,5,5q�$	��Top=�$
��Left=��� $
��Test��$��0��$��1��$��18(�
�������$��5,5,5,5q�$	��Top=�$��Right=��� $��1��$��5,5,5,5q�$
��Wrap=�$����AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA��
�������$��18(�� -   $	 MessageSendingGrid��$��1��$
��auto��$��Bottom=�$y�Visible=�.+��� -   $ SendingMessageTB��#��[  $
 Wrap=�$��46��$��18(�$��Type a message...��
���   �
�������$	 0.8Q�$��0,0,85,0q�$��Bottom=��� -   $
��Send��$��46��$��Bottom=�$��Right=�$��80���� -   $��SpamTest��$y�	Collapsed=�$��46��$��Bottom=�$
��Left=�$��80��$��	415,0,0,0q��� -   $��Create Chat��$��Bottom=�$
��Left=�$��80��$��40��$��	125,0,0,5q�� -   $	 CreateChatGrid��$y�	Collapsed=�
�������$	��600��$	��350��$��0,0,0,0q�$��Center=�$��Center=�.+��� -   $ NewChatNameTB��$��0,35,0,0q�$��25��$	��300��$	��Top=�$��Center=��� $��260,108,0,0q�$��50��$	��Top=�$
��Left=�$	��290����Z� ._��� -   $ CreateChatFeedbackTB��$
��Wrap=�$�� ��$��13(�$��Center=�$��Center=��� -    ��Name$ CreateChatFriendsListLB��$��	10,69,0,0q�$	��220��$	��230��$
��Left=�$	��Top=�
��������� -   $��Create chat��$��
260,69,0,0q�$	��200��$��25��$	��Top=�$
��Left=��� -   $��X��$��0,0,0,0q�$��39��$��30��$	��Top=�$��Right=�� -   $	 FriendsGrid��$y�	Collapsed=�
�������$��0,0,0,0q�$��Center=�$��Center=�$	��650��$	��350��.+�� -   $��YourFriends��$��	10,10,0,0q�$	��Top=�$
��Left=�.+��� $��Your friends��$��20��$	��100��$	��Top=�$
��Left=��� -    $ FriendsListLB��$��0,21,0,0q�$	��220��$	��300��
�������$	��Top=�$
��Left=�� -!   $��	AddFriend��$��253,10,10,0q�.+��� $��
Add friend��$	��100��$��20��$	��Top=�$
��Left=��� $!��Enter your friend username:��$	��200��$��20��$��0,27,0,0q�$	��Top=�$
��Left=��� -"   $ FriendUsernameSearchTB��$��0,50,0,0q�$	��250��$��20��$	��Top=�$
��Left=��� -#   $	��Add��$��
255,48,0,0q�$��24��$��50��$	��Top=�$
��Left=��� -$   $ addFriendFeedbackTB��$�� ��$��	0,79,75,0q��� -%   $��0,0,0,0q�$��30��$��30��$��X��$	��Top=�$��Right=�� -&   $	 ChatPropertiesGrid��$y�	Collapsed=�
������$	��600��$	��350��$��105,70,105,70q�$��Center=�$��Center=�.+�� -'   
������.+�� $	��Top=�$��Center=�$��0,5,0,0q�.+��� -(   $ CurrentChatNameTB��$��25(����� *CurrentChatNameContent+$ PropertyChanged=��� -)   $ ChangeChatNameTB��
������$
��Test��$��Center=�$��0,0,0,0q�$��25(��� -*   $ ChangeChatNameBtn��$y�Visible=�$��60��$��
0,19,-65,0q�$��Change name��$��9(�$��Right=��� $��
266,54,0,0q�$��50��$	��Top=�$
��Left=�$	��290����Z� ._��� -+   $ ChatChangeNameFeedbackTB��$
��Wrap=�$�� ��$��13(�$��Center=�$��Center=��� $��Chat members:��$��10,24,467,296q�$��17(�� $��	10,54,0,0q�$	��250��$	��280��$
��Left=�$	��Top=� �� $��* ��� $
��Auto �.+��� -,   $ CurrentChatMembersLB��$��0��
������.@��� .��� $��24��$	��250��.+��� $��Center=�$
��Name���� $��Center=�$��(You)��$
��Left=�$��80,0,0,0q��� $��Center=�$��Status��$
��Left=�$��	186,0,0,0q��� $��Center=�$
��Rank��$
��Left=�$��	110,0,0,0q��� .��� $��21��$	��217��.+��� $
��Name���� $��Status��$��Right=��� --   $ AddMemberBtn��$��1��$y�Visible=�$��Bottom=�$��
Add member��$��24��$	��250��� -.   $	 ChatMembersContextMenu��$y�	Collapsed=�
������$	��Top=�$
��Left=�$	��120��$
��auto��$	 0.8Q�$��
125,89,0,0q�.+��� .+��� -/   $ ContextMenuBtn_1��$	��Top=�$��Center=�$	��120��$��25��$��Promote to Admin���� -0   $ ContextMenuBtn_2��$	��Top=�$��Center=�$	��120��$��25��$��Promote to Member���� -1   $ ContextMenuBtn_3��$	��Top=�$��Center=�$	��120��$��25��$��Demote to Member���� -2   $ ContextMenuBtn_4��$	��Top=�$��Center=�$	��120��$��25��$��Demote to Spectator���� -3   $ ContextMenuBtn_5��$	��Top=�$��Center=�$	��120��$��25��$��Kick from Chat��
���  ���� -4   $��X��$��0,0,0,0q�$��39��$��30��$	��Top=�$��Right=�� -5   $	 AddChatMemberGrid��$y�	Collapsed=�
�������$��140,30,140,30q�.+��� $��Friends:��$��Center=�$��20(�$	��Top=��� -6   $ AddMemberListLB��$��	0,24,0,25q�
�������.@��� .��Sssssssssssssssssss�� .��Sssssssssssssssssssa�� -7   $��0,0,0,0q�$��Add selected to chat��$��25��$��Bottom=�� -8   $	 	LoginGrid��$y�	Collapsed=�$��Center=�$��Center=�.+��� $��0,99,0,0q�$��37��$	��Top=�$��Center=�$	��276����Z� ._��� -9   $ LoginFeedbackTB��$
��Wrap=�$�� ��$��13(�$��Center=�$��Center=��� $
��Wrap=�$��Username��$��13(�$��	0,136,0,0q�$��16��$	��Top=�$��Center=�$	��234���� -:   $ LoginTB��$��	0,155,0,0q�$��25��$	��Top=�$��Center=�$	��249���� $
��Wrap=�$��Password��$��13(�$��	0,185,0,0q�$��16��$	��Top=�$��Center=�$	��234���� -;   $ 
PasswordTB��$��	0,204,0,0q�$��Center=�$	��249��$��26��$	��Top=��� -<   $��Log In��$��	0,242,0,0q�$��20��$	��Top=�$��Center=�$	��176���� $��	0,267,0,0q�$��16��$	��Top=�$��Center=�$	��234����Z� ._��� $
��Wrap=�$��OR��$��13(�$��Center=�$��Center=��� -=   $��Create a new account��$��	0,288,0,0q�$��25��$	��Top=�$��Center=�$	��176��� ->   $	 RegisterGrid��$y�	Collapsed=�$��Center=�$��Center=�.+��� $��0,24,0,0q�$��55��$	��Top=�$��Center=�$	��348����Z� ._��� -?   $ RegisterFeedbackTB��$
��Wrap=�$�� ��$��13(�$��Center=�$��Center=��� $
��Wrap=�$��Email��$��13(�$��0,84,0,0q�$��16��$	��Top=�$��Center=�$	��234���� -@   $ EmailTB��$��	0,106,0,0q�$��25��$	��Top=�$��Center=�$	��249���� $
��Wrap=�$��Username��$��13(�$��	0,136,0,0q�$��16��$	��Top=�$��Center=�$	��234���� -A   $ RLoginTB��$��	0,155,0,0q�$��25��$	��Top=�$��Center=�$	��249���� $
��Wrap=�$��Password��$��13(�$��	0,185,0,0q�$��16��$	��Top=�$��Center=�$	��234���� -B   $ RPasswordTB��$��	0,204,0,0q�$��Center=�$	��249��$��26��$	��Top=��� $
��Wrap=�$��Repeat your password��$��13(�$��	0,235,0,0q�$��16��$	��Top=�$��Center=�$	��234���� -C   $ RPasswordRepeatTB��$��	0,257,0,0q�$��Center=�$	��250��$��26��$	��Top=��� -D   $��Register��$��	0,297,0,0q�$��29��$	��Top=�$��Center=�$	��176���� $��	0,331,0,0q�$��16��$	��Top=�$��Center=�$	��234����Z� ._��� $
��Wrap=�$��OR��$��13(�$��Center=�$��Center=��� -E   $!��Log in to excisting account��$��	0,352,0,0q�$��29��$	��Top=�$��Center=�$	��176��