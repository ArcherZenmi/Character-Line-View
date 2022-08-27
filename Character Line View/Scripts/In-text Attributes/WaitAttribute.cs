/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file creates the "w" (wait) in-text attribute for usage in the CharacterLineView dialogue box.
///    The w attribute is used as follows:
///     - "w" is a selfclosing attribute, which represents how many seconds to wait
///         at the current position before continuing to animate/display the line.
///     - The w attribute must specify a "w" property that is set to a float/integer
///         that represents how many seconds to pause.
///         - In most cases, this is written as [w=2/] or [w=2.0/], where 2 and 2.0 can be any number.
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Markup;

namespace CharacterLineView
{
    public class WaitAttribute : IIntextAttribute
    {
        private const string ATTRIBUTE_NAME = "w";

        /// <inheritdoc/>
        public bool RunAttributeCommand(CharacterLineView clv, MarkupAttribute attribute, ref Sequence tweenList, ref int currentCps)
        {
            // Error Case: The w property may not be specified
            if(!attribute.Properties.ContainsKey(ATTRIBUTE_NAME))
            {
                Debug.LogWarning($"A \"{ATTRIBUTE_NAME}\" attribute must contain a valid float/integer value to set the \"{ATTRIBUTE_NAME}\" property to. The attribute has been skipped.");
                return true;
            }

            MarkupValue attributeProperty = attribute.Properties[ATTRIBUTE_NAME];

            // Find out how many seconds to wait
            float waitTime;
            if (attributeProperty.Type == MarkupValueType.Float)
                waitTime = attributeProperty.FloatValue;
            else if (attributeProperty.Type == MarkupValueType.Integer)
                waitTime = attributeProperty.IntegerValue;

            // Error case: The w property is not set to a float/integer
            else
            {
                Debug.LogWarning($"A \"{ATTRIBUTE_NAME}\" attribute must contain a valid float/integer value to set the \"{ATTRIBUTE_NAME}\" property to. The attribute has been skipped.");
                return true;
            }

            // Temporarily stop the character's voice
            tweenList.AppendCallback(new TweenCallback(() => clv.characterVoices.StopVoice()));

            // Pause the line's type-writer animation for the attribute's specified time
            tweenList.AppendInterval(waitTime);

            // After the pause, re-start the character's voice
            tweenList.AppendCallback(new TweenCallback(() => clv.characterVoices.PlayVoice()));

            // Continue to parse commands
            return true;
        }
    }
}