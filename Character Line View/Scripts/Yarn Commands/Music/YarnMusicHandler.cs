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
    public class YarnMusicHandler : MonoBehaviour
    {
        /// <summary>
        /// A helper class used to serialize dictionary-like pairs.
        /// </summary>
        [Serializable]
        private struct DictElement<TKey, TValue>
        {
            public TKey key;

            public TValue data;
        }

        /// <summary>
        /// True if this object should set itself as the default handler in Start().
        /// False otherwise.
        /// </summary>
        [SerializeField]
        private bool setAsCurrentHandler = false;

        [SerializeField]
        private List<DictElement<string, AudioClip>> musicList;
        private Dictionary<string, AudioClip> musicDict;

        /// <summary>
        /// Reference to the AudioSource component in this GameObject.
        /// </summary>
        private AudioSource audioSource;

        private Sequence audioFader;


        /// <inheritdoc/>
        /// <summary>
        /// Set this AnimationHandler as the main one to be used by YarnScript,
        /// as well as initialize some internally used variables.
        /// </summary>
        public void Start()
        {
            // Set this as the main animation handler for YarnScript
            if (setAsCurrentHandler)
                YarnMusicSingleton.SetCurrentHandler(this);

            // Find the AudioSource on this game object
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
                Debug.LogError("This GameObject must have an AudioSource component.");

            // Convert the array of music pieces to a dictionary
            musicDict = new Dictionary<string, AudioClip>();
            foreach (DictElement<string, AudioClip> music in musicList)
                musicDict.Add(music.key, music.data);

            // Create an empty Sequence
            audioFader = DOTween.Sequence();
        }


        /// <summary>
        /// Sets this YarnAnimationHandler as the one used by YarnAnimationCommand.
        /// That's to say, any animations passed in by YarnScript will be
        /// handled by this object.
        /// </summary>
        public void SetAsCurrentHandler()
        {
            YarnMusicSingleton.SetCurrentHandler(this);
        }


        /// <summary>
        /// Play a new music and swap out the old (if any exist).
        /// </summary>
        /// <param name="musicName"> The new music to play. </param>
        /// <param name="fadeIn"> How many seconds to fade in the volume for the new music. </param>
        /// <param name="fadeOut"> How many seconds to fade out the volume for the old music. </param>
        public void MusicNew(string musicName, float fadeIn = 0.0f, float fadeOut = 0.0f)
        {
            // Error Case: Music of given name doesn't exist
            if(!musicDict.ContainsKey(musicName))
            {
                Debug.LogError($"No music titled \"{musicName}\" exists. No action taken.");
                return;
            }

            // Edge Case: Any unfinished fading should be immediately forced to finish
            if (audioFader.IsActive() && audioFader.IsPlaying())
                audioFader.Complete(true);

            // Set up a sequence
            audioFader = DOTween.Sequence();
            audioFader.SetEase(Ease.Linear);

            // Fade out the volume
            audioFader.Append(DOTween.To(
                () => { return audioSource.volume; },
                (val) => { audioSource.volume = val; },
                0.0f,
                fadeOut));

            // Change the music
            audioFader.AppendCallback(new TweenCallback(() => {
                audioSource.Stop();
                audioSource.clip = musicDict[musicName];
                audioSource.Play();
                }));

            // Fade in the volume
            audioFader.Append(DOTween.To(
                () => { return audioSource.volume; },
                (val) => { audioSource.volume = val; },
                1.0f,
                fadeIn));
        }

        /// <summary>
        /// Resume music if it's paused.
        /// </summary>
        /// <param name="fadeIn"> How many seconds to fade in the volume for. </param>
        public void MusicResume(float fadeIn = 0.0f)
        {
            // Error Case: No music exists
            if(audioSource.clip == null)
            {
                Debug.LogError("No audio clip exists, cannot resume music.");
                return;
            }

            // Edge Case: Music is already playing
            if(audioSource.isPlaying)
            {
                Debug.LogWarning("Music is already playing, no action taken.");
                return;
            }

            // Edge Case 2: Any unfinished fading should be immediately forced to finish
            if (audioFader.IsActive() && audioFader.IsPlaying())
                audioFader.Complete(true);

            // Begin the music with no volume
            audioSource.volume = 0.0f;
            audioSource.UnPause();

            // Set up a sequence
            audioFader = DOTween.Sequence();
            audioFader.SetEase(Ease.Linear);

            // Fade in the volume
            audioFader.Append(DOTween.To(
                () => audioSource.volume,
                (val) => audioSource.volume = val,
                1.0f,
                fadeIn));
        }

        /// <summary>
        /// Pauses the music.
        /// </summary>
        /// <param name="fadeOut"> How many seconds to fade out the volume for. </param>
        public void MusicPause(float fadeOut = 0.0f)
        {
            // Edge Case: No music is playing
            if (!audioSource.isPlaying)
            {
                Debug.LogWarning("Music is already paused, no action taken.");
                return;
            }

            // Edge Case 2: Any unfinished fading should be immediately forced to finish
            if (audioFader.IsActive() && audioFader.IsPlaying())
                audioFader.Complete(true);

            // Set up a sequence
            audioFader = DOTween.Sequence();
            audioFader.SetEase(Ease.Linear);

            // Fade out the volume
            audioFader.Append(DOTween.To(
                () => audioSource.volume,
                (val) => audioSource.volume = val,
                0.0f,
                fadeOut));

            // At the end of the fading, pause the music
            audioFader.OnComplete(new TweenCallback(() => audioSource.Pause()));
        }
    }
}
