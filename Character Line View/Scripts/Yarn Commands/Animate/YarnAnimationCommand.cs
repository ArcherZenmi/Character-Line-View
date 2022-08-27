/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file contains the "animation" command to be used in YarnScript.
///    Simply pass in the name of an animation in a YarnScript file,
///    and this YarnAnimationCommand will handle displaying it.
///
///    Note: The YarnAnimationHandler to pass the animation onto must be
///    set externally. In most cases, this is done automatically by
///    the YarnAnimationHandler itself.
///    Furthermore, the YarnAnimationHandler must contain a TimeLine
///    object corresponding to the animation's name.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Yarn.Unity;

namespace CharacterLineView
{
    public class YarnAnimationCommand : MonoBehaviour
    {
        private static YarnAnimationHandler currentAnimationHandler;


        /// <summary>
        /// An "animate" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="animationName"> The name of the animation to be played. </param>
        /// <param name="async"> True if dialogue should continue during the animation, 
        ///     false if dialogue should wait for animation to finish. </param>
        [YarnCommand("animate")]
        public static IEnumerator Animate(string animationName, bool async = false)
        {
            // Error case: No handler for the "animate" command may be set.
            if (currentAnimationHandler == null)
            {
                Debug.LogError("No YarnAnimationHandler has been set. The \"animate\" command in Yarnscript cannot run.");
                yield break;
            }

            yield return currentAnimationHandler.PlayAnimation(animationName, async);
        }


        /// <summary>
        /// Set the current YarnAnimationHandler object that the "animate" command will be passed to.
        /// This should not be called directly, rather, it should be called by a YarnAnimationHandler.
        /// </summary>
        /// <param name="newHandler"> The new YarnAnimationHandler. </param>
        public static void SetCurrentAnimationHandler(YarnAnimationHandler newHandler)
        {
            currentAnimationHandler = newHandler;
        }
    }
}