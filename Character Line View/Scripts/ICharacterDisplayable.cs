/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    Any children of a CharacterLineView object that implements ICharacterDisplayable
///    will have their SetDisplayable method called whenever a new line
///    is passed by Yarnscript.
///    
///    This allows for certain displayables to change what they're displaying
///    depending on the line.
///    
///    E.g
///    - Changing the dialogue box's color depdending on the talking character
///    - Changing a character icon depending on who's talking
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace CharacterLineView
{
    public interface ICharacterDisplayable
    {
        /// <summary>
        /// Alters the displayable (i.e part of the dialogue GUI) to match the current line,
        /// e.g change based on characters, their emotion, etc.
        /// </summary>
        /// <param name="dialogueLine"> A LocalizedLine used to inform how the displayable should be set. </param>
        public abstract void SetDisplayable(LocalizedLine dialogueLine);
    }

}