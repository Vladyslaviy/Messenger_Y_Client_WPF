using System.Diagnostics.Metrics;

namespace Messenger_Y_Client_WPF
{
    public struct LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public struct RegistrationInfo
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public class ChatMember
    {
        public string Username { get; set; }
        public int UserId { get; set; }
        public MemberRank Rank { get; set; }
        public bool IsOnline { get; set; } = false;
        //public bool IsYou { get; set; } = false;
        public bool IsTyping { get; set; } = false;
        public ChatMember()
        {

        }
        public ChatMember(string Username_, int UserId_, MemberRank Rank_)
        {
            Username = Username_;
            UserId = UserId_;
            Rank = Rank_;
        }
    }

    public class UserModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public List<ChatModel> chats { get; set; }
        public List<FriendInfo> friends { get; set; }
        public string loginErrorMessage { get; set; } = "";
    }

    public class ChatModel
    {
        public int id { get; set; } = -1;
        public string name { get; set; }
        public List<ChatMember> members { get; set; }
        public List<MessageModel> messages { get; set; }

        public ChatModel()
        {
            members = new List<ChatMember>();
            messages = new List<MessageModel>();
            name = string.Empty;
        }
        public ChatModel(int id_, string name_, ChatMember member)
        {
            id = id_;
            name = name_;
            members = new List<ChatMember> { member };
        }
        public ChatModel(int id_, string name_, List<ChatMember> members_)
        {
            id = id_;
            name = name_;
            members = members_;
        }
        public void ChangeName(string name_)
        {
            name = name_;
        }
    }

    public class MessageModel
    {
        public int id { get; set; }
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public int ChatID { get; set; }
        public ChatMember SenderInfo { get; set; }

    }

    public struct FriendInfo
    {
        public string FriendUsername { get; set; }
        public int FriendID { get; set; }
        public int UserID { get; set; }

    }

    public class FriendModel
    {

    }

    public enum DataType
    {
        Message,
        RegistrationFeedback,
        LoginFeedback,
        LoginInfo,
        RegistrationInfo,
        FriendSearch,
        NewChat,
        ChatsUpdate,
        ChatNameChange,
        ChatMemberRankChange,
        NewChatMembers,
        TypingUpdate
    }

    public enum MemberRank
    {
        Creator,
        Administrator,
        Member,
        Spectator,
        Kicked
    }

    public struct IsTypingUpdate
    {
        public bool isTyping { get; set; }
        public int typingChatID { get; set; }
        public int typingUserID { get; set; }
    }
    public class DataChannel
    {
        public DataType dataType;

        public MessageModel Message;
        public string RegistrationFeedback;
        public UserModel LoginFeedback;
        public LoginInfo loginInfo;
        public RegistrationInfo registrationInfo;
        public FriendInfo addFriendInfo;
        public ChatModel newChat;
        public List<ChatModel> updatedChats;

        public ChatModel chatChange;

        public IsTypingUpdate isTypingUpdate;

        public DataChannel()
        {

        }
        public DataChannel(IsTypingUpdate isTypingUpdate_)
        {
            isTypingUpdate = isTypingUpdate_;
            dataType = DataType.TypingUpdate;
        }
        public DataChannel(ChatModel chatChange_, int chatChangeSpec)
        {
            switch (chatChangeSpec)
            {
                case 0:
                    dataType = DataType.ChatNameChange;
                    break;
                case 1:
                    dataType = DataType.ChatMemberRankChange;
                    break;
                case 2:
                    dataType = DataType.NewChatMembers;
                    break;
                default:
                    break;
            }
            chatChange = chatChange_;
        }
        public DataChannel(List<ChatModel> updatedChats_)
        {
            dataType = DataType.ChatsUpdate;
            updatedChats = updatedChats_;
        }
        public DataChannel(ChatModel newChat_)
        {
            dataType = DataType.NewChat;
            newChat = newChat_;
        }
        
        public DataChannel(FriendInfo addFriendInfo_)
        {
            dataType = DataType.FriendSearch;
            addFriendInfo = addFriendInfo_;
        }
        public DataChannel(MessageModel message)
        {
            dataType = DataType.Message;
            Message = message;
        }
        public DataChannel(string registrationFeedback)
        {
            dataType = DataType.RegistrationFeedback;
            RegistrationFeedback = registrationFeedback;
        }
        public DataChannel(UserModel loginFeedback)
        {
            dataType = DataType.LoginFeedback;
            LoginFeedback = loginFeedback;
        }
        public DataChannel(LoginInfo loginInfo_)
        {
            dataType = DataType.LoginInfo;
            loginInfo = loginInfo_;
        }
        public DataChannel(RegistrationInfo registrationInfo_)
        {
            dataType = DataType.RegistrationInfo;
            registrationInfo = registrationInfo_;
        }
        public dynamic GetData()
        {
            switch (dataType)
            {
                case DataType.Message:
                    return Message;
                case DataType.RegistrationFeedback:
                    return RegistrationFeedback;
                case DataType.LoginFeedback:
                    return LoginFeedback;
                case DataType.LoginInfo:
                    return loginInfo;
                case DataType.RegistrationInfo:
                    return registrationInfo;
                case DataType.FriendSearch:
                    return addFriendInfo;
                case DataType.NewChat:
                    return newChat;
                case DataType.ChatsUpdate:
                    return updatedChats;
                case DataType.ChatNameChange:
                    return chatChange;
                case DataType.ChatMemberRankChange:
                    return chatChange;
                case DataType.NewChatMembers:
                    return chatChange;
                case DataType.TypingUpdate:
                    return isTypingUpdate;
            }
            return -1;
        }
        public DataType GetDataType()
        {
            return dataType;
        }
    }



    internal class SerializerModels
    {
    }
}
