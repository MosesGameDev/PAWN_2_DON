using UnityEngine;

public class DeliveryBox : MonoBehaviour
{
    public BoxVisual currentVisual;
    public bool reserved;
    [SerializeField] public GameObject boxObject;
    [SerializeField] public GameObject cashObject;

    public Rigidbody myRigidbody;

    public void SetVisual(BoxVisual visualType)
    {
        currentVisual = visualType;

        if (visualType == BoxVisual.UnAssigned)
        {
            boxObject.SetActive(false);
            cashObject.SetActive(false);
        }
        else if (visualType == BoxVisual.Box)
        {
            boxObject.SetActive(true);
            cashObject.SetActive(false);
        }
        else if (visualType == BoxVisual.Cash)
        {
            boxObject.SetActive(false);
            cashObject.SetActive(true);
        }
    }
}

public enum BoxVisual
{
    UnAssigned,
    Box,
    Cash
}