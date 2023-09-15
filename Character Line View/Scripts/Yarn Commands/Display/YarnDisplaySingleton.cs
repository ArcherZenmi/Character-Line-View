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
    public class YarnDisplaySingleton : MonoBehaviour
    {
        private static YarnDisplayHandler currentHandler;


        /// <summary>
        /// A "hide" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The time it takes to hide the display. </param>
        [YarnCommand("hide_display")]
        public static void Hide(float time = -1)
        {
            // Error case: No handler was set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"hide_display\" command in Yarnscript cannot run.");
                return;
            }

            // Use specified time
            if (time != -1)
                currentHandler.Hide(time);
            // Use default value
            else
                currentHandler.Hide();
        }


        /// <summary>
        /// A "show" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The time it takes to show the display. </param>
        [YarnCommand("show_display")]
        public static void Show(float time = -1)
        {
            // Error case: No handler was set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"show_display\" command in Yarnscript cannot run.");
                return;
            }

            // Use specified time
            if (time != -1)
                currentHandler.Show(time);
            // Use default value
            else
                currentHandler.Show();
        }


        /// <summary>
        /// A "set hide time" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The new default time it takes to hide the display. </param>
        [YarnCommand("set_hide_display_time")]
        public static void SetHideTime(float time)
        {
            // Error case: No handler was set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"show_display\" command in Yarnscript cannot run.");
                return;
            }

            // Send info
            currentHandler.SetHideTime(time);
        }


        /// <summary>
        /// A "set show time" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The new default time it takes to show the display. </param>
        [YarnCommand("set_show_display_time")]
        public static void SetShowTime(float time)
        {
            // Error case: No handler was set.
            if (currentHandler == null)
            {
                Debug.LogError("No handler has been set. The \"show_display\" command in Yarnscript cannot run.");
                return;
            }

            // Send info
            currentHandler.SetShowTime(time);
        }


        /// <summary>
        /// Set the current handler object that the command will be passed to.
        /// This should not be called directly, rather, it should be called by a handler.
        /// </summary>
        /// <param name="newHandler"> The new handler object. </param>
        public static void SetCurrentHandler(YarnDisplayHandler newHandler)
        {
            currentHandler = newHandler;
        }
    }
}