using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StackObjects : MonoBehaviour
{
    int maxCaacity = 6;
    public Transform stackedObjectsParentTransform;
    public Transform stackPoint;

    public Transform[] stackPoints;

    public int xDisplacement, yDisplacement;
    public bool startedStacking;
    public string type;

    public int capacityToStack;
    public int currentStack;
    private void Awake()
    {
        currentStack = 0;
        GetPickup = GetComponent<PickupMovement>();
        //stackPos = stackPoint.position;
    }
    public bool food;
    private void OnTriggerEnter(Collider other)
    {
        if (startedStacking)
        {
            if (currentStack==0)
            {
                type = null;
                startedStacking = false;
                return;
            }
            if (other.CompareTag(type))
            {
                Stack(other.transform);
                //other.gameObject.tag = "Unstack";
                if (type=="Pizza")
                {
                    food = true;
                }
            }
        }
        else
        {
            if (other.CompareTag("Trash"))
            {
                type = "Trash";
                startedStacking = true;
                food = false;
                //other.gameObject.tag = "Unstack";
                Stack(other.transform);
            }
            else if (other.CompareTag("Pizza"))
            {
                type = "Pizza";
                startedStacking = true;
                food = true;
                //other.gameObject.tag = "Unstack";
                Stack(other.transform);
            }
        }
        /*
        if (other.CompareTag("Cash"))
        {
            //other.gameObject.tag = "Untagged";
            //other.transform.DOMove(transform.position, 1);
        }
        */
        /*
        if (other.CompareTag("S_Stackable")) // single column
        {

        }
        else if (other.CompareTag("D_Stackable")) // double column
        {

        }
        */
    }
    PickupMovement GetPickup;
    void Stack(Transform toStack) // move the stackable object to the free spot
    {
        if (!CheckStack())
        {
            return;
        }
        toStack.gameObject.tag = "Unstack";

        //selected.Add(toStack);

        stackPos = stackPoints[currentStack];
        currentStack++;

        //toStack.DOMove(stackPos.position,.2f);
        GetPickup.InwardMovementForPickUp(toStack,stackPos);
        //toStack.GetComponent<MoveTowardsMovingTransform>().targetTransform = stackPos;
        toStack.SetParent(stackedObjectsParentTransform);
        //UpdateStack();      
    }
    Transform stackPos;
    /*
    void UpdateStack()
    {
        stackPos = stackPoint.position;

        if (currentStack % 2 == 0) // 0,2,4,6
        {
            x = 0;
            if (currentStack != 0)
            {
                y += yDisplacement;
            }
        }
        else // 1,3,5,7
        {
            x = xDisplacement;
        }
        stackPos += new Vector3(x,y,0);
        currentStack++;

    }
    */
   //[SerializeField] List<Transform> selected;
    bool CheckStack() // check if there is a free spot and increment counters for position and capacity
    {
        if (currentStack<=capacityToStack)
        {
            //UpdateStack();
            //Debug.Log("Current Capacity= "+currentStack);
            return true;
        }
        else
        {
            return false;
        }
    }
}
