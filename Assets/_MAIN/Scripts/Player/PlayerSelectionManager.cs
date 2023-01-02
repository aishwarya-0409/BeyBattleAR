using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;
    public GameObject[] beyModels;

    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;
    public Button nextButton;
    public Button previousButton;

    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;

    #region UNITY Methods
    void Start()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);

        playerSelectionNumber = 0;
    }

    void Update()
    {

    }
    #endregion

    #region UI Callback Methods
    public void NextPlayer()
    {
        playerSelectionNumber += 1;

        if (playerSelectionNumber >= beyModels.Length)
        {
            playerSelectionNumber = 0;
        }
#if UNITY_EDITOR
        Debug.Log("Player Select: " + playerSelectionNumber);
#endif
        nextButton.enabled = false;
        previousButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));

        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            playerModelType_Text.text = "Attack";
        }
        else
        {
            playerModelType_Text.text = "Defend";
        }
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber -= 1;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = beyModels.Length - 1;
        }
#if UNITY_EDITOR
        Debug.Log("Player Select: " + playerSelectionNumber);
#endif
        nextButton.enabled = false;
        previousButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            playerModelType_Text.text = "Attack";
        }
        else
        {
            playerModelType_Text.text = "Defend";
        }
    }

    public void OnSelectButtonCiicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { csStoreData.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnReselectButtonClicked()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f)
    {
        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle);

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transformToRotate.rotation = finalRotation;

        nextButton.enabled = true;
        previousButton.enabled = true;
    }
    #endregion
}
