/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    CharacterIconManager is responsible for displaying different
///    character icons depending on who's talking and what
///    their emotion is.
///
///    A game object with a CharacterIconManager is expected to have
///    one or more children with a CharacterIcon component.
///    
///    Each CharacterIcon will be in charge of displaying different
///    icons depending on their character's emotion,
///    while CharacterIconManager will be in charge of deciding
///    which character is speaking and thus which CharacterIcon is active.
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace CharacterLineView
{
    public class CharacterIconManager : MonoBehaviour, ICharacterDisplayable
    {
        private Dictionary<string, GameObject> characters;
        private string currentCharacter;


        /// <summary>
        /// Initialize references to all the character icons/GameObjects
        /// </summary>
        void Start()
        {
            // Initialize variables
            currentCharacter = null;
            characters = new Dictionary<string, GameObject>();

            // Initialize the list of character icons
            foreach (Transform child in transform)
            {
                GameObject characterObject = child.gameObject;
                CharacterIcon iconComponent = characterObject.GetComponent<CharacterIcon>();

                // Only add GameObjects that are character icons (has the CharacterIcon component)
                // Disable them to avoid showing all the icons before they're ready
                if (iconComponent != null)
                {
                    characters.Add(iconComponent.characterName, characterObject);
                }
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// Display the new character icon, as well as their current emotion.
        /// </summary>
        public void SetDisplayable(LocalizedLine dialogueLine)
        {
            string newCharacter = dialogueLine.CharacterName;

            // Error Case: The character doesn't exist
            if(!characters.ContainsKey(newCharacter))
            {
                Debug.LogWarning($"No CharacterIcon is defined for the character \"{newCharacter}\"." +
                    " You must add a child to the Character Icon Manager game object and specify it's" +
                    " \"Character Name\" variable in the inspector.");
                return;
            }

            // Check if the character changed their emotion
            string emotion = null;
            if (dialogueLine.Metadata != null)
            {
                foreach (string tag in dialogueLine.Metadata)
                {
                    if (tag.StartsWith("emotion:"))
                    {
                        emotion = tag.Substring("emotion:".Length);
                        break;
                    }
                }
            }

            // If the character has a new emotion, set it as such
            if (emotion != null)
            {
                CharacterIcon charIcon = characters[newCharacter].GetComponent<CharacterIcon>();
                charIcon.SetEmotion(emotion);
            }

            // If the talking character changes, make the new character visible and hide the old one
            if (newCharacter != currentCharacter)
            {
                if (currentCharacter != null)
                    characters[currentCharacter].SetActive(false);
                characters[newCharacter].SetActive(true);

                currentCharacter = newCharacter;
            }
        }
    }
}