using Caliburn.Micro;
using Dapper;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ironwall.Libraries.RTSP.DataProviders;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class CameraGridViewModel
        : BaseDataGridViewModel<VcaViewModel>
    {
        #region - Ctors -
        public CameraGridViewModel(IEventAggregator eventAggregator): base(eventAggregator)
        {
            #region - Settings -
            Id = 182;
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
            ViewModelBinder.Bind(this, IoC.Get<CameraGridView>(), null);

            DbConnection = IoC.Get<IDbConnection>();
            Provider = IoC.Get<VcaProvider>();
            VcaSetupModel = IoC.Get<VcaSetupModel>();
            _dbService = IoC.Get<VcaDbService>();

            PropertyUpdate += PresetGridViewModel_PropertyUpdate;
            return base.OnActivateAsync(cancellationToken);
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
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
                var viewModel = (e.Source as FrameworkElement).DataContext as VcaViewModel;
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
                if (Provider.CollectionEntity.Count() > 0)
                    id = Provider.CollectionEntity.Select((item) => item.Id).ToList().Max();
                Provider.Add(new VcaViewModel(new VcaModel() 
                { 
                    Id = id + 1, 
                    Status = 1,
                    Used = true 
                }));
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
                    await _dbService.FetchDevice();
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
                    await _dbService.SaveDevice();
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
            try
            {
                UpdateCameraComboList(SelectedItem.NameDevice);
                ClearNotifyEvent();
                if (SelectedItem != null)
                    SelectedItem.Notify += SelectedItem_Notify;
            }
            catch(Exception ex)
                {
                var result = ex.Message;
                Debug.WriteLine("PresetGridViewModel_PropertyUpdate: " + ex.Message);
            }


        }

        private void ClearNotifyEvent()
        {
            foreach (var item in Provider)
            {
                try
                {
                    item.Notify -= SelectedItem_Notify;
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("ClearNotifyEvent: " + ex.Message);
                }
            }
        }

        private Task UpdateCameraComboList(string value)
        {
            return Task.Run(() =>
            {
                var cameraComboProvider = IoC.Get<CameraComboProvider>();
                var comboList = new ObservableCollection<string>();
                DispatcherService.Invoke((System.Action)(() =>
                {
                    foreach (var item in cameraComboProvider.Provider)
                    {
                        if ((Provider.Where(t => t.NameDevice == item).Count() > 0)
                            && item != value)
                            continue;

                        comboList.Add(item);
                    }
                }));
                CameraComboList = comboList;
            });
        }

        private async Task ClearViewModelAsync()
        {
            await ClearSelection();
            SelectedItem = null;
            SelectedItemCount = 0;

            Provider = null;
        }

        private void SelectedItem_Notify(object sender, EventArgs e)
        {
            if (!(e is PropertyNotifyEventArgs args))
                return;

            if (SelectedItem.NameDevice != null)
                SetCameraType(SelectedItem.NameDevice);
        }

        private void SetCameraType(string nameDevice)
        {
            var cameraDeviceProvider = IoC.Get<CameraDeviceProvider>();

            var selectedCamera = cameraDeviceProvider.Where(t => t.Name == nameDevice).FirstOrDefault();

            if (selectedCamera == null)
                return;

            SelectedItem.TypeDevice = selectedCamera.TypeDevice;
        }
        #endregion                
        #region - IHanldes -
        #endregion
        #region - Properties -

        public ObservableCollection<string> CameraComboList
        {
            get { return _cameraComboList; }
            set 
            { 
                _cameraComboList = value;
                NotifyOfPropertyChange(() => CameraComboList);
            }
        }


        public VcaProvider Provider { get; private set; }
        public VcaSetupModel VcaSetupModel { get; private set; }
        #endregion
        #region - Attributes -
        private VcaViewModel _selectedITem;
        private VcaDbService _dbService;
        private ObservableCollection<string> _cameraComboList;
        #endregion
    }
}
