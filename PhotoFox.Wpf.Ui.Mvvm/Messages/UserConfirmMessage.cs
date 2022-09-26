namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class UserConfirmMessage
    {
        public UserConfirmMessage(string messageText, string caption)
        {
            this.MessageText = messageText;
            Caption = caption;
        }

        public string MessageText { get; }

        public string Caption { get; }

        public bool IsConfirmed { get; set; }
    }
}
