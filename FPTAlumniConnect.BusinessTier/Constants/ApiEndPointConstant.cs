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
        public static class Post
        {
            public const string PostsEndPoint = ApiEndpoint + "/posts";
            public const string PostEndPoint = PostsEndPoint + "/{id}";
        }
        public static class PostReport
        {
            public const string PostReportsEndPoint = ApiEndpoint + "/reports";
            public const string PostReportEndPoint = PostReportsEndPoint + "/{id}";
        }
        public static class Comment
        {
            public const string CommentsEndPoint = ApiEndpoint + "/comments";
            public const string CommentEndPoint = CommentsEndPoint + "/{id}";
        }
        public static class Event
        {
            public const string EventsEndPoint = ApiEndpoint + "/events"; 
            public const string EventEndPoint = EventsEndPoint + "/{id}";
        }
        public static class UserJoinEvent
        {
            public const string UserJoinEventsEndPoint = ApiEndpoint + "/user-join-events";
            public const string UserJoinEventEndPoint = UserJoinEventsEndPoint + "/{id}";
            public const string ViewAllUserJoinEventsEndPoint = UserJoinEventsEndPoint + "/view-all";
        }
        public static class EducationHistory
        {
            public const string EducationHistoryEndPoint = "education-history/{id}";
            public const string EducationHistoriesEndPoint = "education-histories";
        }
        public static class PrivacySetting
        {
            public const string PrivacySettingEndPoint = "privacy-setting/{id}";
            public const string PrivacySettingsEndPoint = "privacy-settings";
        }
        public static class NotificationSetting
        {
            public const string NotificationSettingEndPoint = "notification-setting/{id}";
            public const string NotificationSettingsEndPoint = "notification-settings";
        }
        public static class SocialLink
        {
            public const string SocialLinkEndPoint = "social-link/{id}";
            public const string SocialLinksEndPoint = "social-links";
        }
    }
}
