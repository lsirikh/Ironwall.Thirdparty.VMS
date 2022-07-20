using static Ironwall.Thirdparty.VMS.Innodep.TcpResponseModel;

namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface ITcpResponseModel
    {
        public TcpResultModel results { get; set; }
    }
}