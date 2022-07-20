using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class TcpResultModel : ITcpResultModel
    {
        #region - Implementations for ITcpResultModel -
        public int cmd { get; set; }
        public int focus { get; set; }
        public int pan { get; set; }
        public int pan_spd { get; set; }
        public int preset_no { get; set; }
        public int tilt { get; set; }
        public int tilt_spd { get; set; }
        public double x1 { get; set; }
        public double x2 { get; set; }
        public double y1 { get; set; }
        public double y2 { get; set; }
        public int zoom { get; set; }
        #endregion
    }
}
