using System;
using System.Threading;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface IVcaLoginModel
    {
        public bool IsLogin { get; set; }
        public DateTime LoginTime { get; set; }
        public LoginResponseModel Response { get; set; }
        public CancellationTokenSource Cts { get; set; } 
    }
}