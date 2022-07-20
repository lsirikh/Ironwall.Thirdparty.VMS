namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface IVmsResponseModel
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string detail { get; set; }
    }
}