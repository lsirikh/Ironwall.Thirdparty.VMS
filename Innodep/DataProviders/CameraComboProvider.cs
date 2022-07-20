using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class CameraComboProvider
        : Screen
    {
        public CameraComboProvider()
        {
            Provider = new ObservableCollection<string>();
        }

        private ObservableCollection<string> _provider;

        public ObservableCollection<string> Provider
        {
            get { return _provider; }
            set 
            { 
                _provider = value;
                NotifyOfPropertyChange(() => Provider);
            }
        }
        
    }
}
