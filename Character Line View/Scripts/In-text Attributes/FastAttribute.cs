/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file creates the "fast" in-text attribute for usage in the CharacterLineView dialogue box.
///    The fast attribute is used as follows:
///     - "fast" is a selfclosing attribute.
///     - Using the fast attribute in a dialogue box will immediately disregard any animations
///         for the preceding parts of the line.
///         That's to say the type-writer effect will begin from the fast attribute,
///         and everything before it will be displayed immediately.
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Markup;

namespace CharacterLineView
{
    public class FastAttribute : IIntextAttribute
    {
        /// <inheritdoc/>
        public bool RunAttributeCommand(CharacterLineView clv, MarkupAttribute attribute, ref Sequence tweenList, ref int currentCps)
        {
            // Create a new tween with the old settings
            Sequence temp = DOTween.Sequence().SetAs(tweenList);
            tweenList.Kill();
            tweenList = temp;

            // Force the CharacterLineView to begin displaying from the current position
            clv.lineText.maxVisibleCharacters = attribute.Position;

            // Continue to parse attribute
            return true;
        }
    }
}