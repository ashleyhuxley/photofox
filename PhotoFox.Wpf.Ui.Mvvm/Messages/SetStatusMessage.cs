namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class SetStatusMessage
    {
        public SetStatusMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
