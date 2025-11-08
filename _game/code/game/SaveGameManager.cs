using UnityEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class DialogueHistoryEntry
{
    public int entryId;
    public int conversationId;
    public int actorId;
    public string actorName;
    public string dialogueText;
    public string backgroundType;

    public int day;
    public int hp;
    public int power;
    public int cash;
    public int reputation;
    public bool isSpeechBubble;

    public PixelCrushers.DialogueSystem.CharacterInfo speakerInfo;
    public PixelCrushers.DialogueSystem.CharacterInfo coversant;

    public DialogueHistoryEntry(Subtitle subtitle)
    {
        entryId = subtitle.dialogueEntry.id;
        dialogueText = subtitle.dialogueEntry.DialogueText;
        conversationId = DialogueManager.lastConversationID;


        actorId = subtitle.dialogueEntry.ActorID;

        if (subtitle.speakerInfo != null && !string.IsNullOrEmpty(subtitle.speakerInfo.Name))
        {
            actorName = subtitle.speakerInfo.Name;
        }
        else if (subtitle.dialogueEntry.ActorID > 0)
        {
            Actor actor = DialogueManager.MasterDatabase.GetActor(subtitle.dialogueEntry.ActorID);
            if (actor != null)
            {
                actorName = actor.Name;
            }
        }

        speakerInfo = subtitle.speakerInfo;
        coversant = subtitle.listenerInfo;

        // Check for background type
        if (Field.FieldExists(subtitle.dialogueEntry.fields, "ENTRY-BG"))
        {
            backgroundType = Field.LookupValue(subtitle.dialogueEntry.fields, "ENTRY-BG");

            if(backgroundType == "actor")
            {
                isSpeechBubble = true;
            }

        }

        // Store parameter values
        if (Field.FieldExists(subtitle.dialogueEntry.fields, "DAY"))
        {
            day = Field.LookupInt(subtitle.dialogueEntry.fields, "DAY");
        }

        if (Field.FieldExists(subtitle.dialogueEntry.fields, "HP"))
        {
            hp = Field.LookupInt(subtitle.dialogueEntry.fields, "HP");
        }

        if (Field.FieldExists(subtitle.dialogueEntry.fields, "POWER"))
        {
            power = Field.LookupInt(subtitle.dialogueEntry.fields, "POWER");
        }

        if (Field.FieldExists(subtitle.dialogueEntry.fields, "CASH"))
        {
            cash = Field.LookupInt(subtitle.dialogueEntry.fields, "CASH");
        }

        if (Field.FieldExists(subtitle.dialogueEntry.fields, "REPUTATION"))
        {
            reputation = Field.LookupInt(subtitle.dialogueEntry.fields, "REPUTATION");
        }
    }
}

public class SaveGameManager : MonoBehaviour
{
    public string currentConversationID;
    public int currentConversationDialogueEntry;
    public static SaveGameManager instance;

    private const string CONVERSATION_ID_KEY = "CurrentConversationID";
    private const string DIALOGUE_ENTRY_KEY = "CurrentDialogueEntry";
    private const string DIALOGUE_HISTORY_COUNT_KEY = "DialogueHistoryCount";
    private const string DIALOGUE_HISTORY_ENTRY_PREFIX = "DialogueHistoryEntry_";

    [SerializeField, ReadOnly]
    private List<DialogueHistoryEntry> dialogueHistory = new List<DialogueHistoryEntry>();
    public static event Action OnDataSaved;

    private void Awake()
    {
        instance = this;
        LoadConversationData();
    }

    public void OnConversationLinePlayed(Subtitle subtitle)
    {
        // Skip if this is actor 1 (usually the player) or if the text is empty
        if (subtitle.dialogueEntry.ActorID == 1 || string.IsNullOrEmpty(subtitle.dialogueEntry.DialogueText))
        {
            return;
        }

        // Store current conversation state
        currentConversationDialogueEntry = subtitle.dialogueEntry.id;
        currentConversationID = DialogueManager.lastConversationStarted;

        // Add to dialogue history
        var historyEntry = new DialogueHistoryEntry(subtitle);

        // Check if this entry already exists in history (to avoid duplicates)
        bool exists = false;
        foreach (var entry in dialogueHistory)
        {
            if (entry.entryId == historyEntry.entryId &&
                entry.dialogueText == historyEntry.dialogueText)
            {
                exists = true;
                break;
            }
        }

        if (!exists)
        {
            dialogueHistory.Add(historyEntry);
            //Debug.Log($"Added dialogue entry {historyEntry.entryId} to history. Total: {dialogueHistory.Count}");
        }

        // Save conversation data whenever it changes
        //SaveConversationData();
    }

