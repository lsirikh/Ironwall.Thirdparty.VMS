using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class TcpResponseModel 
        : VmsResponseModel,ITcpResponseModel
    {
        
        #region - Implementations for ITcpResultModel -
        public TcpResultModel results { get; set; }
        #endregion


    }
}
