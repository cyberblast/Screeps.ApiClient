using System;

namespace ScreepsApi
{
    public class Client
    {
        private string token;
        public Json js;
        private Http http;
        private string baseUrl;

        public Client(string email, string password, bool ptr = false)
        {
            js = new Json();
            http = new Http();
            baseUrl = ptr ? Path.PTR : Path.SERVER;

            var signIn = Connect(email, password);
            if (signIn.ok == 1)
            {
                token = signIn.token;
                http.SetHeader("X-Token", token);
                http.SetHeader("X-Username", token);
                http.OnCompleted += (response) => http.UpdateHeader("X-Token");
            }
            else
            {
                throw new UnauthorizedAccessException("Login failed!");
            }
        }

        private dynamic Connect(string email, string password)
        {
            return js.Deserialize(
                http.Post(baseUrl, Path.CONNECT, js.Serialize(new 
                {
                    email = email, 
                    password = password
                })));
        }

        public dynamic Me()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ME));
        }

        public dynamic RoomOverview(string room, int interval = 10)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ROOM_OVERVIEW,
                    new UrlParam("interval", interval),
                    new UrlParam("room", room)));
        }

        public dynamic RoomTerrain(string room, bool encoded = false)
        {
            UrlParam[] args = new UrlParam[encoded ? 1 : 2];
            args[0] = new UrlParam("room", room);
            if (encoded) args[1] = new UrlParam("encoded", 1);

            return js.Deserialize(
                http.Get(baseUrl, Path.ROOM_TERRAIN, args));
        }

        public dynamic RoomStatus(string room)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ROOM_STATUS,
                    new UrlParam("room", room)));
        }

        public dynamic Pvp(int interval = 50)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.PVP,
                    new UrlParam("interval", interval)));
        }

        public dynamic PvpSince(int start)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.PVP,
                    new UrlParam("start", start)));
        }

    }
}
