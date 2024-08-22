﻿namespace SupportDesk.Api
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
        }
    }
}
