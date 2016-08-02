namespace MyHomework.WebApp.WeChat
{
    public enum UserType
    {
        Creator = 1,
        InternalSystemAdmin = 2,
        ExternalSystemAdmin = 3,
        Admin = 4,
        Member = 5
    }

    public class LoginUserInfo : UserInfo
    {
        public UserType UserType { get; set; }
    }
}