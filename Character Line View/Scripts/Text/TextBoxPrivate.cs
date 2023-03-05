/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///     This file specifies a partial class for TextBox.cs.
///     Most private methods are contained here so as to not
///         clog up the API/public methods in the other file.
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
        /// <summary>
        /// Takes a line, and creates two versions of it.
        /// yarnLine has all Rich Text in-text attributes stripped from it.
        /// textMeshProLine has all YarnSpinner in-text attributes stripped from it.
        /// 
        /// WARNING: This is NOT a pure function. By necessity,
        /// it'll alter the text contained in this game object.
        /// </summary>
        /// <param name="dialogueLine"> LocalizedLine to strip attributes from. </param>
        /// <param name="yarnLine"> Original line without Rich Text attributes. </param>
        /// <param name="textMeshProLine"> Original line without YarnSpinner attributes. </param>
        private void SeparateInTextCommands(LocalizedLine dialogueLine, out string yarnLine, out string textMeshProLine)
        {
            textMeshProLine = dialogueLine.TextWithoutCharacterName.Text;

            textComponent.text = dialogueLine.RawText;
            textComponent.ForceMeshUpdate(); // This forces "GetParsedText" to give the proper text
            yarnLine = textComponent.GetParsedText();

            return;
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
            if (markUp.TryGetAttributeWithName("character", out character))
                markUp = markUp.DeleteRange(character);

            return markUp;
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
            if (dialogueLine.Metadata == null || !dialogueLine.Metadata.Contains("no_voice"))
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
        /// This will take in a MarkupParseResult that specifies a text animation.
        /// It'll set up the Sequence that'll animate this TextMeshProUGUI.
        /// </summary>
        /// <param name="markUp"> The MarkupParseResult specifying the animation. </param>
        /// <param name="startingCps"> The cps that the line starts with. </param>
        /// <param name="requestViewAdvancement"> A delegate that can be called for a view advancement. </param>
        /// <returns> A Sequence that'll animate this TextMeshProUGUI. </returns>
        private Sequence AnimateDialogueLine(MarkupParseResult markUp, int startingCps, Action requestViewAdvancement)
        {
            // Initialize the Sequence that'll contains all the line's animations
            Sequence dialogueSequence = DOTween.Sequence();
            CharacterLineViewGlobals.ApplyTweenDefaultSettings(dialogueSequence);

            // The dialogueSequence must NOT be autokilled, as it's usage extends the tween's lifespan
            // (such as checking if an animation is currently playing)
            dialogueSequence.SetAutoKill(false);

            // Initialize variables to keep track of during the animation
            int currentCps = startingCps;
            int currentPos = 0;

            // Go through all the attributes, and run their respective commands
            foreach (MarkupAttribute attribute in markUp.Attributes)
            {
                // Edge Case: Check if the specified attribute exists
                if (!CharacterLineViewGlobals.hasIntextAttribute(attribute.Name))
                {
                    Debug.LogWarning($"The yarn attribute \"{attribute.Name}\" is unrecognized.\n" +
                        $"Currently displayed line: \"{textComponent.text}\"");
                    continue;
                }

                // Add a tween to animate the text, with the current settings, from the current position to the next attribute.
                dialogueSequence.Append(
                    CreateDialogueTween(currentPos, attribute.Position, currentCps)
                    );

                // Update the position
                currentPos = attribute.Position;

                // Run the attribute's command
                IIntextAttribute attributeCommand = CharacterLineViewGlobals.getIntextAttributeCommand(attribute.Name).Invoke();
                bool continueCommands = attributeCommand.RunAttributeCommand(textComponent, attribute, ref dialogueSequence, ref currentCps, startingCps, characterVoices, requestViewAdvancement);

                // If no more commands should be parsed, end the method immediately
                if (!continueCommands)
                    return dialogueSequence;
            }

            // Add one last tween to animate the text from the last attribute to the end of the text
            dialogueSequence.Append(
                CreateDialogueTween(currentPos, textComponent.GetParsedText().Length, currentCps) // Used parsed text length, as normal text length includes richtext attributes
                );

            return dialogueSequence;
        }


        /// <summary>
        /// Creates a tweener that will tween this TextMeshProUGUI's displayed characters
        /// from a start position to an end position.
        /// </summary>
        /// <param name="startPos"> The start position to animate from. </param>
        /// <param name="endPos"> The end position to animate to. </param>
        /// <param name="cps"> The speed (characters per second) to animate with. </param>
        /// <returns> The Tweener containing the animation. </returns>
        private Tweener CreateDialogueTween(int startPos, int endPos, int cps)
        {
            float duration = (float)(endPos - startPos) / cps;

            Tweener tweener = DOTween.To(
                                        () => textComponent.maxVisibleCharacters,
                                        x => textComponent.maxVisibleCharacters = x,
                                        endPos,
                                        duration);

            CharacterLineViewGlobals.ApplyTweenDefaultSettings(tweener);

            return tweener;
        }
    }
}