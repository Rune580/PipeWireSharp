using System.Runtime.InteropServices;
using PipeWireSharp.Tests.DBus;
using Tmds.DBus.Protocol;

namespace PipeWireSharp.Tests.ScreenCast;

public class ScreenCastSession : IDisposable
{
    private readonly Connection _connection;
    private readonly DBus.ScreenCast _screencast;
    private readonly Session _session;
    
    private ScreenCastSession(Connection connection, DBus.ScreenCast screencast, Session session)
    {
        _connection = connection;
        _screencast = screencast;
        _session = session;
    }

    public async Task<ScreenCastStream[]> StartAsync()
    {
        var selectSourcesOptions = new Dictionary<string, VariantValue>
        {
            { "handle_token", GenerateHandleToken() },
            { "types", (uint)(1 | 2 | 4) },
            { "multiple", false },
            { "cursor_mode", (uint)2 },
        };
        await _screencast.SelectSourcesAsync(_session.Path, selectSourcesOptions);
        
        var startOptions = new Dictionary<string, VariantValue>
        {
            { "handle_token", GenerateHandleToken() }
        };
        var startResponse = await PortalResponse.WaitAsync(_connection, () => _screencast.StartAsync(_session.Path, "", startOptions));

        var streamDataArray = startResponse.Results["streams"].GetArray<VariantValue>();

        var streams = new ScreenCastStream[streamDataArray.Length];

        for (int i = 0; i < streamDataArray.Length; i++)
        {
            streams[i] = new ScreenCastStream
            {
                PipeWireNodeId = streamDataArray[i].GetItem(0).GetUInt32(),
            };
        }
        
        return streams;
    }

    public async Task<SafeHandle> OpenPipeWireRemoteAsync()
    {
        var pipewireRemoteOptions = new Dictionary<string, VariantValue>();
        var pipewireRemote = await _screencast.OpenPipeWireRemoteAsync(_session.Path, pipewireRemoteOptions);

        if (pipewireRemote is null)
            throw new InvalidOperationException("Failed to open PipeWire remote!");
        
        return pipewireRemote;
    }

    public static async Task<ScreenCastSession> CreateAsync()
    {
        var connection = new Connection(Address.Session!);
        await connection.ConnectAsync();
        var desktop = new DesktopService(connection, "org.freedesktop.portal.Desktop");

        var screencast = desktop.CreateScreenCast("/org/freedesktop/portal/desktop");

        var requestHandleToken = GenerateHandleToken();
        var sessionHandleToken = GenerateHandleToken();

        var createSessionOptions = new Dictionary<string, VariantValue>
        {
            ["handle_token"] = requestHandleToken,
            ["session_handle_token"] = sessionHandleToken
        };
        var response = await PortalResponse.WaitAsync(connection, () => screencast.CreateSessionAsync(createSessionOptions));
        
        if (!response.Results.TryGetValue("session_handle", out var sessionHandle))
            throw new Exception("Failed to create ScreenCast session");

        var session = new Session(desktop, sessionHandle.GetString());
        
        return new ScreenCastSession(connection, screencast, session);
    }
    
    private static string GenerateHandleToken()
    {
        var guid = Guid.NewGuid().ToString();
        return $"pipewiresharp_tests_{guid.Replace("-", "")}";
    }

    public async Task StopAsync()
    {
        await _session.CloseAsync();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}