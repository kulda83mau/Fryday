using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance = null;
    public CSteamID LobbyId { get; private set; }
    public ELobbyType lobbyType { get; private set; }

    public const ELobbyType DEFAULT_LOBBY_TYPE = ELobbyType.k_ELobbyTypePublic;
    public const string HostCSteamIDKey = "HostCSteamID";
    public const string GameKey = "GameID";
    public const string GameID = "Fryday";

    private NetworkManager networkManager;

    private List<CSteamID> lobbyIds = new List<CSteamID>();

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    protected Callback<LobbyMatchList_t> lobbyList;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;

    private bool steamIsInitialized;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        networkManager = GetComponent<NetworkManager>();

        if (!SteamIsInitialized()) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    private bool SteamIsInitialized()
    {
        SteamAPI.Init();
        return SteamManager.Initialized;
    }

    public void HostLobby()
    {
        lobbyType = DEFAULT_LOBBY_TYPE;
        SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
    }

    public void JoinLobby(CSteamID steamID) => SteamMatchmaking.JoinLobby(steamID);

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby was not created!");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();
        networkManager.ServerChangeScene("GameScene");

        SteamMatchmaking.SetLobbyData(LobbyId, HostCSteamIDKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(LobbyId, GameKey, GameID);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) => SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        CSteamID lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        LobbyId = lobbyId;

        string hostAddress = SteamMatchmaking.GetLobbyData(LobbyId, HostCSteamIDKey);
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }

    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(LobbyId);
    }

    public void GetLobbiesList()
    {
        if (lobbyIds.Count > 0) lobbyIds.Clear();
        SteamMatchmaking.AddRequestLobbyListStringFilter(GameKey, GameID, ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIds.Add(lobbyId);
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        
    }
}