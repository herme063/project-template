using GalaSoft.MvvmLight.Views;
using System;
using System.Threading.Tasks;

namespace Simple.Wpf.Service
{
    public interface IDialogServiceEx : IDialogService
    {
        Task ShowBusy();
        Task ShowBusy(string message);
        Task HideBusy();
    }
}
