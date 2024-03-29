/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file creates the "done" in-text attribute for usage in the CharacterLineView dialogue box.
///    The done attribute is used as follows:
///     - "done" is a selfclosing attribute.
///     - Using the done attribute in a dialogue line will immediately terminate the line's animation.
///         That is it say, it gets treated as the end of the line.
///     - If the "nw" property is added (regardless of its value), the done attribute
///         will also automatically move on to the next line without any player input.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Yarn.Markup;

namespace CharacterLineView
{
    public class DoneAttribute : IIntextAttribute
    {
        /// <inheritdoc/>
        public bool RunAttributeCommand(TextMeshProUGUI textComponent, MarkupAttribute attribute, ref Sequence tweenList,
                ref int currentCps, int defaultCps,
                CharacterVoices characterVoices,
                Action requestViewAdvancement)
        {
            // Check if nw attribute is added
            // If so, line should be dismissed at the end of the animation.
            if (attribute.Properties.ContainsKey("nw"))
            {
                // Start this a little later to avoid complications.
                tweenList.AppendInterval(0.05f);

                // Force the line to skip to the next without user input
                // Do NOT use OnComplete callback, as it doesn't get called when Complete() is used.
                tweenList.AppendCallback(new TweenCallback(() => requestViewAdvancement.Invoke()));
            }

            // Stop parsing more attribute
            return false;
        }
    }
}