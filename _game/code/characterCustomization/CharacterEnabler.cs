using UnityEngine;
using static CharacterSelectionHandler;


public class CharacterEnabler : MonoBehaviour
{
    public GameObject african;
    public GameObject caucasian;

    private void OnEnable()
    {
        CharacterSelectionHandler.OnBodyTypeSelected += CharacterSelectionHandler_OnBodyTypeSelected;
    }

    private void CharacterSelectionHandler_OnBodyTypeSelected(BodyType obj)
    {
        EnableCharacter();
    }

    private void OnDisable()
    {
        CharacterSelectionHandler.OnBodyTypeSelected -= CharacterSelectionHandler_OnBodyTypeSelected;
    }


    private void Start()
    {
        EnableCharacter();
    }

    public void EnableCharacter()
    {
        if (PlayerPrefs.HasKey("SELECTED_CHARACTER"))
        {
            if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
            {
                african.SetActive(true);
                caucasian.SetActive(false);

            }
            else if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
            {
                african.SetActive(false);
                caucasian.SetActive(true);


            }
        }

    }
}
