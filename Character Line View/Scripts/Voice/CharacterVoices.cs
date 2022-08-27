/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file serves as a public access point to the AudioSource for character's voices,
///    intended to be used from CharacterLineView or any in-text attributes.
///
///    CharacterVoices handles playing a character's "voice clip" on loop at default speeds,
///    or at specific speeds if needed.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterLineView
{
    public class CharacterVoices : MonoBehaviour
    {
        [Serializable]
        private struct Voice
        {
            public string name;

            public AudioClip audio;
        }

        [SerializeField]
        private Voice[] voices;

        private AudioSource audioSource;
        private Dictionary<string, AudioClip> voicesDict;

        public bool Mute
        {
            get { return audioSource.mute; }
            set { audioSource.mute = value; }
        }

        /// <summary>
        /// This method is called when this GameObject is first initialized.
        /// </summary>
        void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            Debug.Assert(audioSource != null, "The CharacterVoices class requires this GameObject to have an AudioSource component.");

            // Create the dictionary of voices associated with each characters name
            voicesDict = new Dictionary<string, AudioClip>();
            foreach (Voice voice in voices)
                voicesDict.Add(voice.name, voice.audio);
        }


        /// <summary>
        /// Plays the voice for the current character on loop.
        /// SetVoice must have been previously called with a character's name.
        /// </summary>
        public void PlayVoice()
        {
            // Error case: No character is specified
            if(audioSource.clip == null)
            {
                Debug.LogWarning("No character's voice is specified. No audio is played.");
                return;
            }

            StopVoice();

            audioSource.loop = true;
            audioSource.Play();
        }


        /// <summary>
        /// Set's the current voice used by this AudioSource via the character's name.
        /// </summary>
        /// <param name="nameOfCharacter"> Character's name to use the voice of. </param>
        /// <returns> True if voice was changed, false otherwise. </returns>
        public bool SetVoice(string nameOfCharacter)
        {
            // Error case: The character for the voice doesn't exist
            if (!voicesDict.ContainsKey(nameOfCharacter))
            {
                Debug.LogWarning($"A voice for the character \"{nameOfCharacter}\" was not found. No change to current voice was made.");
                return false;
            }

            // Set the voice
            audioSource.clip = voicesDict[nameOfCharacter];

            return true;
        }


        /// <summary>
        /// Stops any voices that are currently playing.
        /// </summary>
        public void StopVoice()
        {
            audioSource.loop = false;
            StopCoroutine("PlayVoiceTimerCoroutine");
        }


        /// <summary>
        /// Play's the current character's voice "voicesPerSecond" times per second.
        /// </summary>
        /// <param name="voicesPerSecond"> How many voice clips to play a second. </param>
        public void PlayVoiceTimer(int voicesPerSecond)
        {
            StopVoice();

            StartCoroutine("PlayVoiceTimerCoroutine", voicesPerSecond);
        }


        /// <summary>
        /// A coroutine that play's the current voice at regular intervals specified by "voicesPerSecond".
        /// </summary>
        /// <param name="voicesPerSecond"> How many voice clips to play a second. </param>
        /// <returns> An IEnumerator, which tells Unity this method can be used as a coroutine. </returns>
        private IEnumerator PlayVoiceTimerCoroutine(int voicesPerSecond)
        {
            float voicePlayTime = 1.0f / voicesPerSecond;

            while(true)
            {
                audioSource.PlayOneShot(audioSource.clip);

                yield return new WaitForSeconds(voicePlayTime);
            }
        }
    }
}