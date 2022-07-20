namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface IPresetModel
    {
        public int Id { get; set; }
        public string NameArea { get; set; }
        public string NameDevice { get; set; }
        public int Home { get; set; }
        public int Preset { get; set; }
        public int ControlTime { get; set; }
    }
}