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
    public static class YarnSoundSingleton
    {
        private static YarnSoundHandler currentHandler;

        /// <summary>
        /// Set the current handler object that the command will be passed to.
        /// This should not be called directly, rather, it should be called by a handler GameObject.
        /// </summary>
        /// <param name="newHandler"> The new handler. </param>
        public static void SetCurrentHandler(YarnSoundHandler newHandler)
        {
            currentHandler = newHandler;
        }


        /// <summary>
        /// Play a new sound and cancel the old (if any exist).
        /// </summary>
        /// <param name="name"> The sound to play. </param>
        /// <param name="volume"> The volume to play at. </param>
        [YarnCommand("sound_play")]
        public static void Play(string name, float volume = 1.0f)
        {
            // Error case: No handler exists set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"Sound Play\" command in Yarnscript cannot run.");
                return;
            }

            currentHandler.Play(name, volume);
        }


        /// <summary>
        /// Stops the current sound.
        /// </summary>
        [YarnCommand("sound_stop")]
        public static void Stop()
        {
            // Error case: No handler exists set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"Sound Play\" command in Yarnscript cannot run.");
                return;
            }

            currentHandler.Stop();
        }
    }
}
