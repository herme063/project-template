using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MvvmValidation;

namespace Simple.Wpf.Util
{
    public abstract class ValidatableViewModelBase : 
        ViewModelBase,
        INotifyDataErrorInfo
    {
        private readonly ValidationHelper _validator;
        private readonly NotifyDataErrorInfoAdapter _notifyDataErrorInfoAdapter;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _notifyDataErrorInfoAdapter.HasErrors;

        protected ValidationHelper Validator => _validator;

        public ValidatableViewModelBase()
            : base()
        {
            _validator = new ValidationHelper();
            _notifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(_validator);
            ErrorsChanged += OnErrorsChanged;
        }

        public ValidatableViewModelBase(IMessenger messenger)
            : base(messenger)
        {
            _validator = new ValidationHelper();
            _notifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(_validator);
            ErrorsChanged += OnErrorsChanged;
        }

        public override void Cleanup()
        {
            ErrorsChanged -= OnErrorsChanged;
            base.Cleanup();
        }

        public IEnumerable GetErrors(string propertyName) => _notifyDataErrorInfoAdapter.GetErrors(propertyName);

        public async Task<ValidationResult> ValidateAsync()
        {
            return await _validator.ValidateAllAsync();
        }

        protected bool SetNValidate<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, bool broadcast = false)
        {
            bool result = Set(propertyExpression, ref field, newValue, broadcast);
             _validator.Validate(propertyExpression);

            return result;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(() => HasErrors);
        }
    }
}