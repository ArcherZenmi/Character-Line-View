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
        private struct BackgroundImage
        {
            public string name;

            public Sprite image;
        }

        [SerializeField]
        private BackgroundImage[] backgrounds;

        private Dictionary<string, Sprite> backgroundsDict;
        private Image imageComponent;


        /// <summary>
        /// This method is called when the GameObject first becomes enabled.
        /// </summary>
        void Start()
        {
            // Create the dictionary of backgrounds/dialogue boxes associated with each characters name
            backgroundsDict = new Dictionary<string, Sprite>();
            foreach (BackgroundImage background in backgrounds)
                backgroundsDict.Add(background.name, background.image);

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

            // Error case: A background may not be defined for the character
            if (!backgroundsDict.ContainsKey(character))
            {
                Debug.LogWarning($"The character \"{character}\" does not have an " +
                    $"associated character background.");
                return;
            }

            // Display the background image for the current talking character
            imageComponent.sprite = backgroundsDict[character];
        }
    }
}