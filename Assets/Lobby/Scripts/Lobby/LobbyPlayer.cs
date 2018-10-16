using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        public GameObject[] Models;
        static string[] ModelNames = null;
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();
        static List<int> _modelInUse = new List<int>();

        public Button colorButton;
        public Button rightButton;
        public Button leftButton;
        public RawImage modelImage;
        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;

        public GameObject localIcone;
        public GameObject remoteIcone;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;
        [SyncVar(hook = "OnMyModel")]
        public string playerModelName = "";


        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f/255.0f, 0.0f, 101.0f/255.0f,1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);


        private Camera viewModelCamera;
        private LobbyManager lobbyManager;

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            viewModelCamera = this.transform.GetComponentInChildren<Camera>();
            //  RenderTexture
            RenderTexture targetTexture = new RenderTexture(viewModelCamera.targetTexture);
            targetTexture.name = "texture";
            viewModelCamera.targetTexture = targetTexture;
            modelImage.texture = targetTexture;

            if (ModelNames == null)
            {
                ModelNames = new string[Models.Length];
                for (int i = 0; i < ModelNames.Length; ++i)
                {
                    ModelNames[i] = Models[i].gameObject.name;
                }
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
            OnMyModel(playerModelName);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;
            leftButton.interactable = false;
            rightButton.interactable = false;

            removePlayerButton.interactable = NetworkServer.active;

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

            CheckRemoveButton();

            //if (playerColor == Color.white)
            //    CmdColorChange();

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount-1));

            //we switch from simple name display to name input
            colorButton.interactable = true;

            rightButton.interactable = true;  // TODO: Bug here that it doesn't set to false
            leftButton.interactable = true;

            if (playerModelName == "")
            {
                CmdNextModelChange(true);
            }

            nameInput.interactable = true;
            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            //colorButton.onClick.RemoveAllListeners();
           // colorButton.onClick.AddListener(OnColorClicked);

            rightButton.onClick.RemoveAllListeners();
            rightButton.onClick.AddListener(()=> {OnNextModelClicked(true);});

            leftButton.onClick.RemoveAllListeners();
            leftButton.onClick.AddListener(() => {OnNextModelClicked(false);});

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            
            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                rightButton.interactable = false;
                leftButton.interactable = false;
                nameInput.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                rightButton.interactable = isLocalPlayer;
                leftButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
        }

        public void OnMyModel(string modelName)
        {
            playerModelName = modelName;
            int modelIdx = System.Array.IndexOf(ModelNames, playerModelName);
            for (int i = 0; i < Models.Length; ++i)
            {
                if (i == modelIdx)       Models[i].SetActive(true);
                else                     Models[i].SetActive(false);
            }
        }

        //===== UI Handler

        public void OnNextModelClicked(bool next)
        {
            if (isLocalPlayer)
            {
                CmdNextModelChange(next);
            }
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        //[Command]
        //public void CmdColorChange()
        //{
        //    int idx = System.Array.IndexOf(Colors, playerColor);

        //    int inUseIdx = _colorInUse.IndexOf(idx);

        //    if (idx < 0) idx = 0;

        //    idx = (idx + 1) % Colors.Length;

        //    bool alreadyInUse = false;

        //    do
        //    {
        //        alreadyInUse = false;
        //        for (int i = 0; i < _colorInUse.Count; ++i)
        //        {
        //            if (_colorInUse[i] == idx)
        //            {//that color is already in use
        //                alreadyInUse = true;
        //                idx = (idx + 1) % Colors.Length;
        //            }
        //        }
        //    }
        //    while (alreadyInUse);

        //    if (inUseIdx >= 0)
        //    {//if we already add an entry in the colorTabs, we change it
        //        _colorInUse[inUseIdx] = idx;
        //    }
        //    else
        //    {//else we add it
        //        _colorInUse.Add(idx);
        //    }

        //    playerColor = Colors[idx];
        //}

        //[ClientRpc]
        //public void RpcUpdateLeftRightButton(int numModelInUse)
        //{
        //    rightButton.interactable = isLocalPlayer && (numModelInUse < Models.Length);
        //    leftButton.interactable = rightButton.interactable;
        //}

        [Command]
        public void CmdNextModelChange(bool next)
        {
            if (_modelInUse.Count == ModelNames.Length)
            {
                return;  
            }

            int idx = System.Array.IndexOf(ModelNames, playerModelName);

            int inUseIdx = _modelInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % ModelNames.Length;

            bool alreadyInUse = false;
            int count = ModelNames.Length;
            do
            {
                --count;
                alreadyInUse = false;
                for (int i = 0; i < _modelInUse.Count; ++i)
                {
                    if (_modelInUse[i] == idx)
                    {
                        alreadyInUse = true;
                        idx = (idx + 1) % ModelNames.Length;
                    }
                }
            }
            while (alreadyInUse && count != -1);
          
            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _modelInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _modelInUse.Add(idx);
            }

            playerModelName = ModelNames[idx];
        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx >= 0)
            {
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        _colorInUse.RemoveAt(i);
                        break;
                    }
                }
            }

            idx = System.Array.IndexOf(ModelNames, playerModelName);

            for (int i = 0; i < _modelInUse.Count; ++i)
            {
                if (_modelInUse[i] == idx)
                {
                    _modelInUse.RemoveAt(i);
                    break;
                }
            }

            LobbyPlayerList._instance.PlayerListModified();
        }
    }
}
