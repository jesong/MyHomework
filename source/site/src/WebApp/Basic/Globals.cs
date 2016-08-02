namespace MyHomework.WebApp.Basic
{
    public static class Globals
    {
        public const string AuthorizePolicySystemAdmin = "SystemAdmin";
        public const string AuthorizePolicyHomeworkPublisher = "HomeworkPublisher";
        public const string AuthorizePolicyMember = "Member";

        public static readonly string ResponseHeaderStrictTransportSecurityName = "Strict-Transport-Security";
        public static readonly string ResponseHeaderStrictTransportSecurityValue = "max-age=31536000; includeSubDomains";
    }
}
