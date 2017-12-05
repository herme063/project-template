using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MvvmValidation;

namespace Simple.Wpf.Util
{
    public abstract class ValidatableObservableObject : 
        ObservableObject,
        INotifyDataErrorInfo
    {
        private readonly ValidationHelper _validator;
        private readonly NotifyDataErrorInfoAdapter _notifyDataErrorInfoAdapter;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _notifyDataErrorInfoAdapter.HasErrors;

        protected ValidationHelper Validator => _validator;

        public ValidatableObservableObject()
        {
            _validator = new ValidationHelper();
            _notifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(_validator);
            ErrorsChanged += OnErrorsChanged;
        }

        public IEnumerable GetErrors(string propertyName) => _notifyDataErrorInfoAdapter.GetErrors(propertyName);

        public async Task<ValidationResult> ValidateAsync()
        {
            return await _validator.ValidateAllAsync();
        }

        protected bool SetNValidate<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            bool result = Set(propertyExpression, ref field, newValue);
             _validator.Validate(propertyExpression);

            return result;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(() => HasErrors);
        }
    }
}