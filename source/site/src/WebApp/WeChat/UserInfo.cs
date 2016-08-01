namespace MyHomework.WebApp.WeChat
{
    public enum UserStatus
    {
        Followed = 1,
        Forbiden = 2,
        NotFollowed = 3
    }
    public class UserInfo
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public int[] DepartmentIds { get; set; }

        public UserStatus Status { get; set; }
    }
}
