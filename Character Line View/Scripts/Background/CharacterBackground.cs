/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    Stores and displays the different backgrounds used in a dialogue box
///    depending on what character is talking.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace CharacterLineView
{
    public class CharacterBackground : MonoBehaviour, ICharacterDisplayable
    {
        // A serializable struct so we can use the inspector
        // to store backgrounds with their associated character's name.
        [Serializable]
        private class BackgroundImage
        {
            public string characterName;

            public Sprite image;
        }

        [SerializeField]
        private Sprite defaultBackground;
        [SerializeField]
        private BackgroundImage[] characterSpecificBackgrounds;

        private Dictionary<string, Sprite> backgroundsDict;
        private Image imageComponent;


        /// <summary>
        /// This method is called when the GameObject first becomes enabled.
        /// </summary>
        void Start()
        {
            // Create the dictionary of backgrounds/dialogue boxes associated with each characters name
            backgroundsDict = new Dictionary<string, Sprite>();
            foreach (BackgroundImage background in characterSpecificBackgrounds)
                backgroundsDict.Add(background.characterName, background.image);

            // Find the image component of this GameObject
            imageComponent = GetComponent<Image>();
        }


        /// <interitdoc/>
        /// <summary>
        /// Change the background image/dialogue box to match the current talking character.
        /// </summary>
        public void SetDisplayable(LocalizedLine dialogueLine)
        {
            string character = dialogueLine.CharacterName;

            // If the current talking character has a special background, display it
            if (backgroundsDict.ContainsKey(character))
            {
                imageComponent.sprite = backgroundsDict[character];
                return;
            }

            // Otherwise, if a default background exists, use it
            else if (defaultBackground != null)
            {
                imageComponent.sprite = defaultBackground;
                return;
            }

            // Error case: No default background nor character background is defined
            Debug.LogWarning("No default background exists to display, " +
                        $"& the character \"{character}\" does not have a character background.");
        }
    }
}