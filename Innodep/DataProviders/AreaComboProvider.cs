using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class AreaComboProvider
    : Screen
    {
        public AreaComboProvider()
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
