namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface ITcpResultModel
    {
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
    }
}