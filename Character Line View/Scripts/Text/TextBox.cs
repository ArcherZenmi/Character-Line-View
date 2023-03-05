/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///     This file specifies the behavior of the text box in Character Line View.
///
///     It specifies what actual names should be displayed for the character tags
///         used in YarnScript.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using Yarn;
using Yarn.Markup;
using Yarn.Unity;

namespace CharacterLineView
{
    public partial class TextBox : MonoBehaviour
    {
        [SerializeField]
        private CharacterVoices characterVoices;

        private TextMeshProUGUI textComponent;
        private Sequence currentAnimation;
        // Must be used ONLY to strip attributes/commands from a string.
        // Unity's debugger will scream at you, but just do it. Using a dedicated Dialogue object is more bug-proof.
        private Dialogue yarnScriptParser;

        // Start is called before the first frame update
        void Start()
        {
            // Find component
            textComponent = GetComponent<TextMeshProUGUI>();
            textComponent.text = String.Empty;

            // Do an illegal, and initialize yarnScriptParser
            yarnScriptParser = new Dialogue(new InMemoryVariableStorage());
        }


        /// <summary>
        /// Animate the textbox to display a given line from YarnScript.
        /// </summary>
        /// <param name="dialogueLine"> The LocalizedLine to display. </param>
        /// <param name="startingCps"> The cps to start with. </param>
        /// <param name="requestViewAdvancement"> An action to call to cause a view advancement. </param>
        public void AnimateAndDisplay(LocalizedLine dialogueLine, int startingCps, Action requestViewAdvancement)
        {
            // Take the LocalizedLine, and create two versions for yarn (rich text attributes removed),
            // and for textMeshPro (yarn attributes removed).
            string yarnLine;
            string textMeshProLine;
            SeparateInTextCommands(dialogueLine, out yarnLine, out textMeshProLine);

            // Set the text to display
            textComponent.text = textMeshProLine;
            //textComponent.ForceMeshUpdate(); // Forcing an update here removes possible errors further down the pipeline

            // Second, extract the markUp from the yarn lines
            MarkupParseResult markUp = ExtractMarkup(yarnLine);

            // Finally, use the markUp to set up the DOTween sequencer for the text's "type writer" effect
            currentAnimation = AnimateDialogueLine(markUp, startingCps, requestViewAdvancement);

            // Set up the voices
            currentAnimation = AddVoicesToSequence(dialogueLine, currentAnimation, dialogueLine.CharacterName);

            // Play the animation
            currentAnimation.Play();
        }


        /// <summary>
        /// Remove the current line (usually to make space for another).
        /// </summary>
        public void DismissLine()
        {
            // Remove visible text
            textComponent.text = String.Empty;
            textComponent.maxVisibleCharacters = 0;

            // Reset the Sequence
            currentAnimation.Kill();
        }


        /// <summary>
        /// If any animation is currently playing, forces it to complete immediately.
        /// </summary>
        public void CompleteAnimation()
        {
            // Edge Case: No animation is playing
            if(!IsPlaying())
            {
                Debug.LogWarning("Cannot complete animation while no animation is playing.");
                return;
            }

            // Complete the animation
            currentAnimation.Complete(true);
        }


        /// <summary>
        /// Returns whether an animation is currently playing.
        /// </summary>
        /// <returns> True if animation is playing, false otherwise. </returns>
        public bool IsPlaying()
        {
            if (currentAnimation == null)
                return false;

            return currentAnimation.IsPlaying();
        }
    }
}