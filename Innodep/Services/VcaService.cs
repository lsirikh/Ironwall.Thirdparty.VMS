using Caliburn.Micro;
using Ironwall.Framework.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ironwall.Libraries.Utils;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.RTSP.Models.Messages;
using Ironwall.Libraries.RTSP.Models;
using Ironwall.Libraries.RTSP.DataProviders;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public sealed class VcaService
        : VcaTaskTimer
        , IService
        , IHandle<SendVmsPtzControlMessageModel>
        
    {

        #region - Ctors -
        public VcaService(
            VcaProvider vcaProvider
            , PresetProvider presetProvider
            , VcaSetupModel setupModel
            , IEventAggregator eventAggregator): base(setupModel, eventAggregator)
        {
            _vcaProvider = vcaProvider;
            _presetProvider = presetProvider;
        }
        #endregion
        #region - Implementation of Interface -
        public async Task ExecuteAsync(CancellationToken token = default)
        {
            InitTimer();
            
            if(SetupModel.ApiUsed)
                await LoginApi(true);
        }

        public void Stop()
        {
        }
        #endregion
        #region - Implementations for IHandle -

        /*
        public async Task HandleAsync(SendSymbolCameraMessageModel message, CancellationToken cancellationToken)
        {
            try
            {
                BuildLookupTable();
                var lookup = LookupTable.Where(record => record.VcaViewModel.NameDevice == message.Value.NameDevice);
                var value = message.Value;
                var nameDevice = value.NameDevice;
                Debug.WriteLine($"Message: {nameDevice}");


                if (!lookup.Any())
                    throw new NullReferenceException();

                foreach (var item in lookup)
                {
                    if (item.VcaViewModel.Status != (int)EnumCameraStatus.ACTIVE)
                        continue;

                    var serialDevice = item.VcaViewModel.SerialDevice;
                    var channelDevice = item.VcaViewModel.Channel;

                    await SendVcaApi(item.VcaViewModel);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VcaService][HandleAsync(SendSymbolCameraMessageModel)]{ex.Message}");
            }
        }
        */
        /*
        public async Task HandleAsync(SendVncMessageModel message, CancellationToken cancellationToken = default)
        {
            try
            {
                BuildLookupTable();
                var lookup = LookupTable.Where(record => record.PresetViewModel.NameArea == message.Value.NameArea);
                var value = message.Value;
                var nameDevice = value.NameDevice;
                Debug.WriteLine($"Message: {nameDevice}");

                if (!lookup.Any())
                    throw new NullReferenceException();

                foreach (var item in lookup)
                {
                    if (item.VcaViewModel.Status != (int)EnumCameraStatus.ACTIVE)
                        continue;

                    RestClient client = new RestClient(UrlVca);


                    switch (item.VcaViewModel.TypeDevice)
                    {
                        case (int)EnumCameraType.NONE:
                            break;
                        case (int)EnumCameraType.FIXED:
                            await Task.Factory.StartNew(async () => 
                            { 
                                await SendVcaApi(item.VcaViewModel);
                            });
                            break;
                        case (int)EnumCameraType.PTZ:


                            await Task.Factory.StartNew(async() => 
                            {
                                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}]Start Task");
                                await SendVcaApi(item.VcaViewModel);
                                await Task.Delay(500);
                                await SendPtzPresetApi(item.VcaViewModel, item.PresetViewModel);

                                for (int i = 0; i < item.PresetViewModel.ControlTime; i++)
                                {
                                    if (item.VcaViewModel.Cts.IsCancellationRequested)
                                        break;
                                    await Task.Delay(1000);
                                }
                                //await Task.Delay(item.PresetViewModel.ControlTime * 1000, token);
                                await SendPtzHomeApi(item.VcaViewModel, item.PresetViewModel);
                                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}]Finish Task");
                            });
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VcaService][HandleAsync(SendVncMessageModel)]{ex.Message}");
            }
        }
        */

        public async Task HandleAsync(SendVmsPtzControlMessageModel message, CancellationToken cancellationToken)
        {
            try
            {
                if (!(message.Value is CameraPresetModel model))
                    return;

                await CameraPresetPtzControl(model);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VcaService][HandleAsync(SendVmsPtzControlMessageModel)]{ex.Message}");
            }
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes - 
        public void BuildLookupTable()
        {
            LookupTable =
                from camera in _vcaProvider
                join preset in _presetProvider
                on new { camera.NameDevice }
                equals new { preset.NameDevice }
                select new VcaLookupViewModel
                {
                    VcaViewModel = camera,
                    PresetViewModel = preset
                };
        }

        public Task CameraPresetPtzControl(CameraPresetModel model)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    var cameraDeviceProvider = IoC.Get<CameraDeviceProvider>();
                    VcaViewModel model1 = null;
                    VcaViewModel model2 = null;
                    foreach (var item in _vcaProvider
                    .Where(t => t.TypeDevice == (int)EnumCameraType.PTZ).ToList())
                    {
                        if (item.Status != (int)EnumCameraStatus.ACTIVE)
                            continue;

                        if (item.NameDevice == model.CameraFirst
                        && IsModeCheck(item, EnumCameraMode.INNODEP_API))
                            model1 = item;
                        else if (item.NameDevice == model.CameraSecond
                        && IsModeCheck(item, EnumCameraMode.INNODEP_API))
                            model2 = item;
                    }

                    if (model1 != null)
                    {
                        if (model1.Cts != null)
                            model1.Cts.Cancel();

                        await Task.Delay(100);
                        model1.Cts = new CancellationTokenSource();
                        await CamearPtzCycle(model1, model.HomePresetFirst, model.TargetPresetFirst, model.ControlTime);

                    }

                    if (model2 != null)
                    {
                        if (model2.Cts != null)
                            model2.Cts.Cancel();

                        await Task.Delay(100);
                        model2.Cts = new CancellationTokenSource();
                        await CamearPtzCycle(model2, model.HomePresetSecond, model.TargetPresetSecond, model.ControlTime);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][VcaService][CameraPresetPtzControl]Raised Exception : {ex.Message}");
                }

            });
        }

        private bool IsModeCheck(VcaViewModel item, EnumCameraMode mode)
        {
            //CameraDeviceProvider에서 API로 세팅된 Camera 찾기
            var deviceProvider = IoC.Get<CameraDeviceProvider>();
            return deviceProvider.Where(t => t.Name == item.NameDevice
            && t.Mode == (int)mode).Count() > 0 ? true : false;
        }

        private Task CamearPtzCycle(VcaViewModel model, string homePresetFirst, string targetPresetFirst, int controlTime)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    await CameraPresetMoveAsync(model, targetPresetFirst);
                    await Task.Delay(TimeSpan.FromSeconds(controlTime), model.Cts.Token);
                    if (model.Cts.IsCancellationRequested)
                    {
                        Debug.WriteLine($"{model.NameDevice} was Cancelled!");
                        return;
                    }

                    await CameraPresetMoveAsync(model, homePresetFirst);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][VcaService][CamearPtzCycle]Raised Exception : {ex.Message}");
                }

            }, model.Cts.Token);
        }

        public Task CameraPresetMoveAsync(VcaViewModel item, string preset)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][VcaService]{item.Model.NameDevice}({preset}) PTZ Move Command!.....");
                    await SendPtzPresetApi(item, preset);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][VcaService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
                }
            });
        }

        /*private async Task<bool> SendPtzHomeApi(VcaViewModel vcaViewModel, PresetViewModel presetViewModel)
        {
            RestClient client = new RestClient(UrlVca);

            if (!LoginModel.IsLogin)
            {
                await LogoutAndLogin();
                await Task.Delay(2000);
            }

            var api = $"api/device/function/ptz/{vcaViewModel.SerialDevice}/{vcaViewModel.Channel}";
            var request = new RestRequest(api, Method.Get);
            var auth_token = LoginModel.Response.results.auth_token;
            var api_serial = LoginModel.Response.results.api_serial;

            request.AddHeader("x-auth-token", auth_token);
            request.AddHeader("x-api-serial", api_serial);

            request.AddParameter("cmd", 32);
            request.AddParameter("preset_no", presetViewModel.Home);

            try
            {
                var response = await client.ExecuteAsync<TcpResponseModel>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][SendPtzHomeApi] success:{response.Data.success}, code:{response.Data.code}, message:{response.Data.message}, detail:{response.Data.detail}");
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    var data = response.Data;
                    InitTimer();
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][SendPtzHomeApi] success:{data.success}, cmd:{data.results.cmd}, preset:{data.results.preset_no}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VcaService][SendPtzHomeApi]{ex.Message}");
                return false;
            }
        }*/

        private async Task<bool> SendPtzPresetApi(VcaViewModel vcaViewModel, string preset)
        {
            RestClient client = new RestClient(UrlVca);

            if (!LoginModel.IsLogin)
            {
                await LogoutAndLogin();
                await Task.Delay(2000);
            }

            var api = $"api/device/function/ptz/{vcaViewModel.SerialDevice}/{vcaViewModel.Channel}";
            var request = new RestRequest(api, Method.Get);
            var auth_token = LoginModel.Response.results.auth_token;
            var api_serial = LoginModel.Response.results.api_serial;

            request.AddHeader("x-auth-token", auth_token);
            request.AddHeader("x-api-serial", api_serial);

            request.AddParameter("cmd", 32);
            request.AddParameter("preset_no", preset);

            try
            {
                var response = await client.ExecuteAsync<TcpResponseModel>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][SendPtzPresetApi] success:{response.Data.success}, code:{response.Data.code}, message:{response.Data.message}, detail:{response.Data.detail}");
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    var data = response.Data;
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][SendPtzPresetApi] success:{data.success}, cmd:{data.results.cmd}, preset:{data.results.preset_no}");
                    InitTimer();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VcaService][SendPtzPresetApi]{ex.Message}");
                return false;
            }
        }


        private async Task<bool> SendVcaApi(VcaViewModel vcaViewModel)
        {
            RestClient client = new RestClient(UrlVca);
            
            if (!LoginModel.IsLogin)
            {
                await LogoutAndLogin();
                await Task.Delay(2000);
            }

            var api = "api/event/send-vca";
            var request = new RestRequest(api, Method.Post);

            var auth_token = LoginModel.Response.results.auth_token;
            var api_serial = LoginModel.Response.results.api_serial;

            request.AddHeader("x-auth-token", auth_token);
            request.AddHeader("x-api-serial", api_serial);

            var jsonBody = new
            {
                dev_serial = vcaViewModel.SerialDevice,
                dch_ch = vcaViewModel.Channel,
                event_id = 1,
                event_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                event_msg = "A loitering event has occurred",
                event_status = 1
            };

            request.AddJsonBody(jsonBody);
            
            try
            {
                var response = await client.ExecuteAsync<VcaResponseModel>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][SendVcaApi] success:{response.Data.success}, code:{response.Data.code}, message:{response.Data.message}, detail:{response.Data.detail}");
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][SendVcaApi] success:{response.Data.success}, code:{response.Data.code}, message:{response.Data.message}, detail:{response.Data.detail}");

                    InitTimer();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[VcaService][SendVcaApi]{ex.Message}");
                return false;
            }
        }

        
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties - 
        private IEnumerable<VcaLookupViewModel> LookupTable { get; set; }
        #endregion
        #region - Attributes -
        private VcaProvider _vcaProvider;
        private PresetProvider _presetProvider;
        #endregion
    }
}
