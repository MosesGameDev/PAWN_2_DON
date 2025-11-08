using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnStack : MonoBehaviour
{
    public bool destroy;
    public StackObjects stacked;
    public float deductionInterval = 1.0f; // Deduction interval in seconds

    private float nextDeductionTime;
    /*
    private void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Unstack")&&destroy)
        {
            //other.gameObject.SetActive(false);
            //ScaleAndDisable(other.gameObject);
        }
    }
    */
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time >= nextDeductionTime)
            {
                if (stacked)
                {
                    Transform toRemove = stacked.stackedObjectsParentTransform.GetChild(stacked.stackedObjectsParentTransform.childCount - 1);
                    if (toRemove)
                    {
                        toRemove.parent = null;
                        Remove(toRemove);
                        stacked.currentStack--;
                        stacked.type = null;
                        stacked.startedStacking = false;
                        stacked.food = false;
                    }
                    
                    //nextDeductionTime = Time.deltaTime + deductionInterval;
                }
            }
        }
    }
    void Remove(Transform obj)
    {
        //obj.DOMove(transform.position, 1);
        InwardMovementForPickUp(obj,transform);
    }
    MoveTowardsMovingTransform move;
    public void InwardMovementForPickUp(Transform objToMove, Transform stackPoint)
    {

        objToMove.DOJump(stackPoint.position + new Vector3(Random.Range(-1, 1), 0, 0), 5, 1, .5f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        // Disable the object when the movement is complete
                        /*
                        move = objToMove.GetComponent<MoveTowardsMovingTransform>();
                        move.targetTransform = stackPoint;
                        move.offOnContact = true;
                        */
                        objToMove.gameObject.SetActive(false);
                    });
    }

    float scaleDuration = 1f; // Duration of the scale animation

    public void ScaleAndDisable(GameObject obj)
    {
        // Scale down the object using DOTween
        obj.transform.DOScale(Vector3.zero, scaleDuration).OnComplete(() =>
        {
            // Disable the object after scaling down
            obj.SetActive(false);
        });
    }
}
