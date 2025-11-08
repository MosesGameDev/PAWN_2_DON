using Unity.AppUI.Core;
using UnityEngine;
using static GameAnalyticsController;

public class AnalyticsEvents : MonoBehaviour
{
    public static AnalyticsEvents instance;


    private void Awake()
    {
        instance = this;
    }

    public void OnLevelStart(int day)
    {
        //if (Application.isEditor || day <1) { return; }
        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;

            print(day);
        }

        LevelBasedProgressionRelated.LogLevelStartEventWithTime(day);
        UniqueEvent($"{day}_start");
    }

    public void OnLevelEndFail(int day)
    {
        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        LevelBasedProgressionRelated.LogLevelFailEvent(day);
    }
    
    public void OnLevelEndSuccess(int day)
    {

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        LevelBasedProgressionRelated.LogLevelEndEventWithTime(LevelBasedProgressionRelated.levelProgressTimeData);
    }

    public void UniqueEvent(string eventName)
    {
        Miscellaneous.NewDesignEvent(eventName);
    }

    public void Day1Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            print(PlayerPrefs.GetInt("RESET_COUNT"));
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day2Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day3Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }
        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day4Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day5Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day6Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }
        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day7Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day8Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }

        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day9Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }
        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }

    public void Day10Events(string eventName)
    {
        int day = PlayerPrefs.GetInt("START_CONVERSATION_ID");

        if (PlayerPrefs.HasKey("COMPLETED"))
        {
            int resetCount = PlayerPrefs.GetInt("RESET_COUNT");
            int d = resetCount * 10;
            day += d;
        }
        Miscellaneous.NewDesignEvent($"day{day}:" + eventName);
    }



}
