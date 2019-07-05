# BridgeRpc
## 介紹
基於[MessagePack](https://msgpack.org/)格式以及參考[JsonRPC 2.0規格](https://www.jsonrpc.org/specification)的雙向RPC框架。
BridgeRpc的C#源自於我個人專案對於通訊框架的要求：雙向、持續性連線、簡單，因此我採用了WebSocket作為底層通訊(可抽換成其他可靠的通訊協定例如
TCP等)，而採用MessagePack作為資料交換格式的原因是MessagePack小而塊，在持續性連線中有其優勢。

BridgeRpc使用了[MessagePack for C#](https://github.com/neuecc/MessagePack-CSharp)函式庫來處理MessagePack資料。
## 通訊規範
### Request Object
一個請求包含以下成員：

| 欄位      | 說明                                                                                                                                    | 範例          |
|-----------|-----------------------------------------------------------------------------------------------------------------------------------------|---------------|
| bridgerpc | 指定版本的字串，必須是"1.0"                                                                                                             | "1.0"         |
| method    | 要呼叫的方法名稱。                                                                                                                      | "add"         |
| data      | 呼叫方法所需要的資料，可為空                                                                                                            | 2,3           |
| id        | 用來辨識此請求的字串，因此請求而回傳的Response Object必須包含一個id欄位且值與此欄位相符 若id為NULL，則此Request是一個通知(Notification) | "S20aix1jjh8" |

### Response Object
一個回應包含以下成員：

| 欄位      | 說明                                                                                                                                    | 範例          |
|-----------|-----------------------------------------------------------------------------------------------------------------------------------------|---------------|
| bridgerpc | 指定版本的字串，必須是"1.0"                                                                                                             | "1.0"         |
| result    | 此欄位在在呼叫成功時必須包含。 此欄位當呼叫失敗時必為空。 代表呼叫回傳的結果                                                            | 5             |
| error     | 此欄位在在呼叫成功時必為空。 此欄位當呼叫失敗時必須包含。 代表呼叫時發生錯誤，此欄位值定義為Error Object                                |               |
| id        | 用來辨識請求的字串 | "S20aix1jjh8" |

### Error Object
一個錯誤包含以下成員：

| 欄位    | 說明                       | 範例                                              |
|---------|----------------------------|---------------------------------------------------|
| code    | 一個整數用來表達此錯誤類型 | -10                                               |
| message | 對此錯誤一段描述的話       | "Wrong type of data."                             |
| data    | 有關此錯誤附加的資料       | "The first parameter is a string, not a integer." |

### Error Code
正整數的部分可供自行定義。

 - ParseError = -1
 - InvalidRequest = -2
 - MethodNotFound = -3
 - InvalidParams = -4
 - InternalError = -10
 
 ## 範例
 
 ### ASP.NET Core Client
 以下列出所有可設定的值以及其預設值：
 ```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddBridgeRpcClient((ref RpcClientOptions options) =>
    {
        options.Host = new Uri("ws://localhost/");
        options.ClientId = "client1";
        options.ReconnectInterval = TimeSpan.FromSeconds(60);
                 
        options.RpcOptions.AllowedOrigins = new List<string> {"*"};
        options.RpcOptions.BufferSize = 16 * 1024;
        options.RpcOptions.KeepAliveInterval = TimeSpan.FromSeconds(120);
        options.RpcOptions.RequestTimeout = TimeSpan.FromSeconds(15);
    });
}
```
在Program裡建立一個獨立的Scope並隨著應用程式啟動時發起連線：
```C#
public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateWebHostBuilder(args).Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            var client = new RpcIndependentScopeClient(serviceScope);

            try
            {
                client.OnConnected += async (hub, provider) =>
                {
                    while (true)
                    {
                        var r = await hub.RequestAsync("greet", MessagePackSerializer.Serialize("Joe"));
                        Console.WriteLine(r.GetResult<string>());
                        await Task.Delay(5000);
                    }
                };
                client.OnConnectFailed += e => Console.WriteLine("Cannot connect to the server");
                client.OnDisonnected += () => Console.WriteLine("Disconnected");
                client.Start();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred.");
            }
        }

        host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}
```
建立一個RpcController處裡來自Server的呼叫：
```C#
[RpcClient]
public class EntryController : RpcController
{
    [RpcMethod("sayHi")]
    public string SayHi([RpcParam("name")] string n)
    {
        Console.WriteLine("Hi from " + n);
        return n;
    }
}
```
### ASP.NET Core Server
 以下列出所有可設定的值以及其預設值：
````C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddBridgeRpc((ref RpcServerOptions options) =>
    {
        options.RoutingOptions.AllowAny = false;
        options.RoutingOptions.AllowedPaths = new List<RoutingPath>();
                
        options.RpcOptions.AllowedOrigins = new List<string> {"*"};
        options.RpcOptions.BufferSize = 16 * 1024;
        options.RpcOptions.KeepAliveInterval = TimeSpan.FromSeconds(120);
        options.RpcOptions.RequestTimeout = TimeSpan.FromSeconds(15);
    });
}
````
註冊連線相關的事件
````C#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseBridgeRpcWithBasic((ref ServerEventBus bus) =>
    {
        bus.OnConnected += (context, hub) =>
        {
            Console.WriteLine("Connected");
        };
        bus.OnNotAllowed += context => Console.WriteLine("Server not allowed this path: " + context.Request.Path);
        bus.OnDisconnected += _ => Console.WriteLine("Disconnected");
    });
}
````
建立一個RpcController處裡來自Client的呼叫：
```C#
[RpcRoute]
    public class EntryController : RpcController
    {
        private readonly IRpcHub _rpcHub;

        public EntryController(IRpcHub rpcHub)
        {
            _rpcHub = rpcHub;
        }

        [RpcMethod("greet")]
        public async Task<string> Greet(RpcRequest request)
        {
            var friend = request.GetData<string>();
            
            Console.WriteLine("Got greet from " + friend);

            var data = new Dictionary<string, string>();
            data.Add("name", "amy");

            RpcResponse reqTask = await _rpcHub.RequestAsync("sayHi",
                MessagePackSerializer.Serialize(data));
            Console.WriteLine("Amy says Hi, got reply, he says " + reqTask.GetResult<string>());

            return "Greeted";
        }
    }
```

### Attribute

#### [RpcClient(string clientId = null)]
套用於一個繼承自RpcController的類別。

在Client中用來表示此類別是一個RpcController且處理來自特定clientId連線的呼叫，若clientId為null則處理來自任意連線的呼叫。此attribute可以
重複套用。

#### [RpcRoute(string path = null)]
套用於一個繼承自RpcController的類別。

在Server中用來表示此類別是一個RpcController且處理來自特定path的呼叫，若path為null則處理來自任意連線的呼叫。此attribute可以重複套用。

#### [RpcMethod(string name = null)]
套用於一個Rpc Controller中的方法。

用來表示此方法是一個Rpc方法且其接受的方法名稱為attribute參數"name"。此attribute不可重複套用。

#### [RpcData(Type need = typeof(byte[]))]
套用於一個Rpc Method中的參數。

用來表示此參數來自於data欄位，並自動以MessagePack解析成Type need，若無指定則此參數會以原始的byte[]資料形式傳入。此attribute不可重複套用。

#### [RpcParam(string name = null)]
套用於一個Rpc Method中的參數。

用來表示此參數來自於data欄位中的資料其名稱為attribute參數"name"。此attribute不可重複套用。