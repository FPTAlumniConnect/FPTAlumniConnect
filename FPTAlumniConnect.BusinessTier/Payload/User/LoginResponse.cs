using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public UserResponse UserInfo { get; set; }
    }

    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string? GoogleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

    }
}
