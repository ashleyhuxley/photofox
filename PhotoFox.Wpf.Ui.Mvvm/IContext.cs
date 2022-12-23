using System;

namespace PhotoFox.Wpf.Ui.Mvvm
{
    public interface IContext
    {
        bool IsSynchronized { get; }
        void Invoke(Action action);
        void BeginInvoke(Action action);
    }
}
