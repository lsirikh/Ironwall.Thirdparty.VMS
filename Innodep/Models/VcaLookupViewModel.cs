using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public class VcaLookupViewModel : IVcaLookupViewModel
    {
        public VcaViewModel VcaViewModel { get; set; }
        public PresetViewModel PresetViewModel { get; set; }
    }
}
