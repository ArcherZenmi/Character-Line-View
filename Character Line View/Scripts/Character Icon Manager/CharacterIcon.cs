/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    Stores the different emotions/icons for a character,
///    and can easily swap between different images.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterLineView
{
    public class CharacterIcon : MonoBehaviour
    {
        [Serializable]
        private struct EmotionIcon
        {
            public string name;

            public Sprite image;
        }

        public string characterName;

        [SerializeField]
        private EmotionIcon[] emotions;

        private Dictionary<string, Sprite> emotionsDict;
        private Image imageComponent;


        void Start()
        {
            // Create the dictionary of icons associated with each emotion's name
            emotionsDict = new Dictionary<string, Sprite>();
            foreach (EmotionIcon emotion in emotions)
                emotionsDict.Add(emotion.name, emotion.image);

            // Find the imageComponent of this GameObject
            imageComponent = GetComponent<Image>();

            gameObject.SetActive(false);
        }


        /// <summary>
        /// Sets the current displayed icon to the new emotion.
        /// If the emotion can't be found, the current icon will be hidden
        /// and nothing will be displayed.
        /// </summary>
        /// <param name="newEmotion"> The emotion to set the characters icon to. </param>
        public void SetEmotion(string newEmotion)
        {
            // Error case: This character may not have an icon for the given emotion
            if (!emotionsDict.ContainsKey(newEmotion))
            {
                Debug.LogWarning($"\"{characterName}\" doesn't contain an icon for the emotion \"{newEmotion}\".");
                gameObject.SetActive(false);
                return;
            }

            // Display the new icon for the characters current emotion
            imageComponent.sprite = emotionsDict[newEmotion];
            gameObject.SetActive(true);
        }
    }
}