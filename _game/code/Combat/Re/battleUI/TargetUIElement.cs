using UnityEngine;
using System.Collections;

/// <summary>
/// UI element that automatically positions itself over a target game object in world space.
/// This component should be attached to a UI element with a RectTransform.
/// </summary>
public class TargetUIElement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // Offset from target position (e.g., display above head)

    [Header("Appearance")]
    [SerializeField] private bool fadeInOut = true;
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeOutDuration = 0.25f;

    [Header("Animation")]
    [SerializeField] private bool animatePosition = true;
    [SerializeField] private float moveSpeed = 10f;

    [Header("References")]
    [SerializeField] private Camera worldCamera; // Camera used for world to screen conversion
    [SerializeField] private CanvasGroup canvasGroup; // For fade in/out effects

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private bool isVisible = false;
    private Vector3 targetScreenPosition;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("TargetUIElement requires a RectTransform component!");
            enabled = false;
            return;
        }

        // Try to find the canvas group if not assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null && fadeInOut)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Find the parent canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("TargetUIElement must be a child of a Canvas!");
            enabled = false;
            return;
        }

        // If no camera is assigned, try to use the main camera
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
            if (worldCamera == null)
            {
                Debug.LogError("No camera assigned and no Main Camera found!");
                enabled = false;
                return;
            }
        }

        // Initialize as invisible
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
        isVisible = false;
    }

    private void Update()
    {
        if (targetTransform != null)
        {
            UpdatePosition();
        }
    }

    /// <summary>
    /// Set a new target for the UI element to follow
    /// </summary>
    /// <param name="newTarget">Transform of the target to follow</param>
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;

        if (newTarget != null)
        {
            // Make the UI element visible when a target is set
            if (!isVisible)
            {
                isVisible = true;
                if (fadeInOut && canvasGroup != null)
                {
                    // Stop any existing fade coroutine
                    if (fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine);
                    }
                    fadeCoroutine = StartCoroutine(FadeIn());
                }
            }

            // Get the initial position
            UpdatePosition(false);
        }
        else
        {
            ClearTarget();
        }
    }

    /// <summary>
    /// Clear the current target
    /// </summary>
    public void ClearTarget()
    {
        targetTransform = null;

        if (isVisible)
        {
            isVisible = false;
            if (fadeInOut && canvasGroup != null)
            {
                // Stop any existing fade coroutine
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeOut());
            }
        }
    }

    /// <summary>
    /// Update the position of the UI element based on the target's position
    /// </summary>
    /// <param name="animate">Whether to animate the movement</param>
    public void UpdatePosition(bool animate = true)
    {
        if (targetTransform == null || worldCamera == null || !isVisible)
        {
            return;
        }

        // Convert world position to screen position with offset
        Vector3 targetPositionWithOffset = targetTransform.position + offset;
        Vector3 newScreenPosition = worldCamera.WorldToScreenPoint(targetPositionWithOffset);

        // Check if target is behind the camera
        if (newScreenPosition.z < 0)
        {
            // Target is behind camera, hide UI element
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
            return;
        }
        else if (canvasGroup != null && isVisible && canvasGroup.alpha == 0)
        {
            // Target is back in view, show UI element
            canvasGroup.alpha = 1;
        }

        // Store target position and handle different canvas render modes
        targetScreenPosition = newScreenPosition;
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            if (animatePosition && animate)
            {
                // Smoothly move toward the target position
                Vector2 currentPos = rectTransform.position;
                Vector2 targetPos = new Vector2(targetScreenPosition.x, targetScreenPosition.y);
                rectTransform.position = Vector2.Lerp(currentPos, targetPos, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Instantly set to target position
                rectTransform.position = new Vector3(targetScreenPosition.x, targetScreenPosition.y, 0);
            }
        }
        else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Convert screen position to canvas space
            Vector2 viewportPosition = new Vector2(
                targetScreenPosition.x / Screen.width,
                targetScreenPosition.y / Screen.height);

            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.GetComponent<RectTransform>(),
                new Vector2(targetScreenPosition.x, targetScreenPosition.y),
                parentCanvas.worldCamera,
                out canvasPosition);

            if (animatePosition && animate)
            {
                // Smoothly move toward the target position
                Vector2 currentPos = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = Vector2.Lerp(currentPos, canvasPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Instantly set to target position
                rectTransform.anchoredPosition = canvasPosition;
            }
        }
        else // World Space Canvas
        {
            Debug.LogWarning("TargetUIElement is not optimized for World Space Canvas! Consider using a different approach.");
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        fadeCoroutine = null;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        fadeCoroutine = null;
    }
}