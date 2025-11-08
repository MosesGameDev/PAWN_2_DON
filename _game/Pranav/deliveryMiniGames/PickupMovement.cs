using UnityEngine;
using DG.Tweening;
using Lean.Pool;

public class PickupMovement : MonoBehaviour
{
    public GameObject cashPrefab;

    public Transform passThroughPoint; // Public transform for the pass through point
    public float chaseSpeed = 5f; // Speed at which the object chases the pass through point
    public Vector3 jumpPower; // Jump power for the arc movement

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cash"))
        {
            other.gameObject.tag = "Unstack";
            other.transform.DORotate(new Vector3(360f, 0f, 360f), 1, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(1);
            InwardMovement(other.transform);
        } else if (other.CompareTag("BuyButton"))
        {
            GameObject c = LeanPool.Spawn(cashPrefab, transform.position, Quaternion.identity);
            OutMovementForPickUp(c.transform, other.transform);
        }
    }

   public void InwardMovement(Transform objToMove)
    {
        // Move the object to the pass through point
        objToMove.DOMove(objToMove.position +new Vector3(Random.Range(-3, 4), Random.Range(0, 3), Random.Range(3,7)), .6f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Chase the pass through point in an arc until the individual distance is small
                objToMove.DOJump(transform.position+(Vector3.up*2), jumpPower.y, 1, chaseSpeed)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        // Disable the object when the movement is complete
                        objToMove.GetComponent<MoveTowardsMovingTransform>().targetTransform = transform;
                        //objToMove.gameObject.SetActive(false);
                    });
            });
    }
    public void InwardMovementForPickUp(Transform objToMove, Transform stackPoint)
    {

        objToMove.DOJump(stackPoint.position , 2, 1, .5f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        // Disable the object when the movement is complete
                        objToMove.GetComponent<MoveTowardsMovingTransform>().targetTransform = stackPoint;
                        //objToMove.gameObject.SetActive(false);
                    });
    }
    public void OutMovementForPickUp(Transform objToMove, Transform stackPoint)
    {

        objToMove.DOJump(stackPoint.position + new Vector3(Random.Range(-1, 2), 0, 0), jumpPower.y-3, 1, .5f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        // Disable the object when the movement is complete
                        //objToMove.GetComponent<MoveTowardsMovingTransform>().targetTransform = stackPoint;
                        objToMove.gameObject.SetActive(false);
                    });
    }
}
