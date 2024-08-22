namespace SupportDesk.Api
{
    public static class ApiEndpoints
    {
        private const string ApiBase = "api";

        public static class WeatherForecast
        {
            private const string Base = $"{ApiBase}/weatherforecast";

            public const string GetAll = Base;
        }

        public static class Auth
        {
            private const string Base = $"{ApiBase}/auth";

            public const string RefreshToken = $"{Base}/refreshToken";
        }

        public static class Requests
        {
            private const string Base = $"{ApiBase}/requests";

            public const string CreateRequest = Base;
            public const string UpdateRequest = Base;
            public const string InactivateRequest = Base;
            public const string ProcessRequest = $"{Base}/process";
            public const string GetRequestById = $"{Base}/{{id:int}}";
        }

        public static class Users
        {
            private const string Base = $"{ApiBase}/users";

            public const string GetMyRequests = $"{Base}/me/requests";
        }
    }
}
