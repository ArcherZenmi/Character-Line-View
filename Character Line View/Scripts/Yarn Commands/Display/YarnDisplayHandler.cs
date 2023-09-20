/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file contains the "animation" command to be used in YarnScript.
///    It's not called directly by YarnScript, rather, this is called by
///    YarnAnimationCommand whenever this game object/handler is set
///    as the "current animation handler" (see YarnAnimationCommand for details).
///
///     Simply pass in a list of TimelineAssets, then call PlayAnimation() to play it.
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yarn.Unity;

namespace CharacterLineView
{
    public class YarnDisplayHandler : MonoBehaviour
    {
        [SerializeField]
        private bool setAsCurrentHandler = false;

        [SerializeField]
        private CharacterLineView display;
        private CanvasGroup canvasGroupComp;

        [SerializeField]
        private float defaultShowTime;
        [SerializeField]
        private float defaultHideTime;

        private Tween animateGUI;

        public bool Hidden
        {
            get;
            private set;
        }


        /// <inheritdoc/>
        /// <summary>
        /// Set this AnimationHandler as the main one to be used by YarnScript,
        /// as well as initialize some internally used variables.
        /// </summary>
        public void Start()
        {
            // Error Case: Display isn't set
            if (display == null)
                Debug.LogError("A YarnDisplayHandler MUST have a reference to a CharacterLineView GameObject.");

            // Find the CanvasGroup component to modify
            canvasGroupComp = display.GetComponent<CanvasGroup>();
            if (canvasGroupComp == null)
                Debug.LogError("CharacterLineView does not have a CanvasGroup defined.");

            // Set this as the main animation handler for YarnScript
            if(setAsCurrentHandler)
                SetAsCurrentHandler();

            // In the beginning, the display is hidden
            Hidden = false;
            Hide(0);
        }


        /// <summary>
        /// A "set hide time" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The new default time it takes to hide the display. </param>
        public void SetHideTime(float time)
        {
            defaultHideTime = time;
        }


        /// <summary>
        /// A "set show time" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The new default time it takes to show the display. </param>
        public void SetShowTime(float time)
        {
            defaultShowTime = time;
        }


        /// <summary>
        /// A "hide" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The time it takes to hide the display. </param>
        /// <returns> How long in seconds the animation will take. </returns>
        public float Hide(float? time = null)
        {
            // Edge Case: Dialogue is already hidden
            if (Hidden)
            {
                Debug.Log("The Character Line View Gameobject is already hidden. The call to Hide() will be skipped.");
                return 0.0f;
            }

            Hidden = true;

            // If no time was specified, use the default
            if (time == null)
                time = defaultHideTime;


            // A tween that slowly hides this gameObject
            animateGUI = DOTween.To(
                () => canvasGroupComp.alpha,
                (value) => canvasGroupComp.alpha = value,
                0.0f, (float)time);
            CharacterLineViewGlobals.ApplyTweenDefaultSettings(animateGUI);

            animateGUI.Play();

            return animateGUI.Duration();
        }


        /// <summary>
        /// A "show" command usable when writing Yarn Script.
        /// </summary>
        /// <param name="time"> The time it takes to show the display. </param>
        public float Show(float? time = null)
        {
            // Edge Case: Display is already shown
            if (!Hidden)
            {
                Debug.Log("The Character Line View Gameobject is already shown. The call to Show() will be skipped.");
                return 0.0f;
            }

            // If no time was specified, use the default
            if (time == null)
                time = defaultShowTime;

            // A tween that slowly shows this gameObject
            animateGUI = DOTween.To(
                () => canvasGroupComp.alpha,
                (value) => canvasGroupComp.alpha = value,
                1.0f, (float)time);
            CharacterLineViewGlobals.ApplyTweenDefaultSettings(animateGUI);

            // Once the GUI is fully shown, it should become interactable again
            animateGUI.OnComplete(() => Hidden = false);

            animateGUI.Play();

            return animateGUI.Duration();
        }


        /// <summary>
        /// Checks if the display is available to animate or not,
        /// i.e if another animation is already playing.
        /// </summary>
        /// <returns> True if a new animation can be played, false otherwise. </returns>
        public bool CanPlay()
        {
            return !(animateGUI.IsActive() && animateGUI.IsPlaying());
        }


        /// <summary>
        /// Sets this YarnAnimationHandler as the one used by YarnAnimationCommand.
        /// That's to say, any animations passed in by YarnScript will be
        /// handled by this object.
        /// </summary>
        public void SetAsCurrentHandler()
        {
            YarnDisplaySingleton.SetCurrentHandler(this);
        }
    }
}