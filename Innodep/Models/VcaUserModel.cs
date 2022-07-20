using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaUserModel : IVcaUserModel
    {
        #region - Implementation of Interface -
        public string IdUser { get; set; }          //1
        public string Password { get; set; }        //2
        public string Group { get; set; }           //3
        public string License { get; set; }         //4
        #endregion
    }
}
