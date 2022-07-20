using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class PresetModel : IPresetModel
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
        public int Id { get; set; }
        public string NameArea { get; set; }
        public string NameDevice { get; set; }
        public int Home { get; set; }
        public int Preset { get; set; }
        public int ControlTime { get; set; }
        public bool Used { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
