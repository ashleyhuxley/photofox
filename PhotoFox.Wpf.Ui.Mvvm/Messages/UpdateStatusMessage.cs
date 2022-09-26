namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class UpdateStatusMessage
    {
        public string Message { get; set; }

        public UpdateStatusMessage(string message)
        {
            this.Message = message;
        }
    }
}
