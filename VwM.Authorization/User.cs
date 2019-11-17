using System;

namespace VwM.Authorization
{
    public class User
    {
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; } = "User";
    }
}
