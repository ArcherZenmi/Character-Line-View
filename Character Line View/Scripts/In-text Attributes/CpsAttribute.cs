/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file creates the "cps" in-text attribute for usage in the CharacterLineView dialogue box.
///    The cps attribute is used as follows:
///     - "cps" is a self-closing attribute, which represents the characters per second
///         that's displayed to the player.
///     - The cps attribute must specify a property labelled "cps" that is set to an integer
///         to be used as the new cps.
///         - In most cases, this is written as [cps=30/], where 30 can be any integer.
///     - Note: The default cps used at the start of a line is specified in CharacterLineViewGlobals.
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
    public class CpsAttribute : IIntextAttribute
    {
        /// <inheritdoc/>
        /// <summary>
        /// Change the cps (charcters per second) of the displayed text.
        /// </summary>
        public bool RunAttributeCommand(CharacterLineView clv, MarkupAttribute attribute, ref Sequence tweenList, ref int currentCps)
        {
            // Edge Case: Cps may want to return to the default value
            if(attribute.Properties.ContainsKey("default"))
            {
                currentCps = CharacterLineViewGlobals.defaultCpsDict[clv.lineProvider.textLanguageCode];
                return true;
            }

            // Error Case: A new int value for cps may not be defined
            if(!attribute.Properties.ContainsKey("cps") || attribute.Properties["cps"].Type != MarkupValueType.Integer)
            {
                Debug.LogWarning("A cps attribute must contain a valid int value to set the cps property to. The attribute has been skipped.");
                return true;
            }

            // Set the cps to the new value
            int newCps = attribute.Properties["cps"].IntegerValue;
            currentCps = newCps;

            // Keep parsing further commands
            return true;
        }
    }
}