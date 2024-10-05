using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Constants
{
    public static class ApiEndPointConstant
    {

        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
            public const string Login = AuthenticationEndpoint + "/login";
        }
        public static class User
        {
            public const string UsersEndPoint = ApiEndpoint + "/users";
            public const string UserEndPoint = UsersEndPoint + "/{id}";
            public const string UserLoginEndPoint = UsersEndPoint + "/login";
        }
        public static class EducationHistory
        {
            public const string EducationHistoryEndPoint = "education-history/{id}";
            public const string EducationHistoriesEndPoint = "education-histories";
        }
    }
}
