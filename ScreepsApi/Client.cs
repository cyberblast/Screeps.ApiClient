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

        /// <summary>
        /// Log in to screeps API
        /// </summary>
        /// <param name="email">account email</param>
        /// <param name="password">account password</param>
        /// <returns>{ok, token}</returns>
        private dynamic Connect(string email, string password)
        {
            return js.Deserialize(
                http.Post(baseUrl, Path.CONNECT, new 
                {
                    email = email, 
                    password = password
                }));
        }

        /// <summary>
        /// Information about logged in user
        /// </summary>
        /// <returns>{ ok, _id, email, username, cpu, badge: { type, color1, color2, color3, param, flip }, password, notifyPrefs: { sendOnline, errorsInterval, disabledOnMessages, disabled, interval }, gcl, credits, lastChargeTime, lastTweetTime, github: { id, username }, twitter: { username, followers_count } }</returns>
        public dynamic Me()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ME));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <param name="interval"></param>
        /// <returns>{ ok, owner: { username, badge: { type, color1, color2, color3, param, flip } }, stats: { energyHarvested: [ { value, endTime } ], energyConstruction: [ { value, endTime } ], energyCreeps: [ { value, endTime } ], energyControl: [ { value, endTime } ], creepsProduced: [ { value, endTime } ], creepsLost: [ { value, endTime } ] }, statsMax: { energy1440, energyCreeps1440, energy8, energyControl8, creepsLost180, energyHarvested8, energy180, energyConstruction180, creepsProduced8, energyControl1440, energyCreeps8, energyHarvested1440, creepsLost1440, energyConstruction1440, energyHarvested180, creepsProduced180, creepsProduced1440, energyCreeps180, energyControl180, energyConstruction8, creepsLost8 } }</returns>
        public dynamic RoomOverview(string room, int interval = 10)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ROOM_OVERVIEW,
                    new UrlParam("interval", interval),
                    new UrlParam("room", room)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <param name="encoded"></param>
        /// <returns>unencoded: { ok, terrain: [ { room, x, y, type } ] }, encoded: { ok, terrain: [ { _id, room, terrain, type } ] }</returns>
        public dynamic RoomTerrain(string room, bool encoded = false)
        {
            UrlParam[] args = new UrlParam[encoded ? 1 : 2];
            args[0] = new UrlParam("room", room);
            if (encoded) args[1] = new UrlParam("encoded", 1);

            return js.Deserialize(
                http.Get(baseUrl, Path.ROOM_TERRAIN, args));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns>{ _id, status, novice }</returns>
        public dynamic RoomStatus(string room)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ROOM_STATUS,
                    new UrlParam("room", room)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns>{ ok, time, rooms: [ { _id, lastPvpTime } ] }</returns>
        public dynamic Pvp(int interval = 50)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.PVP,
                    new UrlParam("interval", interval)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns>{ ok, time, rooms: [ { _id, lastPvpTime } ] }</returns>
        public dynamic PvpSince(int start)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.PVP,
                    new UrlParam("start", start)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{"ok":1,"list":[{"_id":"XUHO2","count":2}]}</returns>
        public dynamic OrdersIndex()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ORDERS_INDEX));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, list: [ { _id, created, user, active, type, amount, remainingAmount, resourceType, price, totalAmount, roomName } ] }</returns>
        public dynamic OrdersMy()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ORDERS_MY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType">One of the RESOURCE_* constants</param>
        /// <returns>{ ok, list: [ { _id, created, user, active, type, amount, remainingAmount, resourceType, price, totalAmount, roomName } ] }</returns>
        public dynamic Orders(string resourceType)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.ORDERS,
                    new UrlParam("resourceType", resourceType)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{"ok":1,"page":0,"list":[ { _id, date, tick, user, type, balance, change, market: {} } ] }</returns>
        public dynamic MoneyHistory()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.MONEY_HIST));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, seasons: [ { _id, name, date } ] }</returns>
        public dynamic LeaderboardSeasons()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.LEADERBOARD_SEASONS));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="season">like "2015-09" or null to list ranks in all seasons</param>
        /// <param name="mode"></param>
        /// <returns>{ ok, _id, season, user, score, rank } or { ok, list: [ _id, season, user, score, rank ] } (all seasons)</returns>
        public dynamic LeaderboardFind(string username, string season = null, string mode = "world")
        {
            UrlParam[] args = new UrlParam[string.IsNullOrEmpty(season) ? 2 : 3];
            args[0] = new UrlParam("username", username);
            args[1] = new UrlParam("mode", mode);
            if (!string.IsNullOrEmpty(season)) args[2] = new UrlParam("season", season);

            return js.Deserialize(
                http.Get(baseUrl, Path.LEADERBOARD_FIND, args));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="season"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="mode"></param>
        /// <returns>{ ok, list: [ { _id, season, user, score, rank } ], count, users: { "user's _id": { _id, username, badge: { type, color1, color2, color3, param, flip }, gcl } } }</returns>
        public dynamic LeaderboardList(string season, int limit, int offset, string mode = "world")
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.LEADERBOARD_LIST,
                    new UrlParam("season", season),
                    new UrlParam("limit", limit),
                    new UrlParam("offset", offset),
                    new UrlParam("mode", mode)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, messages: [ { _id, message: { _id, user, respondent, date, type, text, unread } } ], users: { "user's _id": { _id, username, badge: { type, color1, color2, color3, param, flip } } } }</returns>
        public dynamic MessagesIndex()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.MESSAGES_INDEX));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="respondent"></param>
        /// <returns>{ ok, messages: [ { _id, date, type, text, unread } ] }</returns>
        public dynamic MessagesList(string respondent)
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.MESSAGES_LIST,
                    new UrlParam("respondent", respondent)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="respondent">the long _id of the user, not the username</param>
        /// <param name="text"></param>
        /// <returns>{ ok }</returns>
        public dynamic MessagesSend(string respondent, string text)
        {
            return js.Deserialize(
                http.Post(baseUrl, Path.MESSAGES_SEND, new
                {
                    respondent, 
                    text
                }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, count }</returns>
        public dynamic MessagesUnreadCount()
        {
            return js.Deserialize(
                http.Get(baseUrl, Path.MESSAGES_UNREAD));
        }

    }
}
