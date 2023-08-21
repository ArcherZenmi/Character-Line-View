/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file specifies the IIntextAttribute interface.
///    Any class that extends this interface can create an implementation
///    for an in-text attribute/command that can be used in a CharacterLineView dialogue box.
///    Note that the attribute's name must be added to the CharacterLineViewGlobals's
///    dictionary of attribute for it to be registered and usable.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Yarn.Markup;
using Yarn.Unity;

namespace CharacterLineView
{
    public interface IIntextAttribute
    {
        /// <summary>
        /// Run the command associated with this attribute.
        /// </summary>
        /// <param name="textComponent"> The TextMeshProUGUI that's being animated. </param>
        /// <param name="attribute"> The markup/command associated with the current position of the line. </param>
        /// <param name="tweenList"> The sequence of tween's thus far constructed for this line. </param>
        /// <param name="currentCps"> The cps at the current position of the line. </param>
        /// <param name="defaultCps"> The default cps used at the beginning of a line. </param>
        /// <param name="characterVoices"> The CharacterVoices in charge of audio. </param>
        /// <param name="requestViewAdvancement"> An action to call to cause a view advancement to occur. </param>
        /// <returns> True if further commands can be parsed. False otherwise. </returns>
        public abstract bool RunAttributeCommand(TextMeshProUGUI textComponent, MarkupAttribute attribute, ref Sequence tweenList,
                ref int currentCps, int defaultCps,
                CharacterVoices characterVoices,
                Action requestViewAdvancement);
    }
}