using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaLoginModel : IVcaLoginModel
    {
        #region - Implementations for IVcaLoginModel -
        public bool IsLogin { get; set; }
        public DateTime LoginTime { get; set; }
        public LoginResponseModel Response { get; set; }
        public CancellationTokenSource Cts { get; set; }
        #endregion
    }
}
