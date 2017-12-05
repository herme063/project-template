using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MvvmValidation;

namespace Simple.Wpf.Util
{
    /// <summary>
    /// Loosely based on https://github.com/pglazkov/MvvmValidation/blob/master/Examples/FormValidationExample/Infrastructure/ValidatableViewModelBase.cs
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.ObservableObject" />
    /// <seealso cref="System.ComponentModel.INotifyDataErrorInfo" />
    public abstract class ValidatableObservableObject : 
        ObservableObject,
        IValidatable,
        INotifyDataErrorInfo
    {
        private readonly ValidationHelper _validator;
        private readonly NotifyDataErrorInfoAdapter _notifyDataErrorInfoAdapter;

        protected ValidationHelper Validator => _validator;

        public ValidatableObservableObject()
        {
            _validator = new ValidationHelper();
            _notifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(_validator);
            _notifyDataErrorInfoAdapter.ErrorsChanged += OnErrorsChanged;
        }

        protected bool SetNValidate<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            bool result = Set(propertyExpression, ref field, newValue);
             _validator.Validate(propertyExpression);

            return result;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #region IValidatable

        public Task<ValidationResult> Validate()
        {
            return _validator.ValidateAllAsync();
        }

        #endregion IValidatable

        #region INotifyDataErrorInfo

        public IEnumerable GetErrors(string propertyName) => _notifyDataErrorInfoAdapter.GetErrors(propertyName);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { _notifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { _notifyDataErrorInfoAdapter.ErrorsChanged += value; }
        }

        public bool HasErrors => _notifyDataErrorInfoAdapter.HasErrors;

        #endregion INotifyDataErrorInfo
    }
}