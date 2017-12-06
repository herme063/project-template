using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Simple.Wpf.ViewModel
{

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private CultureInfo _culture;
        private List<CultureInfo> _cultures;

        public List<CultureInfo> Cultures
        {
            get { return _cultures; }
            set { Set(() => Cultures, ref _cultures, value); }
        }

        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                if (Set(() => Culture, ref _culture, value))
                {
                    WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                    WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            Cultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("fr-FR")
            };
            Culture = Thread.CurrentThread.CurrentUICulture;
        }

        public override void Cleanup()
        {
            // TODO: Cleanup logic
            base.Cleanup();
        }

        private void SwitchCulture(CultureInfo ci)
        {
            Culture = ci;
        }
    }
}