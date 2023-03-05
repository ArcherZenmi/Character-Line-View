/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file serves as the bridge between YarnScript's dialogue,
///    and actually displaying that onto a GUI in Unity.
///
///     CharacterLineView is a LineView that allows for...
///     1) In-text commands specify pauses, cps, and other stuff for the dialogue
///         (this also supports RichText simultaneously)
///     2) Changing backgrounds for whatever character is speaking
///     3) Icons w/ emotions displayed alongside a character
///     4) A looping "voice" effect to play for each character while they speak
///    
/// </summary>

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Yarn;
using Yarn.Markup;
using Yarn.Unity;

namespace CharacterLineView
{
    public class CharacterLineView : DialogueViewBase
    {
        public NameTextBox nameTextBox;
        public CharacterVoices characterVoices;
        public TextLineProvider lineProvider;
        public TextBox textBox;

        public float fadeInTime;
        public float fadeOutTime;

        // True when dialogue box is hidden/uninteractable, false when shown/interactable.
        private bool Hidden
        {
            get;
            set;
        }


        /// <summary>
        /// This method is called when this GameObject is loaded into the scene (specifically when the script is first enabled).
        /// </summary>
        public void Start()
        {
            // Hide the dialogue box so it's uninteractable.
            Hidden = true;
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
        }


        /// <summary>
        /// Called whenever the current dialogue finishes and no more lines will be delivered.
        /// </summary>
        public override void DialogueComplete()
        {
            // Hide the dialogue view, as it will no longer be used.
            Hide();
        }


        /// <summary>
        /// This method is called by DialogueRunner to get rid of the current line, and (potentially) prepare for a new one.
        /// </summary>
        /// <param name="onDismissalComplete"> A callback once the dialogue has been deleted. </param>
        public override void DismissLine(Action onDismissalComplete)
        {
            // Prepare the textbox for the next character
            textBox.DismissLine();

            // Tell YarnSpinner that we're ready for the next line.
            onDismissalComplete();
        }


        /// <summary>
        /// Hides this Line View via a fade effect.
        /// </summary>
        /// <returns> A float value representing how long this animation/process will take. </returns>
        public float Hide()
        {
            // Edge Case: Dialogue is already hidden
            if(Hidden)
            {
                Debug.Log("The Character Line View Gameobject is already hidden. The call to Hide() will be skipped.");
                return 0.0f;
            }

            Hidden = true;

            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();

            // A tween that slowly hides this gameObject
            Tween animateGUI = DOTween.To(
                () => canvasGroup.alpha,
                (value) => canvasGroup.alpha = value,
                0.0f, fadeOutTime);
            CharacterLineViewGlobals.ApplyTweenDefaultSettings(animateGUI);

            // Fully show the GUI, so that the full hide effect can be seen
            canvasGroup.alpha = 1.0f;

            animateGUI.Play();

            return animateGUI.Duration();
        }


        /// <summary>
        /// This method is meant to finish displaying the line as quickly as possible.
        /// That is to say it immediately display all the text
        /// and ignores any in-text pauses and "type writer" effects.
        /// </summary>
        /// <param name="dialogueLine"> The LocalizedLine to show the player. </param>
        /// <param name="onDialogueLineFinished"> A callback once the dialogue has finished presenting. </param>
        public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            textBox.CompleteAnimation();

            onDialogueLineFinished();
        }


        /// <summary>
        /// This method is called whenever a new line is recieved from the DialogueRunner.
        /// </summary>
        /// <param name="dialogueLine"> The LocalizedLine to show the player. </param>
        /// <param name="onDialogueLineFinished"> A callback once the dialogue has finished presenting. </param>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // If the dialogue box isn't displayed, make sure to account for its delay
            float delayTime = 0;
            if (Hidden)
                delayTime = Show();

            // Activate any changes to the GUI that child objects may want to do
            UpdateChildren(dialogueLine);

            // Display all text
            nameTextBox.ChangeName(dialogueLine.CharacterName);
            if (delayTime > 0) // If necessary, animate the text after a delay
            {
                Sequence sequence = DOTween.Sequence();
                CharacterLineViewGlobals.ApplyTweenDefaultSettings(sequence);
                sequence.AppendInterval(delayTime);
                sequence.AppendCallback(new TweenCallback(
                    () => textBox.AnimateAndDisplay(dialogueLine, CharacterLineViewGlobals.defaultCpsDict[lineProvider.textLanguageCode], UserRequestedViewAdvancement)
                ));
                sequence.Play();
            }
            else
                textBox.AnimateAndDisplay(dialogueLine, CharacterLineViewGlobals.defaultCpsDict[lineProvider.textLanguageCode], UserRequestedViewAdvancement);
        }

        /// <summary>
        /// Shows this Line View via a fade effect.
        /// </summary>
        /// <returns> A float value representing how long this animation/process will take. </returns>
        public float Show()
        {
            // Edge Case: Dialogue is already shown
            if (!Hidden)
            {
                Debug.Log("The Character Line View Gameobject is already shown. The call to Show() will be skipped.");
                return 0.0f;
            }

            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();

            // A tween that slowly shows this gameObject
            Tween animateGUI = DOTween.To(
                () => canvasGroup.alpha,
                (value) => canvasGroup.alpha = value,
                1.0f, fadeInTime);
            CharacterLineViewGlobals.ApplyTweenDefaultSettings(animateGUI);

            // Once the GUI is fully shown, it should become interactable again
            animateGUI.OnComplete(() => Hidden = false);

            // Fully hide the GUI, so that the full hide effect can be seen
            canvasGroup.alpha = 0.0f;

            animateGUI.Play();

            return animateGUI.Duration();
        }


        /// <summary>
        /// Called by DialogueAdvanceScript whenever the user presses space, or some equivalent.
        /// If dialogue is displaying, skips it. If dialogue has finished displaying, goes to the next.
        /// </summary>
        public override void UserRequestedViewAdvancement()
        {
            // If no line is currently being processed, skip this call
            if (Hidden)
                return;

            // If the line is still presenting, skip to the end.
            else if (textBox.IsPlaying())
                textBox.CompleteAnimation();

            // If the line has already finished presenting, dismiss it and go to the next line.
            else
                requestInterrupt.Invoke();
        }


        /// <summary>
        /// Activate any changes to the GUI that child objects
        ///     (only those that inherit from ICharacterDisplayable) may want to do.
        /// </summary>
        /// <param name="dialogueLine"> The current LocalizedLine. </param>
        private void UpdateChildren(LocalizedLine dialogueLine)
        {
            foreach (Transform child in transform)
            {
                ICharacterDisplayable displayable = child.gameObject.GetComponent(typeof(ICharacterDisplayable)) as ICharacterDisplayable;
                if (displayable == null)
                    continue;

                displayable.SetDisplayable(dialogueLine);
            }
        }
    }
}