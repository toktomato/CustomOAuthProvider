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
    public class SecurityToken
    {
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string IssuedAt { get; set; }
        public string NotBefore { get; set; }
        public string ExpiredAt { get; set; }
        public string Name { get; set; }
        public string PreferredUsername { get; set; }
        public Roles Roles { get; set; }
    }

    public class Roles
    {
        public string Name { get; set; }
        public string PreferredUsername { get; set; }
        public string UserType { get; set; }
        public string Dept { get; set; }
        public string AdminDept { get; set; }
    }
}
