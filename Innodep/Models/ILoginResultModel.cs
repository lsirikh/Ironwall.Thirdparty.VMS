namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface ILoginResultModel
    {
        public string auth_token { get; set; }
        public int api_serial { get; set; }
        public int vms_id { get; set; }
        public int grp_serial { get; set; }
        public int user_serial { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public int utc { get; set; }
    }
}