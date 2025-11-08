using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using PixelCrushers.DialogueSystem;

//[RequireComponent(typeof(BoxCollider))]
public class DeliveryAgent : MonoBehaviour
{
    public Animator dummyAnimator;

    public float pedestrianHitForce = 3;

    [Space]
    public FeelVibrationManager feelVibrationManager;


    public int currentItems;
    public Transform stackObjParent;
    public float stackTime = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeliveryDrop"))
        {
            DeliveryDropPoint dropPoint = other.GetComponent<DeliveryDropPoint>();
            if (dropPoint != null && !dropPoint.processed)
            {
                if (feelVibrationManager!=null)
                {
                    feelVibrationManager.PlayLightHaptic();
                }
                ProcessDelivery(dropPoint);
            }
        }
        else if (other.CompareTag("DeliveryItemToPickUp"))
        {
            ProcessDeliveryPickUp(other.transform);
        }
        else if (other.CompareTag("TrafficObstacle"))
        {
            if (ScriptRegistry.Instance != null)
            {
                if (feelVibrationManager != null)
                {
                    feelVibrationManager.PlayLightHaptic();
                }
                print("Play Vibration");
            }
            Debug.Log("hit car");
            KillPlayer();
           // ProcessObstacleCollision();
        }
        else if (other.CompareTag("Pedestrian"))
        {
            if(ScriptRegistry.Instance != null)
            {
                if (feelVibrationManager!=null)
                {
                    feelVibrationManager.PlayLightHaptic();
                }
                print("Play Vibration");
            }

            ProcessObstacleCollision();

            ApplyImpactForceToPedestrian(other);

            KillTrafficObject(other.transform.parent.parent);

            DealDamageToPlayerHealth();
        }
    }
    int pedestrianCollisionDamage = 1;
    void DealDamageToPlayerHealth()
    {
        int currentHealth = DialogueLua.GetVariable("HP").asInt;
    }

    void KillTrafficObject(Transform other)
    {
        TrafficObject trafficObject = other.GetComponent<TrafficObject>();
        if (trafficObject != null)
        {
            trafficObject.KillObject();
        }
        else
        {
            Debug.Log("hit Pedestrian no trafficObject");

        }
    }


    void ApplyImpactForceToPedestrian(Collider pedestrian)
    {
        Rigidbody rb = pedestrian.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            Vector3 impactDirection = -transform.forward;
            float forceMagnitude = pedestrianHitForce;
            rb.AddForce(impactDirection * forceMagnitude, ForceMode.Impulse);
        }
    }
    void ProcessObstacleCollision()
    {
        if (currentItems>0)
        {
            DeliveryBox box = GetFullStackPoint(false);

            DropBox(box, 2, 4);

            DisableBox(box);
            currentItems--;
            // StartCoroutine(DisableBoxAfterSeconds(box, 2f));
        }

    }
    public PlayerDeliveryController playerDeliveryController;

    void ToggleMainVisual(bool s)
    {
        if (GetSetVehicleVisual!=null)
        {
            GetSetVehicleVisual.GetMainVisual().SetActive(s);
        }
        else
        {
            if (mainVisual!=null)
            {
                mainVisual.SetActive(s);
            }
        }
    }


    public GameObject mainVisual;
    public GameObject dummyRagdollParent;
    public Tween pavementTween;
    public float force = 5f;
    public float duration = 1.5f;

    public Image deathImage;           
    private float fadeDuration = 1;

    public void KillPlayer()
    {
        if (!dying)
        {
            dying = true;
            StartCoroutine(PlayDeathWithRagdoll());
            PlayDeathFade();

            if (playerDeliveryController == null)
                playerDeliveryController = GetComponent<PlayerDeliveryController>();

            if (playerDeliveryController != null)
                playerDeliveryController.OnMovementButtonUp();
        }
    }

    private void PlayDeathFade()
    {
        if (deathImage == null)
        {
            return;
        }
        deathImage.gameObject.SetActive(true);

        Color currentColor = deathImage.color;
        currentColor.a = 0f;
        deathImage.color = currentColor;

        deathImage.DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                deathImage.DOFade(0f, fadeDuration)
                    .SetEase(Ease.InOutSine)
                    .SetDelay(0.5f)
                    .OnComplete(() =>
                    {
                        deathImage.gameObject.SetActive(false);
                    });
            });
    }


    public bool dying;

    void HideMainCycle()
    {

    }

    public SetVehicleVisual GetSetVehicleVisual;
    private IEnumerator PlayDeathWithRagdoll()
    {
        //  mainVisual.SetActive(false);
        // ToggleMainVisual(false);

        if (GetSetVehicleVisual!=null)
        {
            ToggleMainVisual(false);
        }
        if (playerDeliveryController.currentPlayerModel.animator!=null)
        {
            playerDeliveryController.currentPlayerModel.animator.SetTrigger("Fall");
        }
        if (playerDeliveryController.mainVehicleAnimator!=null)
        {
            playerDeliveryController.mainVehicleAnimator.SetTrigger("Fall");
        }


        dummyRagdollParent.SetActive(true);

        List<Transform> ragdollChildren = new();
        List<Vector3> originalLocalPositions = new();
        List<Quaternion> originalLocalRotations = new();

        if (dummyAnimator!=null)
        {
            dummyAnimator.SetTrigger("Fall");
        }

        /*
        foreach (Transform child in dummyRagdollParent.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ragdollChildren.Add(child);
                    originalLocalPositions.Add(child.localPosition);
                    originalLocalRotations.Add(child.localRotation);

                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddForce((child.forward + Vector3.up) * force, ForceMode.Impulse);
                }
            }
           
        }
        */
        
        yield return new WaitForSeconds(duration);

        dummyRagdollParent.SetActive(false);

        if (playerDeliveryController.currentPlayerModel.animator!=null)
        {
            playerDeliveryController.currentPlayerModel.animator.SetTrigger("Stop");
        }

        if (playerDeliveryController.mainVehicleAnimator != null)
        {
            playerDeliveryController.mainVehicleAnimator.SetTrigger("Stop");
        }


        /*
        for (int i = 0; i < ragdollChildren.Count; i++)
        {
            ragdollChildren[i].localPosition = originalLocalPositions[i];
            ragdollChildren[i].localRotation = originalLocalRotations[i];
        }
        */
        //   mainVisual.SetActive(true);
        ToggleMainVisual(true);

        ResetTween();
        dying = false;

    }

    void ResetTween()
    {
        pavementTween = playerDeliveryController.movementTween;

        float currentPct = pavementTween.ElapsedPercentage();
        float resumePct = Mathf.Max(currentPct - 0.05f, 0f);
        float resumeTime = pavementTween.Duration() * resumePct;

        pavementTween.Goto(resumeTime, true);
        pavementTween.Pause();
        //  pavementTween.Play();
    }

    private void ProcessDelivery(DeliveryDropPoint point)
    {
        int amountToGive = Mathf.Min(currentItems, point.targetItems);

        //  point.CompleteOrder();
        point.AcceptItems(amountToGive);
        point.processed = true;

        for (int i = 0; i < amountToGive; i++)
        {
            DeliveryBox box = GetFullStackPoint(false);
            DisableBox(box);

           // DeliveryBox box = GetFullStackPoint();
            // SetBoxVisual(box, BoxVisual.Cash);
        }
        currentItems -= amountToGive;

        EarnCashViaDelivery();

        // JumpToStackAndDisable(box.transform, point.transform, null, false);
    }

    void EarnCashViaDelivery()
    {
        if (ScriptRegistry.Instance)
        {
            ScriptRegistry.Instance.textGameController.GetVariableUIElement("CASH").UpdateUIElementDirect_2(cashPerBox);
        }
    }

    int cashPerBox = 10;

    void DisableBox(DeliveryBox box)
    {
        box.reserved = false;
        box.currentVisual = BoxVisual.UnAssigned;
        box.gameObject.SetActive(false);
    }
    IEnumerator DisableBoxAfterSeconds(DeliveryBox box, float delay)
    {
        yield return new WaitForSeconds(delay);

        box.reserved = false;
        box.currentVisual = BoxVisual.UnAssigned;
        box.transform.parent.gameObject.SetActive(false);
    }

    private void ProcessDeliveryPickUp(Transform item)
    {
        // item.SetParent(stackObjParent);
        //  item.DOLocalMove(Vector3.zero, stackTime).SetEase(Ease.InOutSine);

        DeliveryBox freePoint = GetFreeStackPoint(); // boxObject.transform

        Transform stackPoint = freePoint.boxObject.transform; // boxObject.transform
        if (stackPoint!=null)
        {
            StartCoroutine(SetBoxVisualDelayed(boxJumpDuration, BoxVisual.Box, freePoint));
            JumpToStackAndDisable(item, stackPoint, stackPoint.gameObject);
            currentItems++;
            item.tag = "StackedObj";
        }
        else
        {
            Debug.Log("No space for delivery boy");
        }

        if(ScriptRegistry.Instance)
        {
            ScriptRegistry.Instance.feelVibrationManager.PlayLightHaptic();
        }
       
    }
    #region box movement
    public List<DeliveryBox> GetDeliveryBoxes;

    DeliveryBox GetFreeStackPoint()
    {
        foreach (var item in GetDeliveryBoxes)
        {
            if (item.currentVisual == BoxVisual.UnAssigned && !item.reserved) // !item.gameObject.activeInHierarchy
            {
                // item.gameObject.SetActive(true);
                item.reserved = true;
                return item;
            }
        }
        return null;
    }

    DeliveryBox GetFullStackPoint(bool accept = true) // true == first , false == last
    {
        if (accept)
        {
            foreach (var item in GetDeliveryBoxes)
            {
                if (item.currentVisual == BoxVisual.Box)
                {
                    return item;
                }
            }
        }
        else
        {
            for (int i = GetDeliveryBoxes.Count - 1; i >= 0; i--)
            {
                var item = GetDeliveryBoxes[i];
                if (item.currentVisual == BoxVisual.Box)
                {
                    return item;
                }
            }
        }

        return null;
    }
    IEnumerator SetBoxVisualDelayed(float delay, BoxVisual visual, DeliveryBox box)
    {
        yield return new WaitForSeconds(delay);
        box.gameObject.SetActive(true);
        SetBoxVisual(box,visual);
    }
    public void SetBoxVisual(int i, BoxVisual visual)
    {
        if (GetDeliveryBoxes == null || i < 0 || i >= GetDeliveryBoxes.Count)
            return;

        if (GetDeliveryBoxes[i] == null)
            return;

        GetDeliveryBoxes[i].transform.parent.gameObject.SetActive(true);
        GetDeliveryBoxes[i].SetVisual(visual);
    }
    public void SetBoxVisual(DeliveryBox box, BoxVisual visual)
    {

        if (box == null)
            return;

        box.transform.parent.gameObject.SetActive(true);
        box.SetVisual(visual);
    }
    public float boxJumpDuration = .2f;
    public Ease throwEase;

    public void JumpToStackAndDisable(
    Transform objToMove,
    Transform stackPoint,
    GameObject objectToEnable,
    bool collecting = true)
    {
        objToMove.DOJump(stackPoint.position, 0.4f, 1, boxJumpDuration) // .65f
            .SetEase(throwEase)
            .OnComplete(() =>
            {
                objToMove.gameObject.SetActive(false);
                if (collecting)
                {
                 //   objToMove.gameObject.SetActive(false);
                    if (objectToEnable != null)
                    {
                        objectToEnable.SetActive(true);
                    }
                }

            });
    }

    #endregion

    #region Drop Box Logic
    //public Rigidbody myRigidbody;
    void DropBox(DeliveryBox box, float disableAfterSeconds = 2f, float forceAmount = 2f)
    {
        if (box == null)
        {
            return;
        }
        DeliveryBox boxCopy = Lean.Pool.LeanPool.Spawn(box, box.transform.position, box.transform.rotation);
        Rigidbody copyRigidbody = boxCopy.myRigidbody;
        StartCoroutine(LaunchAndDisable(copyRigidbody, disableAfterSeconds, forceAmount));

        if (ScriptRegistry.Instance)
        {
            ScriptRegistry.Instance.feelVibrationManager.PlayLightHaptic();
        }
        // StartCoroutine(DisableBoxAfterSeconds(boxCopy, 2f));
    }

    IEnumerator LaunchAndDisable(Rigidbody rb, float disableAfterSeconds, float forceAmount)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(rb.transform.forward * forceAmount, ForceMode.Impulse);

        Collider col = rb.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        yield return new WaitForSeconds(disableAfterSeconds);

        rb.gameObject.SetActive(false);
    }

    #endregion
}