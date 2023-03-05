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
        public NameTextBox nameText;
        public CharacterVoices characterVoices;
        public TextLineProvider lineProvider;
        public TextMeshProUGUI lineText;

        public float fadeInTime;
        public float fadeOutTime;

        // Must be used ONLY to strip attributes/commands from a string.
        // Unity's debugger will scream at you, but just do it. Using a dedicated Dialogue object is more bug-proof.
        private Dialogue yarnScriptParser;

        // The current Sequence used to animate the dialogue
        private Sequence animatedDialogueSequence;

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

            // Do an illegal, and initialize yarnScriptParser
            yarnScriptParser = new Dialogue(new InMemoryVariableStorage());
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
            // Delete the Sequence so a new one can take it's place when a new line comes in
            animatedDialogueSequence.Kill();

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
            animatedDialogueSequence.Complete(true);

            onDialogueLineFinished();
        }


        /// <summary>
        /// This method is called whenever a new line is recieved from the DialogueRunner.
        /// </summary>
        /// <param name="dialogueLine"> The LocalizedLine to show the player. </param>
        /// <param name="onDialogueLineFinished"> A callback once the dialogue has finished presenting. </param>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // First, hide all the text that's displayed while we do the math
            lineText.maxVisibleCharacters = 0;

            // Activate any changes to the GUI that child objects may want to do
            UpdateChildren(dialogueLine);

            // Take the LocalizedLine, and create two versions for yarn (rich text attributes removed),
            // and for textMeshPro (yarn attributes removed).
            string yarnLine;
            string textMeshProLine;
            SeparateInTextCommands(dialogueLine, out yarnLine, out textMeshProLine);

            // First, display the textMeshPro lines
            nameText.ChangeName(dialogueLine.CharacterName);
            lineText.text = textMeshProLine;
            lineText.ForceMeshUpdate(); // Forcing an update here removes possible errors further down the pipeline

            // Second, extract the markUp from the yarn lines
            MarkupParseResult markUp = ExtractMarkup(yarnLine);

            // Finally, use the markUp to set up the DOTween sequencer for the text's "type writer" effect
            animatedDialogueSequence = AnimateDialogueLine(lineText, markUp);

            // Set up the voices
            animatedDialogueSequence = AddVoicesToSequence(dialogueLine, animatedDialogueSequence, dialogueLine.CharacterName);

            // If the dialogue box isn't displayed, make sure to display it before the animation begins
            if(Hidden)
            {
                float showTime = Show();
                animatedDialogueSequence.PrependInterval(showTime);
            }

            // Play the animation
            animatedDialogueSequence.Play();
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
            if (Hidden || animatedDialogueSequence == null)
                return;

            // If the line is still presenting, skip to the end.
            else if (animatedDialogueSequence.IsPlaying())
                animatedDialogueSequence.Complete(true);

            // If the line has already finished presenting, dismiss it and go to the next line.
            else
                requestInterrupt.Invoke();
        }


        /// <summary>
        /// Starts up the audio for a character's voice
        /// at the current start position of the sequence (prepended, NOT set for OnPlay),
        /// and sets it to stop at the end of the sequence (set for OnComplete).
        /// </summary>
        /// <param name="dialogueLine"> The LocalizedLine that voices are being added to. </param>
        /// <param name="dialogueSequence"> The sequence to add voices to. </param>
        /// <param name="characterName"> The name of the speaking character. </param>
        /// <returns> The Sequence after adding audio. </returns>
        private Sequence AddVoicesToSequence(LocalizedLine dialogueLine, Sequence dialogueSequence, String characterName)
        {
            // Check whether the "no_voice" tag is added, and mute the audio if necessary
            // Do NOT skip the code afterwards just because it gets muted. You don't know what the dialogueLine's attributes are.
            if(dialogueLine.Metadata == null || !dialogueLine.Metadata.Contains("no_voice"))
                characterVoices.Mute = false;
            else
                characterVoices.Mute = true;

            // Set the audio to the proper voice
            characterVoices.SetVoice(characterName);

            // Start up the audio
            dialogueSequence.PrependCallback(
                new TweenCallback(() => characterVoices.PlayVoice()));

            // Set the audio to end after the sequence
            dialogueSequence.OnComplete(
                new TweenCallback(() => characterVoices.StopVoice()));

            return dialogueSequence;
        }


        /// <summary>
        /// This will take in a TextMeshProUGUI, and a MarkupParseResult that specifies its animation.
        /// It'll set up the Sequence that'll animate the TextMeshProUGUI.
        /// </summary>
        /// <param name="textToAnimate"> The TextMeshProUGUI to animate. </param>
        /// <param name="markUp"> The MarkupParseResult specifying the animation. </param>
        /// <returns> A Sequence that'll animate the TextMeshProUGUI. </returns>
        private Sequence AnimateDialogueLine(TextMeshProUGUI textToAnimate, MarkupParseResult markUp)
        {
            // Initialize the Sequence that'll contains all the line's animations
            Sequence dialogueSequence = DOTween.Sequence();
            CharacterLineViewGlobals.ApplyTweenDefaultSettings(dialogueSequence);

            // The dialogueSequence must NOT be autokilled, as it's usage extends the tween's lifespan
            // (such as checking if an animation is currently playing)
            dialogueSequence.SetAutoKill(false);

            int currentCps = CharacterLineViewGlobals.defaultCpsDict[lineProvider.textLanguageCode];
            int currentPos = 0;

            // Go through all the attributes, and run their respective commands
            foreach (MarkupAttribute attribute in markUp.Attributes)
            {
                // Edge Case: Check if the specified attribute exists
                if (!CharacterLineViewGlobals.hasIntextAttribute(attribute.Name))
                {
                    Debug.LogWarning($"The yarn attribute \"{attribute.Name}\" is unrecognized.\n" +
                        $"Currently displayed line: \"{textToAnimate.text}\"");
                    continue;
                }

                // Add a tween to animate the text, with the current settings, from the current position to the next attribute.
                dialogueSequence.Append(
                    CreateDialogueTween(textToAnimate, currentPos, attribute.Position, currentCps)
                    );

                // Update the position
                currentPos = attribute.Position;

                // Run the attribute's command
                IIntextAttribute attributeCommand = CharacterLineViewGlobals.getIntextAttributeCommand(attribute.Name).Invoke();
                bool continueCommands = attributeCommand.RunAttributeCommand(this, attribute, ref dialogueSequence, ref currentCps);

                // If no more commands should be parsed, end the method immediately
                if (!continueCommands)
                    return dialogueSequence;
            }

            // Add one last tween to animate the text from the last attribute to the end of the text
            dialogueSequence.Append(
                CreateDialogueTween(textToAnimate, currentPos, lineText.GetParsedText().Length, currentCps) // Used parsed text length, as normal text length includes richtext attributes
                );

            return dialogueSequence;
        }


        /// <summary>
        /// Creates a tweener that will tween a TextMeshProUGUI's displayed characters
        /// from a start position to an end position.
        /// </summary>
        /// <param name="textToAnimate"> The TextMeshProUGUI to animate. </param>
        /// <param name="startPos"> The start position to animate from. </param>
        /// <param name="endPos"> The end position to animate to. </param>
        /// <param name="cps"> The speed (characters per second) to animate with. </param>
        /// <returns> The Tweener containing the animation. </returns>
        private Tweener CreateDialogueTween(TextMeshProUGUI textToAnimate, int startPos, int endPos, int cps)
        {
            float duration = (float)(endPos - startPos) / cps;

            Tweener tweener = DOTween.To(
                                        () => textToAnimate.maxVisibleCharacters,
                                        x => textToAnimate.maxVisibleCharacters = x,
                                        endPos,
                                        duration);

            CharacterLineViewGlobals.ApplyTweenDefaultSettings(tweener);

            return tweener;
        }


        /// <summary>
        /// This method takes in a yarn script string, and returns the MarkupResult from it.
        /// Character attributes are automatically removed.
        /// </summary>
        /// <param name="yarnLine"> Yarn script to parse. </param>
        /// <returns> The Markup in the string. </returns>
        private MarkupParseResult ExtractMarkup(string yarnLine)
        {
            // Extract the mark up from the line
            MarkupParseResult markUp = yarnScriptParser.ParseMarkup(yarnLine);

            // Remove any character attributes
            MarkupAttribute character;
            if(markUp.TryGetAttributeWithName("character", out character))
                markUp = markUp.DeleteRange(character);

            return markUp;
        }


        /// <summary>
        /// Takes a line, and creates two versions of it.
        /// yarnLine has all Rich Text in-text attributes stripped from it.
        /// textMeshProLine has all YarnSpinner in-text attributes stripped from it.
        /// 
        /// WARNING: This is NOT a pure function. By necessity,
        /// it'll alter the text contained in lineText.
        /// </summary>
        /// <param name="dialogueLine"> LocalizedLine to strip attributes from. </param>
        /// <param name="yarnLine"> Original line without Rich Text attributes. </param>
        /// <param name="textMeshProLine"> Original line without YarnSpinner attributes. </param>
        private void SeparateInTextCommands(LocalizedLine dialogueLine, out string yarnLine, out string textMeshProLine)
        {
            textMeshProLine = dialogueLine.TextWithoutCharacterName.Text;

            lineText.text = dialogueLine.RawText;
            lineText.ForceMeshUpdate(); // This forces "GetParsedText" to give the proper text
            yarnLine = lineText.GetParsedText();

            return;
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