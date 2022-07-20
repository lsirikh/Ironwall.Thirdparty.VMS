using Caliburn.Micro;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaTaskTimer
    {
        #region - Ctors -
        public VcaTaskTimer(
            VcaSetupModel setupModel
            , IEventAggregator eventAggregator
            )
        {
            SetupModel = setupModel;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            LoginModel = new VcaLoginModel();
        }
        #endregion
        #region - Implementation of Interface -
        public void InitTimer()
        {
            if (timer != null)
                timer.Close();

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(LoginTimeout);
            SetTimerEnable(true);
            SetSession(SetupModel.TimeIntervalKeepAliveVca);
        }


        #endregion
        #region - Overrides -

        public void SetTimerEnable(bool value)
        {
            timer.Enabled = value;
        }

        /*private void SetExpire()
        {
            Expire = DateTime.Now + TimeSpan.FromMinutes(Session);
        }*/

        public bool SetSession(int time = 1)
        {
            if (time == 0)
                return false;
            //Input Value will be minutes
            timer.Interval = TimeSpan.FromMinutes(time).TotalMilliseconds; ;
            Session = time;

            return true;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        private async void LoginTimeout(object sender, ElapsedEventArgs e)
        {
            LoginModel = new VcaLoginModel();
            await LogoutAndLogin();
            Debug.WriteLine($"[TaskTimer][LoginTimeout]Called!!");
        }


        #region - Processes -
        public Task LogoutAndLogin()
        {
            if (SetupModel.ApiUsed)
            {
                return Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await LogoutApi();
                        await Task.Delay(500);
                        await LoginApi(true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[TaskTimer][LogoutAndLogin]{ex.Message}");
                    }
                    finally
                    {
                        Debug.WriteLine($"[TaskTimer][LogoutAndLogin] Completed Task...");
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task LoginApi(bool force_login = false)
        {
            return Task.Factory.StartNew(async () =>
            {
                RestClient client = new RestClient(UrlVca);

                var api = "api/login";
                var request = new RestRequest(api, Method.Get);
                request.AddHeader("x-account-id", IdUserVca);
                request.AddHeader("x-account-pass", PasswordVca);
                request.AddHeader("x-account-group", GroupVca);
                request.AddHeader("x-license", LicenceVca);

                if (force_login)
                    request.AddParameter("force-login", "true");

                try
                {
                    var response = await client.ExecuteAsync<LoginResponseModel>(request);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][LoginApi({force_login})] success:{response.Data?.success}, code:{response.Data?.code}, message:{response.Data?.message}, detail:{response.Data?.detail}");
                        //return false;
                    }
                    else
                    {
                        var data = response.Data;
                        Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][LoginApi({force_login})] success:{data?.success}, auth_token:{data?.results?.auth_token}, api_serial:{data?.results?.api_serial}");

                        SetupModel.TokenVca = data.results.auth_token;
                        LoginModel.IsLogin = true;
                        LoginModel.LoginTime = DateTime.Now;
                        LoginModel.Response = response.Data;
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[TaskTimer][LoginApi]{ex.Message}");
                    //return false;
                }
            });
            
        }

        public async Task<bool> LogoutApi()
        {
            RestClient client = new RestClient(UrlVca);
            var api = "api/logout";
            var request = new RestRequest(api, Method.Delete);

            var auth_token = "";
            if (LoginModel.IsLogin)
                auth_token = LoginModel.Response.results.auth_token;
            else
                auth_token = SetupModel.TokenVca;

            request.AddHeader("x-auth-token", auth_token);

            try
            {
                var response = await client.ExecuteAsync<LogoutResponseModel>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][LogoutApi] success:{response.Data?.success}, code:{response.Data?.code}, message:{response.Data?.message}, detail:{response.Data?.detail}");
                    return false;
                }
                else
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss")}][LogoutApi] success:{response.Data?.success}, code:{response.Data?.code}, message:{response.Data?.message}, detail:{response.Data?.detail}");

                    LoginModel.IsLogin = false;
                    LoginModel.Response = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TaskTimer][LogoutApi]{ex.Message}");
                return false;
            }
            
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string UrlVca => $"http://{ SetupModel.IpAddressVca}:{SetupModel.PortVca}";
        public string IdUserVca => SetupModel.IdUserVca;
        private string PasswordVca => SetupModel.PasswordVca;
        private string GroupVca => SetupModel.GroupVca;
        private string LicenceVca => SetupModel.LicenceVca;

        protected VcaSetupModel SetupModel { get; }
        private IEventAggregator _eventAggregator;

        //protected DateTime Expire { get; set; }
        protected int Session { get; set; }
        protected VcaLoginModel LoginModel { get; set; }
        #endregion
        #region - Attributes -
        private Timer timer;
        #endregion
    }
}