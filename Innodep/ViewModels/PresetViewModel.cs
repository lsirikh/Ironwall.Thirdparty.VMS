using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class PresetViewModel
        : Screen
    {
        #region - Ctors -
        public PresetViewModel()
        {

        }
        public PresetViewModel(PresetModel model)
        {
            Model = model;
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
            NotifyOfPropertyChange(() => NameArea);
            NotifyOfPropertyChange(() => NameDevice);
            NotifyOfPropertyChange(() => Home);
            NotifyOfPropertyChange(() => Preset);
            NotifyOfPropertyChange(() => ControlTime);
            NotifyOfPropertyChange(() => Used);
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
        public string NameArea
        {
            get { return Model.NameArea; }
            set
            {
                Model.NameArea = value;
                NotifyOfPropertyChange(() => NameArea);
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

        public int Home
        {
            get { return Model.Home; }
            set
            {
                Model.Home = value;
                NotifyOfPropertyChange(() => Home);
            }
        }

        public int Preset
        {
            get { return Model.Preset; }
            set
            {
                Model.Preset = value;
                NotifyOfPropertyChange(() => Preset);
            }
        }

        public int ControlTime
        {
            get { return Model.ControlTime; }
            set
            {
                Model.ControlTime = value;
                NotifyOfPropertyChange(() => ControlTime);
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


        public PresetModel Model
        {
            get { return _model; }
            set
            { 
                _model = value; 
                NotifyOfPropertyChange(() => Model);
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

        #endregion
        #region - Attributes -
        private PresetModel _model;
        private bool _isSelected;
        public event EventHandler Notify;
        //public delegate void PropertyUpdateEvent(string Property);
        //public event PropertyUpdateEvent PropertyUpdate;
        #endregion
    }
}
