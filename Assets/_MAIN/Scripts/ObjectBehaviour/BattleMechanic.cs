using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleMechanic : MonoBehaviourPun
{
    public Spinner spinnerScript;

    public GameObject uI_3D_Gameobject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;

    private Rigidbody rb;

    private float startSpinSpeed;
    private float currentSpinSpeed;

    public Image spinSpeedbar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    public bool isAttacker;
    public bool isDefender;
    public bool isDead = false;

    public float commom_Damage_Coefficient = 0.04f;

    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f;       // do more damage than defender - ADVANTAGE
    public float getDamage_Coefficient_Attacker = 1.2f;     // get more damage - DISADVANTAGE

    public float doDamage_Coefficient_Defender = 0.75f;     // do less damage - DISADVANTAGE
    public float getDamage_Coefficient_Defender = 0.2f;     // get less damage - ADVANTAGE

    // vfx variable
    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedbar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();

        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            spinnerScript.spinSpeed = 4400;

            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

            // instantiate collision effect particle system
            GameObject collisionEffectGameobject = GetPooledObject();
            if (collisionEffectGameobject != null)
            {
                collisionEffectGameobject.transform.position = effectPosition;
                collisionEffectGameobject.SetActive(true);
                collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                // de-activate collision effect particle system after some seconds.
                StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // Comparing the speeds of the SpinnerTops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

#if UNITY_EDITOR
            Debug.Log("My speed: " + mySpeed + " ----- Other player speed: " + otherPlayerSpeed);
#endif

            if (mySpeed > otherPlayerSpeed)
            {
#if UNITY_EDITOR
                Debug.Log("You DAMAGE the other player.");
#endif
                float default_Damage_Amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * commom_Damage_Coefficient;

                if (isAttacker)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Attacker;
                }
                else if (isDefender)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Defender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // Apply damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_Damage_Amount);
                }
            }
        }
    }

    [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamage_Coefficient_Attacker;

                if (_damageAmount > 1000)
                {
                    _damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damageAmount *= getDamage_Coefficient_Defender;
            }
            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedbar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                // Die condition
                Die();
            }
        }
    }

    private void Die()
    {
        isDead = true;

        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;

        uI_3D_Gameobject.SetActive(false);

        if (photonView.IsMine)
        {
            // countdown for respawn
            StartCoroutine(Respawn());
        }
    }

    [PunRPC]
    public void Reborn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedbar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        uI_3D_Gameobject.SetActive(true);

        isDead = false;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    IEnumerator Respawn()
    {
        GameObject canvasGameoject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject == null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameoject.transform);
        }
        else
        {
            deathPanelUIGameobject.SetActive(true);
        }
        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;
        }
        deathPanelUIGameobject.SetActive(false);

        GetComponent<MovementController>().enabled = true;
        photonView.RPC("Reborn", RpcTarget.AllBuffered);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
}
