using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

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
            http.Deserializer = Deserialize;
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

        private dynamic Deserialize(string response)
        {
            return js.Deserialize(response);
        }

        private byte[] Base64Decode(string encoded)
        {
            return Convert.FromBase64String(encoded);
        }

        private string UnGZip(byte[] zipped)
        {
            string result = null;
            using (MemoryStream zipMemory = new MemoryStream(zipped))
            using (GZipStream zipper = new GZipStream(zipMemory, CompressionMode.Decompress))
            using (MemoryStream unzipped = new MemoryStream())
            {
                const int BUFFER = 4096;
                byte[] buffer = new byte[BUFFER];
                int size;
                do{
                    size = zipper.Read(buffer, 0, BUFFER);
                    unzipped.Write(buffer, 0, size);
                } while (size > 0);
                result = Encoding.UTF8.GetString(unzipped.ToArray());
            }
            return result;
        }


        /// <summary>
        /// Log in to screeps API
        /// </summary>
        /// <param name="email">account email</param>
        /// <param name="password">account password</param>
        /// <returns>{ok, token}</returns>
        private dynamic Connect(string email, string password)
        {
            return http.Post(baseUrl, Path.CONNECT, new 
            {
                email = email, 
                password = password
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <param name="interval"></param>
        /// <returns>{ ok, owner: { username, badge: { type, color1, color2, color3, param, flip } }, stats: { energyHarvested: [ { value, endTime } ], energyConstruction: [ { value, endTime } ], energyCreeps: [ { value, endTime } ], energyControl: [ { value, endTime } ], creepsProduced: [ { value, endTime } ], creepsLost: [ { value, endTime } ] }, statsMax: { energy1440, energyCreeps1440, energy8, energyControl8, creepsLost180, energyHarvested8, energy180, energyConstruction180, creepsProduced8, energyControl1440, energyCreeps8, energyHarvested1440, creepsLost1440, energyConstruction1440, energyHarvested180, creepsProduced180, creepsProduced1440, energyCreeps180, energyControl180, energyConstruction8, creepsLost8 } }</returns>
        public dynamic RoomOverview(string room, int interval = 10)
        {
            return http.Get(baseUrl, Path.ROOM_OVERVIEW,
                new UrlParam("interval", interval),
                new UrlParam("room", room));
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

            return http.Get(baseUrl, Path.ROOM_TERRAIN, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns>{ _id, status, novice }</returns>
        public dynamic RoomStatus(string room)
        {
            return http.Get(baseUrl, Path.ROOM_STATUS,
                new UrlParam("room", room));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns>{ ok, time, rooms: [ { _id, lastPvpTime } ] }</returns>
        public dynamic Pvp(int interval = 50)
        {
            return http.Get(baseUrl, Path.PVP,
                new UrlParam("interval", interval));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns>{ ok, time, rooms: [ { _id, lastPvpTime } ] }</returns>
        public dynamic PvpSince(int start)
        {
            return http.Get(baseUrl, Path.PVP,
                new UrlParam("start", start));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{"ok":1,"list":[{"_id":"XUHO2","count":2}]}</returns>
        public dynamic OrdersIndex()
        {
            return http.Get(baseUrl, Path.ORDERS_INDEX);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, list: [ { _id, created, user, active, type, amount, remainingAmount, resourceType, price, totalAmount, roomName } ] }</returns>
        public dynamic OrdersMy()
        {
            return http.Get(baseUrl, Path.ORDERS_MY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType">One of the RESOURCE_* constants</param>
        /// <returns>{ ok, list: [ { _id, created, user, active, type, amount, remainingAmount, resourceType, price, totalAmount, roomName } ] }</returns>
        public dynamic Orders(string resourceType)
        {
            return http.Get(baseUrl, Path.ORDERS,
                new UrlParam("resourceType", resourceType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{"ok":1,"page":0,"list":[ { _id, date, tick, user, type, balance, change, market: {} } ] }</returns>
        public dynamic MoneyHistory()
        {
            return http.Get(baseUrl, Path.MONEY_HIST);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, seasons: [ { _id, name, date } ] }</returns>
        public dynamic LeaderboardSeasons()
        {
            return http.Get(baseUrl, Path.LEADERBOARD_SEASONS);
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

            return http.Get(baseUrl, Path.LEADERBOARD_FIND, args);
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
            return http.Get(baseUrl, Path.LEADERBOARD_LIST,
                new UrlParam("season", season),
                new UrlParam("limit", limit),
                new UrlParam("offset", offset),
                new UrlParam("mode", mode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, messages: [ { _id, message: { _id, user, respondent, date, type, text, unread } } ], users: { "user's _id": { _id, username, badge: { type, color1, color2, color3, param, flip } } } }</returns>
        public dynamic MessagesIndex()
        {
            return http.Get(baseUrl, Path.MESSAGES_INDEX);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="respondent"></param>
        /// <returns>{ ok, messages: [ { _id, date, type, text, unread } ] }</returns>
        public dynamic MessagesList(string respondent)
        {
            return http.Get(baseUrl, Path.MESSAGES_LIST,
                new UrlParam("respondent", respondent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="respondent">the long _id of the user, not the username</param>
        /// <param name="text"></param>
        /// <returns>{ ok }</returns>
        public dynamic MessagesSend(string respondent, string text)
        {
            return http.Post(baseUrl, Path.MESSAGES_SEND, new
            {
                respondent, 
                text
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, count }</returns>
        public dynamic MessagesUnreadCount()
        {
            return http.Get(baseUrl, Path.MESSAGES_UNREAD);
        }

        /// <summary>
        /// Information about logged in user
        /// </summary>
        /// <returns>{ ok, _id, email, username, cpu, badge: { type, color1, color2, color3, param, flip }, password, notifyPrefs: { sendOnline, errorsInterval, disabledOnMessages, disabled, interval }, gcl, credits, lastChargeTime, lastTweetTime, github: { id, username }, twitter: { username, followers_count } }</returns>
        public dynamic Me()
        {
            return http.Get(baseUrl, Path.ME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns>{ ok, user: { _id, username, badge: { type, color1, color2, color3, param, flip }, gcl } }</returns>
        public dynamic UserFind(string username)
        {
            return http.Get(baseUrl, Path.USER_FIND,
                new UrlParam("username", username));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>{ ok, user: { _id, username, badge: { type, color1, color2, color3, param, flip }, gcl } }</returns>
        public dynamic UserFindById(string id)
        {
            return http.Get(baseUrl, Path.USER_FIND,
                new UrlParam("id", id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="interval"></param>
        /// <returns>{ ok, rooms: [ "room name" ], stats: { "room name": [ { value, endTime } ] }, statsMax }</returns>
        public dynamic UserOverview(string statName = "energyControl", int interval = 1440)
        {
            return http.Get(baseUrl, Path.USER_OVERVIEW,
                new UrlParam("statName", statName),
                new UrlParam("interval", interval));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, rooms: [ "room name" ] }</returns>
        public dynamic UserRespawnProhibitedRooms()
        {
            return http.Get(baseUrl, Path.USER_PROHIBITED_ROOMS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, status }</returns>
        public dynamic UserWorldStatus()
        {
            return http.Get(baseUrl, Path.USER_STATUS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, room: [ "room name" ] }</returns>
        public dynamic UserWorldStartRoom()
        {
            return http.Get(baseUrl, Path.USER_START);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, active }</returns>
        public dynamic UserSubscriptionTime()
        {
            return http.Get(baseUrl, Path.USER_SUBSCRIPTION);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>{ ok, result: { ok, n }, ops: [ { user, expression, _id } ], insertedCount, insertedIds: [ "mongodb id" ] }</returns>
        public dynamic UserConsole(string expression)
        {
            return http.Post(baseUrl, Path.USER_CONSOLE, new
            {
                expression
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns>{ ok, data } data is the string gz: followed by base64-encoded gzipped JSON encoding of the requested memory path</returns>
        public dynamic UserMemoryGet(string path)
        {
            dynamic memory = http.Get(baseUrl, Path.USER_MEMORY,
                new UrlParam("path", path));
            if (memory.data == null) return memory;
            string encoded = memory.data;
            if (!string.IsNullOrWhiteSpace(encoded) && encoded.StartsWith("gz:"))
            {
                memory.Set("data", js.Deserialize(UnGZip(Base64Decode(encoded.Substring(3)))));
            }
            return memory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns>{ ok, result: { ok, n }, ops: [ { user, expression, hidden } ], data, insertedCount, insertedIds }</returns>
        public dynamic UserMemorySet(string path, string value)
        {
            return http.Post(baseUrl, Path.USER_MEMORY, new
            {
                path, 
                value
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns>{ ok, data: '' }</returns>
        public dynamic UserMemorySegmentGet(int segment)
        {
            return http.Get(baseUrl, Path.USER_MEMORY_SEGMENT,
                new UrlParam("segment", segment));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public dynamic UserMemorySet(int segment, string data)
        {
            return http.Post(baseUrl, Path.USER_MEMORY_SEGMENT, new
            {
                segment,
                data
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">can be at least "flag" or "spawn"</param>
        /// <returns>{ ok, name }</returns>
        public dynamic GenerateUniqueObjectName(string type)
        {
            return http.Post(baseUrl, Path.UNIQUE_NAME, new
            {
                type
            });
        }

        /// <summary>
        /// if the name is new, result.upserted[0]._id is the game id of the created flag. f not, this moves the flag and the response does not contain the id (but the id doesn't change).
        /// </summary>
        /// <param name="room"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="secondaryColor"></param>
        /// <returns>{ ok, result: { nModified, ok, upserted: [ { index, _id } ], n }, connection: { host, id, port } }</returns>
        public dynamic FlagCreate(string room, int x, int y, string name, string color, string secondaryColor)
        {
            return http.Post(baseUrl, Path.FLAG_CREATE, new
            {
                room,
                x,
                y,
                name,
                color,
                secondaryColor
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>{ ok, result: { nModified, ok, n }, connection: { host, id, port } }</returns>
        public dynamic FlagChange(string id, string room, int x, int y)
        {
            return http.Post(baseUrl, Path.FLAG_CHANGE, new
            {
                _id = id,
                room,
                x,
                y
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="color"></param>
        /// <param name="secondaryColor"></param>
        /// <returns>{ ok, result: { nModified, ok, n }, connection: { host, id, port } }</returns>
        public dynamic FlagChangeColor(string id, string color, string secondaryColor)
        {
            return http.Post(baseUrl, Path.FLAG_CHANGE_COLOR, new
            {
                _id = id,
                color,
                secondaryColor
            });
        }

        /// <summary>
        /// remove flag: name = "remove", intent = {}; 
        /// destroy structure: _id = "room", name = "destroyStructure", intent = [ {id: "structure id", roomName, "room name", user: "user id"} ]; 
        /// suicide creep: name = "suicide", intent = {id: "creep id"}; 
        /// unclaim controller: name = "unclaim", intent = {id: "controller id"}
        /// remove construction site: name = "remove", intent = {}
        /// </summary>
        /// <param name="id">game id of the object to affect (except for destroying structures)</param>
        /// <param name="room">name of the room it's in</param>
        /// <param name="name">intent name</param>
        /// <param name="intent">intent details</param>
        /// <returns>{ ok, result: { nModified, ok, upserted: [ { index, _id } ], n }, connection: { host, id, port } }</returns>
        public dynamic AddObjectIntent(string id, string room, string name, string intent)
        {
            return http.Post(baseUrl, Path.INTENT, new
            {
                _id = id,
                room,
                name,
                intent
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns>{ ok, result: { ok, nModified, n }, connection: { id, host, port } }</returns>
        public dynamic SetNotifyWhenAttacked(string id, bool enabled)
        {
            return http.Post(baseUrl, Path.NOTIFY_ATTACK, new
            {
                _id = id,
                enabled
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <param name="structureType">one of the in-game STRUCTURE_* constants</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>{ ok, result: { ok, n }, ops: [ { type, room, x, y, structureType, user, progress, progressTotal, _id } ], insertedCount, insertedIds }</returns>
        public dynamic CreateConstruction(string room, string structureType, int x, int y)
        {
            return http.Post(baseUrl, Path.CREATE_CONSTRUCTION, new
            {
                room,
                structureType,
                x,
                y
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ ok, time }</returns>
        public dynamic Time()
        {
            return http.Get(baseUrl, Path.TIME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branch"></param>
        /// <returns>{ ok, branch, modules: {"module name": "code"} }</returns>
        public dynamic CodeGet(string branch)
        {
            return http.Get(baseUrl, Path.CODE,
                new UrlParam("branch", branch));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branch"></param>
        /// <param name="modules"></param>
        /// <returns>{ ok, timestamp }</returns>
        public dynamic CodeSet(string branch, Dictionary<string, string> modules)
        {
            return http.Post(baseUrl, Path.CODE, new
            {
                branch, 
                modules
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="statName">can be "owner0", "claim0", or a stat</param>
        /// <returns>{ ok, stats: { "room name": { status, novice, own: { user, level }, "stat": [ { user, value } ] } }, users: { "user's _id": { _id, username, badge: { type, color1, color2, color3, param, flip } } } }</returns>
        public dynamic CodeSet(string[] rooms, string statName)
        {
            return http.Post(baseUrl, Path.MAP_STATS, new
            {
                rooms,
                statName
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <param name="sinceTick">must be a multiple of 20; the response includes information for the 20 ticks starting then</param>
        /// <returns>{ timestamp, room, base, ticks: { "time": "tick update object" } }</returns>
        public dynamic RoomHistory(string room, int sinceTick)
        {
            string path = string.Format("{0}/{1}/{2}.json", Path.ROOM_HISTORY, room, sinceTick);
            return http.Get(Path.HOST, path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ok, result: { ok, nModified, n }, connection: { id, host, port } }</returns>
        public dynamic ActivatePtr()
        {
            return http.Post(Path.HOST, Path.ACTIVATE_PTR);
        }
    }
}
