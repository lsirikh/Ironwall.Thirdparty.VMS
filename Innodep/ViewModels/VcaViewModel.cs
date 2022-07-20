using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaViewModel 
        : Screen
    {
        #region - Ctors -
        public VcaViewModel()
        {

        }
        public VcaViewModel(VcaModel model)
        {
            Model = model;
            Update();
            
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void Update()
        {
            NotifyOfPropertyChange(() => Id);
            NotifyOfPropertyChange(() => TypeDevice);
            NotifyOfPropertyChange(() => NameDevice);
            NotifyOfPropertyChange(() => SerialDevice);
            NotifyOfPropertyChange(() => Channel);
            NotifyOfPropertyChange(() => Status);
            NotifyOfPropertyChange(() => Used);
            NotifyOfPropertyChange(() => IsSelected);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Id
        {
            get { return Model.Id; }
            set
            {
                Model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public int TypeDevice
        {
            get { return Model.TypeDevice; }
            set 
            { 
                Model.TypeDevice = value;
                NotifyOfPropertyChange(() => TypeDevice);
            }
        }

        public string NameDevice
        {
            get { return Model.NameDevice; }
            set
            {
                Model.NameDevice = value;
                NotifyOfPropertyChange(() => NameDevice);
                if (Notify != null)
                    Notify(this, new PropertyNotifyEventArgs() { Property = "NameDevice" });
            }
        }

        public int SerialDevice
        {
            get { return Model.SerialDevice; }
            set
            {
                Model.SerialDevice = value;
                NotifyOfPropertyChange(() => SerialDevice);
            }
        }

        public int Channel
        {
            get { return Model.Channel; }
            set
            {
                Model.Channel = value;
                NotifyOfPropertyChange(() => Channel);
            }
        }

        public int Status
        {
            get { return Model.Status; }
            set
            {
                Model.Status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        public bool Used
        {
            get { return Model.Used; }
            set
            {
                Model.Used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }

        public VcaModel Model
        {
            get { return _model; }
            set 
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }


        public string TokenAuth
        {
            get { return _tokenAuth; }
            set 
            { 
                _tokenAuth = value;
                NotifyOfPropertyChange(() => TokenAuth);
            }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            { 
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public CancellationTokenSource Cts { get; set; }

        #endregion
        #region - Attributes -
        private string _tokenAuth;
        private VcaModel _model;
        private bool _isSelected;
        public event EventHandler Notify;
        #endregion

    }

}
