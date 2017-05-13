This is a client for the screeps API, inspired by [screepers/python-screeps](https://github.com/screepers/python-screeps) (thank you for the great [documentation](https://github.com/screepers/python-screeps/blob/master/docs/Endpoints.md)!).  
The console application (ScreepsDemoConsole project) is a sample consumer on how to use the client (ScreepsApi project). 

Each method will return a [dynamic object](https://docs.microsoft.com/en-us/dotnet/articles/csharp/language-reference/keywords/dynamic), having just those properties which the json response from screeps server API contains. You will need to know yourself which properties are present and which aren't, because there are no Data Contracts defined anywhere for Serialization. But you can access present properties as if you were using a Data Contract.

Like this: 
```C#
Client client = new Client(email, password);
dynamic me = client.Me();
Console.Write("User {0}, GCL: {1}", me.username, me.gcl);
```
This way, the client is resistant to future changes to screeps API response.  
For your convenience, there is an inline documentation of how current responses are structured (according to documentation from screepers/python-screeps). It should also give you intellisense hints.
```C#
/// <summary>
/// Information about logged in user
/// </summary>
/// <returns>{ ok, _id, email, username, cpu, badge: { type, color1, color2, color3, param, flip }, password, notifyPrefs: { sendOnline, errorsInterval, disabledOnMessages, disabled, interval }, gcl, credits, lastChargeTime, lastTweetTime, github: { id, username }, twitter: { username, followers_count } }</returns>
public dynamic Me()
```
