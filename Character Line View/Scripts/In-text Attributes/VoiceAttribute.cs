/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file creates the "voice" in-text attribute for usage in the CharacterLineView dialogue box.
///    The voice attribute is used as follows:
///     - "voice" is a selfclosing attribute.
///     - One, and only one, of the following properties should be attached to a voice atttribute.
///         If multiple properties are needed, consider writing two or more attributes in a row.
///     - By attaching a "default" property (value doesn't matter),
///         the voice clips will be played at default speed.
///     - By attatching a "speed" property with an integer,
///         we can specify the voice clips per second to play.
///         This can be especially useful when the character is talking slowly.
///         - It's a known issue that particularly slow speeds can cause it to act up.
///             Consider prepending a space or two to your line to give the audio time to stabilize,
///             and everything afterwards should feel synced up.
///     - By attaching a "mute" property with a boolean,
///         we can specify whether to mute or unmute the voice.
///         This tag does not affect the voice speed.
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Markup;

namespace CharacterLineView
{
    public class VoiceAttribute : IIntextAttribute
    {
        /// <inheritdoc/>
        /// <summary>
        /// Used to alter how the character's voice is played.
        /// "default" plays the voice at normal speed, "speed" specifies how many voice clips to play per second (integer).
        /// </summary>
        public bool RunAttributeCommand(CharacterLineView clv, MarkupAttribute attribute, ref Sequence tweenList, ref int currentCps)
        {
            // Edge Case: If the voice command is at the start of the line, start the voice a little later
            // to avoid conflicting with initialization
            if(attribute.Position == 0)
                tweenList.AppendInterval(0.01f);

            // If a "default" property is specified, play the character's voice at normal speed
            if(attribute.Properties.ContainsKey("default"))
            {
                tweenList.AppendCallback(new TweenCallback(
                    () => clv.characterVoices.PlayVoice()));

                return true;
            }

            // If a voice property is specified, play the character's voice with the specified speed
            else if(attribute.Properties.ContainsKey("speed"))
            {
                // Error case: The voice property must be set to an integer.
                if(attribute.Properties["speed"].Type != MarkupValueType.Integer)
                {
                    Debug.LogWarning("The speed property, which represents how many voice clips to play per second, must be set to an integer." +
                    "The attribute will be ignored.");
                }

                // Change the character's voice
                else
                {
                    tweenList.AppendCallback(new TweenCallback(
                        () => clv.characterVoices.PlayVoiceTimer(attribute.Properties["speed"].IntegerValue)));
                }

                return true;
            }

            // If a "mute" property is specified, just use its boolean value
            else if(attribute.Properties.ContainsKey("mute"))
            {
                // Error case: The mute property must be set to a boolean.
                if(attribute.Properties["mute"].Type != MarkupValueType.Bool)
                {
                    Debug.LogWarning("The mute property must be set to a boolean value. The attribute will be ignored.");
                }

                // Set the mute value
                else
                {
                    bool mute = attribute.Properties["mute"].BoolValue;
                    tweenList.AppendCallback(new TweenCallback(
                        () => clv.characterVoices.Mute = mute));
                }

                return true;
            }

            // Error case: The proper properties must be specified.
            Debug.LogWarning("The voice attribute must have a \"default\", \"speed\", or \"mute\" property." +
                "The attribute will be ignored.");

            return true;
        }
    }
}