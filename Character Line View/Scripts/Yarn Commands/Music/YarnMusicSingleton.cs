/// <summary> 
/// Author:    Archer Zenmi
/// Package:   N/A
/// 
/// File Contents 
/// 
///     This class creates any music related YarnScript commands,
///         and passes command calls to a handler.
///
///     (Currently this is NOT part of the standard CLV package,
///         but hopefully it will be soon).
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace CharacterLineView
{
    public static class YarnMusicSingleton
    {
        private static YarnMusicHandler currentHandler;

        /// <summary>
        /// Set the current handler object that the command will be passed to.
        /// This should not be called directly, rather, it should be called by a handler GameObject.
        /// </summary>
        /// <param name="newHandler"> The new handler. </param>
        public static void SetCurrentHandler(YarnMusicHandler newHandler)
        {
            currentHandler = newHandler;
        }


        /// <summary>
        /// Play a new music and swap out the old (if any exist).
        /// </summary>
        /// <param name="musicName"> The new music to play. </param>
        /// <param name="fadeIn"> How many seconds to fade in the volume for the new music. </param>
        /// <param name="fadeOut"> How many seconds to fade out the volume for the old music. </param>
        [YarnCommand("MusicNew")]
        public static void MusicNew(string musicName, float fadeIn = 0.0f, float fadeOut = 0.0f)
        {
            // Error case: No handler exists set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"MusicPlay\" command in Yarnscript cannot run.");
                return;
            }

            currentHandler.MusicNew(musicName, fadeIn, fadeOut);
        }

        /// <summary>
        /// Resume music if it's paused.
        /// </summary>
        /// <param name="fadeIn"> How many seconds to fade in the volume for. </param>
        [YarnCommand("MusicResume")]
        public static void MusicResume(float fadeIn = 0.0f)
        {
            // Error case: No handler exists set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"MusicPlay\" command in Yarnscript cannot run.");
                return;
            }

            currentHandler.MusicResume(fadeIn);
        }

        /// <summary>
        /// Pauses the music.
        /// </summary>
        /// <param name="fadeOut"> How many seconds to fade out the volume for. </param>
        [YarnCommand("MusicPause")]
        public static void MusicPause(float fadeOut = 0.0f)
        {
            // Error case: No handler exists set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"MusicPause\" command in Yarnscript cannot run.");
                return;
            }

            currentHandler.MusicPause(fadeOut);
        }
    }
}
