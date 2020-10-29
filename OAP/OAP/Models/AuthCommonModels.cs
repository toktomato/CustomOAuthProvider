namespace OAP.Models
{
    public class AuthIncomingDataModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class AuthOutgoingDataModel
    {
        public string IDToken { get; set; }
    }

    public class Roles
    {
        public string Name { get; set; }
        public string AdminDept { get; set; }
    }
}
