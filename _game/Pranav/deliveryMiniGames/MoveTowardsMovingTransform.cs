using UnityEngine;

public class MoveTowardsMovingTransform : MonoBehaviour
{
    public bool offOnContact;

    public Transform targetTransform; // The target moving transform to move towards
    public float moveSpeed = 10f; // The speed at which this transform moves towards the target
    public float minDistance = 0.1f; // The minimum distance at which to disable the script

    void Update()
    {
        if (targetTransform != null)
        {
            // Move towards the target in a straight line
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);

            // Check the distance to the target
            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

            // Disable the script if the distance is less than the minimum distance
            if (distanceToTarget < minDistance)
            {
                enabled = false;
                if (offOnContact==true)
                {
                    targetTransform = null;
                    gameObject.SetActive(false);
                }

            }
        }
    }
}
