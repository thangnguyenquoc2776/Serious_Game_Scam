using UnityEngine;
using SeriousGame.Runtime;

// Interactable chỉ dùng để set GameState, không trigger Beat/Episode
public class StateFlagInteractable : MonoBehaviour
{
    [Header("Game State Key to set")]
    public string flagKey;

    public bool valueToSet = true;

    [Header("One-shot?")]
    public bool oneShot = true;

    private bool _used = false;

    [Header("UI Prompt")]
    public GameObject interactPrompt; // Text/Canvas worldspace: "Press E to interact"

    void Start()
    {
        UpdatePromptVisibility();
    }

    public void Interact()
    {
        if (oneShot && _used) return;

        _used = true;

        // Ẩn prompt sau khi đã tương tác
        UpdatePromptVisibility();

        if (string.IsNullOrEmpty(flagKey))
        {
            Debug.LogWarning("[StateFlagInteractable] flagKey is empty.");
            return;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetFlag(flagKey, valueToSet);
        }
        else
        {
            Debug.LogWarning("[StateFlagInteractable] GameStateManager.Instance is null.");
        }
    }

    void UpdatePromptVisibility()
    {
        if (interactPrompt != null)
        {
            // Nếu oneShot và đã dùng rồi thì ẩn, còn lại thì hiện
            bool show = !(oneShot && _used);
            interactPrompt.SetActive(show);
        }
    }
}
