/// <summary> 
/// Author:    Archer Zenmi
/// Package:   N/A
/// 
/// File Contents 
/// 
///     This class can be attatched to a GameObject
///         to handle any music related YarnScript commands.
///
///     (Currently this is NOT part of the standard CLV package,
///         but hopefully it will be soon).
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yarn.Unity;

namespace CharacterLineView
{
    public class YarnSoundHandler : MonoBehaviour
    {
        /// <summary>
        /// A helper class used to serialize dictionary-like pairs.
        /// </summary>
        [Serializable]
        private struct DictElement<TKey, TValue>
        {
            public TKey name;

            public TValue audioClip;
        }

        /// <summary>
        /// True if this object should set itself as the default handler in Start().
        /// False otherwise.
        /// </summary>
        [SerializeField]
        private bool setAsCurrentHandler = false;

        [SerializeField]
        private List<DictElement<string, AudioClip>> soundList;
        private Dictionary<string, AudioClip> soundDict;

        /// <summary>
        /// Reference to the AudioSource component in this GameObject.
        /// </summary>
        private AudioSource audioSource;


        /// <inheritdoc/>
        /// <summary>
        /// Set this AnimationHandler as the main one to be used by YarnScript,
        /// as well as initialize some internally used variables.
        /// </summary>
        public void Start()
        {
            // Set this as the main animation handler for YarnScript
            if (setAsCurrentHandler)
                YarnSoundSingleton.SetCurrentHandler(this);

            // Find the AudioSource on this game object
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
                Debug.LogError("This GameObject must have an AudioSource component.");

            // Convert the array of sounds to a dictionary
            soundDict = new Dictionary<string, AudioClip>();
            foreach (DictElement<string, AudioClip> sound in soundList)
                soundDict.Add(sound.name, sound.audioClip);
        }


        /// <summary>
        /// Play a new sound and cancel the old (if any exist).
        /// </summary>
        /// <param name="name"> The sound to play. </param>
        /// <param name="volume"> The volume to play at. </param>
        public void Play(string name, float volume = 1.0f)
        {
            // Error Case: Sound of given name doesn't exist
            if (!soundDict.ContainsKey(name))
            {
                Debug.LogError($"No sound titled \"{name}\" exists. No action taken.");
                return;
            }

            // Edge Case: Any unfinished sound should be immediately forced to finish
            if (audioSource.isPlaying)
                audioSource.Stop();

            // Play the sound
            audioSource.clip = soundDict[name];
            audioSource.Play();
        }


        /// <summary>
        /// Stops the current sound.
        /// </summary>
        public void Stop()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }


        /// <summary>
        /// Sets this Handler as the one used by the Singleton.
        /// That's to say, any relevant commands passed in by YarnScript
        /// will be handled by this object.
        /// </summary>
        public void SetAsCurrentHandler()
        {
            YarnSoundSingleton.SetCurrentHandler(this);
        }
    }
}
