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
        /// <param name="clv"> The CharacterLineView running that's parsing dialogue with this command. </param>
        /// <param name="attribute"> The markup/command associated with the current position of the line. </param>
        /// <param name="tweenList"> The sequence of tween's thus far constructed for this line. </param>
        /// <param name="currentCps"> The cps at the current position of the line. </param>
        /// <returns> True if further commands can be parsed. False otherwise. </returns>
        public abstract bool RunAttributeCommand(CharacterLineView clv, MarkupAttribute attribute, ref Sequence tweenList, ref int currentCps);
    }
}