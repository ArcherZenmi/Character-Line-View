/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///     This file specifies the behavior of the name text box in Character Line View.
///
///     It specifies what actual names should be displayed for the character tags
///         used in YarnScript.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace CharacterLineView
{
    public class NameTextBox : MonoBehaviour, ICharacterDisplayable
    {
        [Serializable]
        private class NameAndTag
        {
            public string yarnScriptTag;

            public string displayedName;
        }

        [SerializeField]
        private NameAndTag[] nameAndTags;

        // Key:     Tag used in YarnScript.
        // Value:   Actual name to display.
        private Dictionary<string, string> namesDict;

        /// <summary>
        /// Initialize variables.
        /// </summary>
        public void Start()
        {
            // Create a dictionary of character tags mapped to actually displayed names
            namesDict = new Dictionary<string, string>();
            foreach (NameAndTag nameAndTag in nameAndTags)
                namesDict.Add(nameAndTag.yarnScriptTag, nameAndTag.displayedName);
        }


        /// <summary>
        /// Change the currently displayed name based on a YarnScript tag.
        /// </summary>
        public void SetDisplayable(LocalizedLine dialogueLine)
        {
            TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
            textMesh.text = namesDict[dialogueLine.CharacterName];
        }
    }
}
