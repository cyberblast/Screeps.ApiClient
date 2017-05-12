using System;

namespace ScreepsApi
{
    public class Client
    {
        const string SERVER_URL = "https://screeps.com/api";

        private string token;
        private Json js;
        private Http http;

        public Client(string email, string password)
        {
            js = new Json();
            http = new Http();
            var signin = Connect(email, password);
            if (signin.ok == 1)
            {
                token = signin.token;
                http.SetHeader("X-Token", token);
                http.SetHeader("X-Username", token);
                http.OnCompleted += (response) =>
                {
                    var newToken = http.UpdateHeader("X-Token");
                };
            }
            else
            {
                throw new UnauthorizedAccessException("Login failed!");
            }
        }

        private dynamic Connect(string email, string password)
        {
            return js.Deserialize(
                http.Post(SERVER_URL, "/auth/signin", js.Serialize(new 
                {
                    email = email, 
                    password = password
                })));
        }

        public dynamic Me()
        {
            return js.Deserialize(
                http.Get(SERVER_URL, "/auth/me"));
        }
    }
}
