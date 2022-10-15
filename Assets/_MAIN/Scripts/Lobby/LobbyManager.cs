using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject uI_LoginGameObject;

    [Header("Lobby UI")]
    public GameObject uI_LobbyGameObject;
    public GameObject uI_3DGameObject;

    [Header("Connection Status UI")]
    public GameObject uI_ConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;

    #region UNITY Methods
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            uI_LobbyGameObject.SetActive(true);
            uI_3DGameObject.SetActive(true);

            uI_ConnectionStatusGameObject.SetActive(false);
            uI_LoginGameObject.SetActive(true);
        }
        else
        {
            uI_LobbyGameObject.SetActive(false);
            uI_3DGameObject.SetActive(false);
            uI_ConnectionStatusGameObject.SetActive(false);

            uI_LoginGameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClick()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            uI_LobbyGameObject.SetActive(false);
            uI_3DGameObject.SetActive(false);
            uI_LoginGameObject.SetActive(false);

            showConnectionStatus = true;
            uI_ConnectionStatusGameObject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Player name is invalid or empty!");
#endif
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }
    #endregion

    #region PHOTON Callback Methods
    public override void OnConnected()
    {
#if UNITY_EDITOR
        Debug.Log("We connected to Internet");
#endif
    }

    public override void OnConnectedToMaster()
    {
        uI_LobbyGameObject.SetActive(true);
        uI_3DGameObject.SetActive(true);

        uI_LoginGameObject.SetActive(false);
        uI_ConnectionStatusGameObject.SetActive(false);
#if UNITY_EDITOR
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");
#endif
    }
    #endregion
}
