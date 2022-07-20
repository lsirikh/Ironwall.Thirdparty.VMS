namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface IVcaModel
    {
        public int Id { get; set; }
        public int TypeDevice { get; set; }
        public string NameDevice { get; set; }
        public int SerialDevice { get; set; }
        public int Channel { get; set; }
        public int Status { get; set; }
        public bool Used { get; set; }
    }
}