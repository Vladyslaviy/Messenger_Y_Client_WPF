using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Y_Client_WPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _chatNameTextBoxContent;

        

        public string CurrentChatNameContent
        {
            get { return _chatNameTextBoxContent; }
            set
            {
                if (_chatNameTextBoxContent != value)
                {
                    _chatNameTextBoxContent = value;
                    OnPropertyChanged(nameof(CurrentChatNameContent));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
