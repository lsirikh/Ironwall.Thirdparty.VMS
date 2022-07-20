namespace Ironwall.Thirdparty.VMS.Innodep
{
    public interface IVcaUserModel
    {
        public string IdUser { get; set; }          //1
        public string Password { get; set; }        //2
        public string Group { get; set; }           //3
        public string License { get; set; }         //4
    }
}