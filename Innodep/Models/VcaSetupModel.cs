using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaSetupModel
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        /// <summary>
        /// VCA
        /// </summary>
        public string TableVca => Properties.Settings.Default.TableVca;
        public string TablePreset => Properties.Settings.Default.TablePreset;

        //public string IpAddressVca => Properties.Settings.Default.IpAddressVca;
        //public int PortVca => Properties.Settings.Default.PortVca;
        //public string IdUserVca => Properties.Settings.Default.IdUserVca;
        //public string PasswordVca => Properties.Settings.Default.PasswordVca;
        //public string LicenceVca => Properties.Settings.Default.LicenceVca;
        //public string GroupVca => Properties.Settings.Default.GroupVca;

        public int TimeIntervalKeepAliveVca
        {
            get => Properties.Settings.Default.TimeIntevalKeepAliveVca;
            set
            {
                Properties.Settings.Default.TimeIntevalKeepAliveVca = value;
                Properties.Settings.Default.Save();
            }
        }
        
        public string TokenVca
        {
            get => Properties.Settings.Default.TokenVca;
            set
            {
                Properties.Settings.Default.TokenVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsLoginApplied
        {
            get => Properties.Settings.Default.IsLoginApplied;
            set
            { 
                Properties.Settings.Default.IsLoginApplied = value;
                Properties.Settings.Default.Save();
            }
        }

        public string IpAddressVca
        {
            get => Properties.Settings.Default.IpAddressVca;
            set
            {
                Properties.Settings.Default.IpAddressVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public int PortVca
        {
            get => Properties.Settings.Default.PortVca;
            set
            {
                Properties.Settings.Default.PortVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public string IdUserVca
        {
            get => Properties.Settings.Default.IdUserVca;
            set
            {
                Properties.Settings.Default.IdUserVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public string PasswordVca
        {
            get => Properties.Settings.Default.PasswordVca;
            set
            {
                Properties.Settings.Default.PasswordVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public string LicenceVca
        {
            get => Properties.Settings.Default.LicenceVca;
            set
            {
                Properties.Settings.Default.LicenceVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public string GroupVca
        {
            get => Properties.Settings.Default.GroupVca;
            set
            {
                Properties.Settings.Default.GroupVca = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool CancelSync
        {
            get => Properties.Settings.Default.CancelSync;
            set
            {
                Properties.Settings.Default.CancelSync = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ApiUsed
        {
            get => Properties.Settings.Default.ApiUsed;
            set
            {
                Properties.Settings.Default.ApiUsed = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
