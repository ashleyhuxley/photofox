using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    public interface IMessageHandler
    {
        void Register(Window ownerWindow);
    }
}
