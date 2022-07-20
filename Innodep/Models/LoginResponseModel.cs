using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class LoginResponseModel
        : VmsResponseModel, ILoginResponseModel
    {
        #region - Implementations for ILoginResponseModel -
        public LoginResultModel results { get; set; }
        #endregion
    }
}
