using Caliburn.Micro;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class LoginVcaViewModel
        : BaseViewModel
    {
        #region - Ctors -
        public LoginVcaViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            #region - Settings -
            Id = 181;
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
            ViewModelBinder.Bind(this, IoC.Get<LoginVcaView>(), null);
            _vcaSetupModel = IoC.Get<VcaSetupModel>();

            ToggleNetwork = false;
            ToggleAccount = false;
            ToggleProperty = false;
            return base.OnActivateAsync(cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public bool CanClickButtonApply => !_vcaSetupModel.IsLoginApplied;

        public void ClickButtonApply()
        {
            _vcaSetupModel.IsLoginApplied = true;
            NotifyOfPropertyChange(nameof(CanClickButtonApply));
        }

        public bool CanClickButtonCancel => true;

        public void ClickButtonCancel()
        {
            _vcaSetupModel.IsLoginApplied = false;
            NotifyOfPropertyChange(nameof(CanClickButtonApply));
        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool ApiUsed
        {
            get { return _vcaSetupModel.ApiUsed; }
            set
            {
                _vcaSetupModel.ApiUsed = value;
                NotifyOfPropertyChange(() => ApiUsed);
            }
        }
        public string UserId
        {
            get { return _vcaSetupModel.IdUserVca; }
            set
            {
                _vcaSetupModel.IdUserVca = value;
                NotifyOfPropertyChange(() => UserId);
            }
        }

        public string Password
        {
            get { return _vcaSetupModel.PasswordVca; }
            set
            {
                _vcaSetupModel.PasswordVca = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public int First
        {
            get => int.Parse(_vcaSetupModel.IpAddressVca.Split('.')[0]);
            set
            {
                _first = value;
                _vcaSetupModel.IpAddressVca = $"{_first}.{Second}.{Third}.{Forth}";
                NotifyOfPropertyChange(() => First);
            }
        }

        public int Second
        {
            get => int.Parse(_vcaSetupModel.IpAddressVca.Split('.')[1]);
            set
            {
                _second = value;
                _vcaSetupModel.IpAddressVca = $"{First}.{_second}.{Third}.{Forth}";
                NotifyOfPropertyChange(() => Second);
            }
        }

        public int Third
        {
            get => int.Parse(_vcaSetupModel.IpAddressVca.Split('.')[2]);
            set
            {
                _third = value;
                _vcaSetupModel.IpAddressVca = $"{First}.{Second}.{_third}.{Forth}";
                NotifyOfPropertyChange(() => Third);
            }
        }

        public int Forth
        {
            get => int.Parse(_vcaSetupModel.IpAddressVca.Split('.')[3]);
            set
            {
                _forth = value;
                _vcaSetupModel.IpAddressVca = $"{First}.{Second}.{Third}.{_forth}";
                NotifyOfPropertyChange(() => Forth);
            }
        }

        public int Port
        {
            get { return _vcaSetupModel.PortVca; }
            set
            {
                _vcaSetupModel.PortVca = value;
                NotifyOfPropertyChange(() => Port);
            }
        }

        public string Liscense
        {
            get { return _vcaSetupModel.LicenceVca; }
            set
            {
                _vcaSetupModel.LicenceVca = value;
                NotifyOfPropertyChange(() => Liscense);
            }
        }

        public string Group
        {
            get { return _vcaSetupModel.GroupVca; }
            set
            {
                _vcaSetupModel.GroupVca = value;
                NotifyOfPropertyChange(() => Group);
            }
        }

        public bool ToggleNetwork { get; set; }
        public bool ToggleAccount { get; set; }
        public bool ToggleProperty { get; set; }
        #endregion
        #region - Attributes -
        private VcaSetupModel _vcaSetupModel;
        private int _first;
        private int _second;
        private int _third;
        private int _forth;
        #endregion
    }
}
