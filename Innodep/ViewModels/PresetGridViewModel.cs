using Caliburn.Micro;
using Dapper;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class PresetGridViewModel
        : BaseDataGridViewModel<PresetViewModel>
    {
        #region - Ctors -
        public PresetGridViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            #region - Settings -
            Id = 183;
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            ViewModelBinder.Bind(this, IoC.Get<PresetGridView>(), null);
            DbConnection = IoC.Get<IDbConnection>();
            Provider = IoC.Get<PresetProvider>();
            VcaSetupModel = IoC.Get<VcaSetupModel>();
            _dbService = IoC.Get<VcaDbService>();
            PropertyUpdate += PresetGridViewModel_PropertyUpdate;

            return base.OnActivateAsync(cancellationToken);
        }
        protected async override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(close, cancellationToken);
            await ClearViewModelAsync();
            PropertyUpdate -= PresetGridViewModel_PropertyUpdate;
        }

        

        public override async void OnClickCheckBoxItem(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkbox = e.Source as CheckBox;
                bool value = checkbox.IsChecked ?? false;
                var viewModel = (e.Source as FrameworkElement).DataContext as PresetViewModel;
                viewModel.IsSelected = value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await CheckSelectState();
            }
        }

        protected override async Task SelectAll(bool isSelected)
        {
            await Task.Run(() =>
            {
                foreach (var item in Provider)
                {
                    item.IsSelected = isSelected;
                }
            }).ContinueWith(async (t, _) => await CheckSelectState(), null, TaskScheduler.Default);
        }

        protected override async Task CheckSelectState(CancellationToken cancellationToken = default)
        {
            var count = 0;

            await Task.Run(() =>
            {
                foreach (var item in Provider)
                {
                    if (item.IsSelected == true)
                    {
                        count++;
                    }
                }
                //SelectedAccountCount = count;
            }, cancellationToken).ContinueWith((t, _) => SelectedItemCount = count, cancellationToken, TaskScheduler.Default);

            Debug.WriteLine($"선택된 항목 : {SelectedItemCount}");
        }
        #endregion
        #region - Binding Methods -
        public async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {
                var id = 0;
                if(Provider.CollectionEntity.Count() > 0)
                    id = Provider.CollectionEntity.Select((item) => item.Id).ToList().Max();
                Provider.Add(new PresetViewModel(new PresetModel() { Id = id + 1, Used = true }));
            });
        }

        public async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                var providerList = Provider.CollectionEntity.ToList();

                foreach (var item in providerList)
                {
                    if (item.IsSelected)
                        Provider.Remove(item);
                }

            }, _cancellationTokenSource.Token)
                .ContinueWith(async (t, _) => await ClearSelection(), null, TaskScheduler.Default);
        }

        public async void OnClickReloadButton(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel());
            await Task.Delay(500);
            await Task.Run(async () =>
            {
                try
                {
                    await Task.Run(() => Provider.Clear());
                    await _dbService.FetchPreset();
                    await ClearSelection();
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Theading to reload DB data Error: " + ex.Message);
                }
                finally
                {
                    await _eventAggregator?.PublishOnUIThreadAsync(new ClosePopupMessageModel());

                }
            });
        }

        public async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel());
            await Task.Delay(500);
            await Task.Run(async () =>
            {
                try
                {
                    await _dbService.SavePreset();
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Theading to insert DB data Error: " + ex.Message);
                }
                finally
                {
                    await _eventAggregator?.PublishOnUIThreadAsync(new ClosePopupMessageModel());
                }
            });
        }
        #endregion
        #region - Processes -
        private void PresetGridViewModel_PropertyUpdate(object sender, EventArgs e)
        {
            
            UpdateCameraComboList(SelectedItem.NameDevice);
            UpdateAreaComboList(SelectedItem.NameArea);
        }

        private Task UpdateCameraComboList(string value)
        {
            return Task.Run(() =>
            {
                var provider = IoC.Get<VcaProvider>();
                var cameraComboList = new ObservableCollection<string>();

                /*
                 * CameraComboList 업데이트
                 */
                DispatcherService.Invoke((System.Action)(() =>
                {
                    foreach (var item in provider)
                    {
                        /*if (
                            (Provider.Where(t => t.NameDevice == item.NameDevice).Count() > 0)
                            && item.NameDevice != value
                            )
                        {
                            continue;
                        }*/
                        cameraComboList.Add(item.NameDevice);
                    }
                }));
                CameraComboList = cameraComboList;
            });
        }

        private Task UpdateAreaComboList(string value)
        {
            return Task.Run(() =>
            {
                var provider = IoC.Get<AreaComboProvider>().Provider;
                var areaComboList = new ObservableCollection<string>();

                /*
                 * AreaComboList 업데이트
                 */

                DispatcherService.Invoke((System.Action)(() =>
                {
                    foreach (var item in provider)
                    {
                        /*if (
                            (Provider.Where(t => t.NameArea == item).Count() > 0)
                            && item != value
                            )
                        {
                            continue;
                        }*/
                        areaComboList.Add(item);
                    }
                }));
                AreaComboList = areaComboList;

            });
        }

        private async Task ClearViewModelAsync()
        {
            await ClearSelection();
            SelectedItem = null;
            SelectedItemCount = 0;

            Provider = null;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        /*public PresetViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                NotifyOfPropertyChange(() => SelectedViewModel);
                if (SelectedViewModel != null)
                {
                    UpdateCameraComboList(SelectedViewModel.NameDevice);
                    UpdateAreaComboList(SelectedViewModel.NameArea);
                }

            }
        }


        public bool IsCheckedCheckBoxColumnHeader
        {
            get => isCheckedCheckBoxColumnHeader;
            set
            {
                isCheckedCheckBoxColumnHeader = value;
                NotifyOfPropertyChange(() => IsCheckedCheckBoxColumnHeader);
            }
        }


        public int SelectedItemCount
        {
            get { return _selectedItemCount; }
            set
            {
                _selectedItemCount = value;
                NotifyOfPropertyChange(() => SelectedItemCount);
            }
        }
        */

        public ObservableCollection<string> AreaComboList
        {
            get { return _areaComboList; }
            set
            {
                _areaComboList = value;
                NotifyOfPropertyChange(() => AreaComboList);
            }
        }


        public ObservableCollection<string> CameraComboList
        {
            get { return _cameraComboList; }
            set
            {
                _cameraComboList = value;
                NotifyOfPropertyChange(() => CameraComboList);
            }
        }

        public PresetProvider Provider { get; private set; }
        public VcaSetupModel VcaSetupModel { get; private set; }
        //private IDbConnection DbConnection { get; set; }
        #endregion
        #region - Attributes -
        //private int _selectedItemCount;
        //private bool isCheckedCheckBoxColumnHeader;
        //private PresetViewModel _selectedViewModel;
        private VcaDbService _dbService;
        private ObservableCollection<string> _areaComboList;
        private ObservableCollection<string> _cameraComboList;
        #endregion
    }
}
