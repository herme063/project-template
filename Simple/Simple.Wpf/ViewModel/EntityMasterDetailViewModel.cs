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
        private EntityObservable _selectedItem;
        private ObservableCollection<EntityObservable> _items;
        private ICommand _saveCommand;
        private ICommand _loadCommand;
        private ICommand _removeCommand;
        private readonly IDialogService _dialog;

        public EntityObservable SelectedItem
        {
            get { return _selectedItem; }
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        public ObservableCollection<EntityObservable> Items
        {
            get { return _items; }
            set { Set(() => Items, ref _items, value); }
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(async () => await SaveAsync()));

        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand<int>(async (id) => await RemoveAsync(id)));

        public ICommand LoadCommand => _loadCommand ?? (_loadCommand = new RelayCommand(async () => await LoadAsync()));

        public EntityMasterDetailViewModel(
            IEntityService service,
            IDialogService dialog)
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
            List<Model.Entity> entities = await _service.AllAsync();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Items = new ObservableCollection<EntityObservable>(entities.Select(e => new EntityObservable(e, _service)));

                if (resetEntry)
                {
                    SelectedItem = new EntityObservable(_service);
                }
            });
        }

        private async Task SaveAsync()
        {
            ValidationResult validation = await SelectedItem.ValidateAsync();
            if (validation.IsValid)
            {
                var entity = new Model.Entity();
                SelectedItem.Apply(ref entity);

                Model.Entity savedEntity = await _service.SaveAsync(entity);
                await LoadAsync();
            }
            else
            {
                // TODO: Raise validation
            }
        }

        private async Task RemoveAsync(int id)
        {
            bool removeConfirmed = await _dialog.ShowMessage(
                "Are you sure to remove this entity?",
                "Confirm Removal",
                "Yes",
                "No",
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
                Validator.AddRequiredRule(() => Name, "Name required");
                Validator.AddAsyncRule(() => Name, async () =>
                { 
                    bool nameDuplicacy = await _service.NameDuplicateAsync(_id, Name);
                    return RuleResult.Assert(!nameDuplicacy, "Name already in use");
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