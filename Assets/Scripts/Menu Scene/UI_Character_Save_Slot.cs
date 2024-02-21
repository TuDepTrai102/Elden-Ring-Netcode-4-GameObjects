using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EldenRing.NT
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileWriter;

        [Header("GAME SLOT")]
        public CharacterSlot characterSlot;

        [Header("CHARACTER INFO")]
        public Text characterName;  //  OR WE CAN USE (TEXTMESHPROUGUI) //TextMeshProUGUI characterName;
        public Text timedPlayed;    //  OR WE CAN USE (TEXTMESHPROUGUI) //TextMeshProUGUI timedPlayed;

        private void OnEnable()
        {
            LoadSaveSlots();
        }

        private void LoadSaveSlots()
        {
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

            //SAVE SLOT 01 (IF/ELSE STATEMENT)
            /*if (characterSlot == CharacterSlot.CharacterSlot_01)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                
                //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                else
                {
                    gameObject.SetActive(false);
                }
            }*/

            //  SAVE SLOT (SWITCH STATEMENT)
            switch (characterSlot)
            {
                case CharacterSlot.CharacterSlot_01:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break; 
                case CharacterSlot.CharacterSlot_02:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_03:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_04:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_05:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_06:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_07:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_08:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_09:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case CharacterSlot.CharacterSlot_10:

                    saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    //  IF THE FILE EXISTS, GET INFORMATION FROM IT
                    if (saveFileWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot10.characterName;
                    }
                    //  IF IT DOES NOT, DISABLE THIS GAMEOBJECT
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                default: 
                    break;
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
        }
    }
}