    [Button]
    public void StartSavedDialogue()
    {
        // Reset the dialogue tracking before starting the saved dialogue
        ScriptRegistry.Instance.textGameController.ResetDialogueTracking();

        if (string.IsNullOrEmpty(currentConversationID))
        {
            Debug.LogWarning("No saved conversation ID found.");
            return;
        }

        Transform player = ScriptRegistry.Instance.player.transform;
        Transform conversant = ScriptRegistry.Instance.textGameController.transform;

        // Clear existing dialogue first
        ScriptRegistry.Instance.textGameController.ClearDialogueElements();

        // Recreate dialogue history if available
        if (dialogueHistory.Count > 0)
        {
            ScriptRegistry.Instance.textGameController.RecreateDialogueHistory(dialogueHistory);
        }

        // Now continue from the last point
        DialogueManager.instance.StartConversation(currentConversationID, player, conversant, currentConversationDialogueEntry);
    }


    public void SaveConversationData()
    {
        PlayerPrefs.SetString(CONVERSATION_ID_KEY, currentConversationID);
        PlayerPrefs.SetInt(DIALOGUE_ENTRY_KEY, currentConversationDialogueEntry);

        // Save dialogue history
        PlayerPrefs.SetInt(DIALOGUE_HISTORY_COUNT_KEY, dialogueHistory.Count);

        for (int i = 0; i < dialogueHistory.Count- 1; i++)
        {
            DialogueHistoryEntry entry = dialogueHistory[i];
            string json = JsonUtility.ToJson(entry);
            PlayerPrefs.SetString(DIALOGUE_HISTORY_ENTRY_PREFIX + i, json);
        }

        PlayerPrefs.Save();
        Debug.Log($"Saved conversation data. Entries: {dialogueHistory.Count}");
        OnDataSaved?.Invoke();
    }

    public void LoadConversationData()
    {
        // Get default values if PlayerPrefs doesn't have the keys
        currentConversationID = PlayerPrefs.GetString(CONVERSATION_ID_KEY, "");
        currentConversationDialogueEntry = PlayerPrefs.GetInt(DIALOGUE_ENTRY_KEY, 0);

        // Load dialogue history
        dialogueHistory.Clear();
        int historyCount = PlayerPrefs.GetInt(DIALOGUE_HISTORY_COUNT_KEY, 0);

        for (int i = 0; i < historyCount; i++)
        {
            string json = PlayerPrefs.GetString(DIALOGUE_HISTORY_ENTRY_PREFIX + i, "");
            if (!string.IsNullOrEmpty(json))
            {
                DialogueHistoryEntry entry = JsonUtility.FromJson<DialogueHistoryEntry>(json);
                dialogueHistory.Add(entry);
            }
        }
    }

    [Button]
    public void ClearSavedConversationData()
    {
        PlayerPrefs.DeleteKey(CONVERSATION_ID_KEY);
        PlayerPrefs.DeleteKey(DIALOGUE_ENTRY_KEY);

        // Clear dialogue history
        int historyCount = PlayerPrefs.GetInt(DIALOGUE_HISTORY_COUNT_KEY, 0);
        for (int i = 0; i < historyCount; i++)
        {
            PlayerPrefs.DeleteKey(DIALOGUE_HISTORY_ENTRY_PREFIX + i);
        }
        PlayerPrefs.DeleteKey(DIALOGUE_HISTORY_COUNT_KEY);

        PlayerPrefs.Save();

        currentConversationID = "";
        currentConversationDialogueEntry = 0;
        dialogueHistory.Clear();

        Debug.Log("Cleared all saved conversation data.");
    }


    public void SaveVariables()
    {
        var variables = ScriptRegistry.Instance.textGameController.variableUIElements;

        for (int i = 0; i < variables.Length; i++)
        {
            variables[i].SaveData();
        }

        //print($"<color=cyan>Saved {variables.Length} variables to PlayerPrefs.</color>");
    }

    public void LoadVariableData()
    {
        var variables = ScriptRegistry.Instance.textGameController.variableUIElements;

        for (int i = 0; i < variables.Length; i++)
        {
            variables[i].LoadData();
        }

        //print($"<color=yellow>Loaded {variables.Length} variables from PlayerPrefs.</color>");
    }
}