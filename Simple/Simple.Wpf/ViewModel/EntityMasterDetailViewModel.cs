using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using MvvmValidation;
using Simple.Wpf.Service;
using Simple.Wpf.Util;

namespace Simple.Wpf.ViewModel
{
    public class EntityMasterDetailViewModel : ViewModelBase
    {
        private readonly IEntityService _service;
        private readonly IDialogService _dialog;
        private bool _isBusy;
        private EntityObservable _editItem;
        private ObservableCollection<EntityObservable> _items;
        private string _validationSummary;
        private ICommand _editCommand;
        private ICommand _saveCommand;
        private ICommand _newCommand;
        private ICommand _loadCommand;
        private ICommand _removeCommand;

        public EntityObservable EditItem
        {
            get { return _editItem; }
            set { Set(() => EditItem, ref _editItem, value); }
        }

        public ObservableCollection<EntityObservable> Items
        {
            get { return _items; }
            set { Set(() => Items, ref _items, value); }
        }

        public string ValidationSummary
        {
            get { return _validationSummary; }
            set { Set(() => ValidationSummary, ref _validationSummary, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(() => IsBusy, ref _isBusy, value); }
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(async () => await SaveAsync()));

        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand<int>(async (id) => await RemoveAsync(id)));

        public ICommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand<int>(async (id) => await EditAsync(id)));

        public ICommand LoadCommand => _loadCommand ?? (_loadCommand = new RelayCommand(async () => await LoadAsync()));

        public ICommand NewCommand => _newCommand ?? (_newCommand = new RelayCommand(async () => await NewAsync()));

        public EntityMasterDetailViewModel(
            IDialogService dialog,
            IEntityService service)
        {
            _service = service;
            _dialog = dialog;
        }

        public override void Cleanup()
        {
            // TODO: Cleanup logic
            // eg: Unregister from messenger
            base.Cleanup();
        }

        private async Task LoadAsync(bool resetEntry = true)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsBusy = true;
            });

            List<Model.Entity> entities = await _service.AllAsync();

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsBusy = false;
                Items = new ObservableCollection<EntityObservable>(entities.Select(e => new EntityObservable(e, _service)));

                if (resetEntry)
                {
                    ValidationSummary = string.Empty;
                    EditItem = new EntityObservable(_service);
                }
            });
        }

        private async Task EditAsync(int itemId)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsBusy = true;
            });

            Model.Entity entity = await _service.GetById(itemId);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsBusy = false;
                ValidationSummary = string.Empty;
                EditItem = new EntityObservable(entity, _service);
            });
        }

        private async Task NewAsync()
        {
            if (EditItem?.Id > 0)
            {
                bool confirmed = await _dialog.ShowMessage(
                    Resource.Strings.Message_ConfirmDiscard,
                    Resource.Strings.Title_ConfirmDiscard,
                    Resource.Strings.Button_Yes,
                    Resource.Strings.Button_No, 
                    null);
                if (confirmed)
                {
                    await EditAsync(0);
                }
            }
        }

        private async Task SaveAsync()
        {
            ValidationSummary = string.Empty;

            ValidationResult validation = await EditItem.Validate();
            if (validation.IsValid)
            {
                var entity = new Model.Entity();
                EditItem.Apply(ref entity);

                Model.Entity savedEntity = await _service.SaveAsync(entity);
                await LoadAsync();
            }
            else
            {
                ValidationSummary = validation.ToString(new ValidationSummaryXamlFormatter());
            }
        }

        private async Task RemoveAsync(int id)
        {
            bool removeConfirmed = await _dialog.ShowMessage(
                Resource.Strings.Message_ConfirmDelete,
                Resource.Strings.Title_ConfirmDelete,
                Resource.Strings.Button_Yes,
                Resource.Strings.Button_No,
                null);
            if (removeConfirmed)
            {
                await _service.DeleteAsync(id);
                await LoadAsync(false);
            }
        }

        public class EntityObservable : ValidatableObservableObject
        {
            private int _id;
            private string _name;
            private readonly IEntityService _service;

            public int Id => _id;

            public string Name
            {
                get { return _name; }
                set { SetNValidate(() => Name, ref _name, value); }
            }

            public EntityObservable(IEntityService service)
            {
                _service = service;
                Validator.AddRequiredRule(() => Name, Resource.Strings.Message_IsRequired);
                Validator.AddAsyncRule(() => Name, async () =>
                { 
                    bool nameDuplicacy = await _service.NameDuplicateAsync(_id, Name);
                    return RuleResult.Assert(!nameDuplicacy, Resource.Strings.Message_AlreadyInUse);
                });
            }

            public EntityObservable(
                Model.Entity entity, 
                IEntityService service)
                : this(service)
            {
                _id = entity.Id;
                _name = entity.Name;
            }

            public void Apply(ref Model.Entity entity)
            {
                entity.Id = _id;
                entity.Name = _name;
            }
        }
    }
}