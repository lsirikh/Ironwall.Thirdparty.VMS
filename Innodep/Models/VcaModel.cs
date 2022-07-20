using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaModel : IVcaModel
    {
        #region - Implementations for IVcaModel -
        public int Id { get; set; }
        public int TypeDevice { get; set; }
        public string NameDevice { get; set; }
        public int SerialDevice { get; set; }
        public int Channel { get; set; }
        public int Status { get; set; }
        public bool Used { get; set; }
        #endregion
    }
}
