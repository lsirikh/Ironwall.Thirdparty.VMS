using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class LoginResultModel : ILoginResultModel
    {
        #region - Implementations for ILoginResultModel -

        public string auth_token { get; set; }
        public int api_serial { get; set; }
        public int vms_id { get; set; }
        public int grp_serial { get; set; }
        public int user_serial { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public int utc { get; set; }
        #endregion
    }
}
