/// <summary> 
/// Author:    Archer Zenmi
/// Package:   N/A
/// 
/// File Contents 
/// 
///     Character serves as a simplified way to display a character on
///         screen in a VN style.
///     Character supports having multiple emotions, as well as
///         moving the character to some predefined positions on screen.
///         (names of emotions & positions are case-insensitive).
///     Most importantly, Character can be controlled via YarnScript commands.
///
///     (Currently this is NOT part of the standard CLV package,
///         but hopefully it will be soon).
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

namespace CharacterLineView
{
    public class Character : MonoBehaviour
    {
        [Serializable]
        private struct DictElement<TKey, TValue>
        {
            public TKey key;

            public TValue data;
        }


        [Serializable]
        public class CharacterPosition
        {
            public string name;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
        }


        [SerializeField]
        private List<DictElement<string, Sprite>> emotions;
        private Dictionary<string, Sprite> emotionsDict;

        [SerializeField]
        private List<CharacterPosition> positions;
        private Dictionary<string, CharacterPosition> positionsDict;


        // Start is called before the first frame update
        void Awake()
        {
            // Unpack the character's emotions/sprites
            emotionsDict = new Dictionary<string, Sprite>();
            foreach (DictElement<string, Sprite> emotion in emotions)
                emotionsDict[emotion.key.ToLower()] = emotion.data;

            // Unpack the character's positions to move to
            positionsDict = new Dictionary<string, CharacterPosition>();
            foreach (CharacterPosition pos in positions)
                positionsDict[pos.name.ToLower()] = pos;
        }


        /// <summary>
        /// Sets this Character's sprite according to the given emotion.
        /// </summary>
        /// <param name="emotionName"> The name of the emotion. </param>
        [YarnCommand("emotion")]
        public void ChangeEmotion(String emotionName)
        {
            // Error Case: Emotion is undefined.
            if (!emotionsDict.ContainsKey(emotionName.ToLower()))
            {
                Debug.LogWarning(name + " does not contains the emotion " + emotionName + ".");
                return;
            }

            // Change the displayed sprite.
            GetComponent<SpriteRenderer>().sprite = emotionsDict[emotionName.ToLower()];
        }


        /// <summary>
        /// Moves/translates this Character to a pre-specified position.
        /// </summary>
        /// <param name="positionName"> Name of the position to move to. </param>
        /// <param name="time"> How long the move should take. </param>
        [YarnCommand("move")]
        public void Move(String positionName, float time = 0.5f)
        {
            // Error Case: Specified position is undefined.
            if (!positionsDict.ContainsKey(positionName.ToLower()))
            {
                Debug.LogWarning(name + " does not have the position " + positionName + ".");
                return;
            }

            // Animate the movement
            transform.DOMove(positionsDict[positionName.ToLower()].position, time);
            transform.DORotate(positionsDict[positionName.ToLower()].rotation, time);
            transform.DOScale(positionsDict[positionName.ToLower()].scale, time);
        }
    }
}
