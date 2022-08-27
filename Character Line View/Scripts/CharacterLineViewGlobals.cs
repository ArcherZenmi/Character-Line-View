/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file contains many "globally" accessibly variables & methods
///    used throughout the CharacterLineView namespace and associated classes.
///    
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Markup;

namespace CharacterLineView
{
    public class CharacterLineViewGlobals : MonoBehaviour
    {

        /// <summary>
        /// When displaying a new line, this is the cps that's always used (unless attributes specify otherwise).
        /// Naturally, cps differs depending on the language.
        /// The key/language identifier must match that used in YarnSpinner.
        /// </summary>
        public static readonly Dictionary<string, int> defaultCpsDict =
            new Dictionary<string, int>
            {
                {"en-US", 60 },
                {"ja-JP", 30 },
            };

        /// <summary>
        /// This stores all of the available in-text commands when writing dialogue.
        /// All attributes MUST be specified in lowercase.
        /// However, they are case-insensitive when actually writing in yarn script (see getIntextAttributeCommand).
        /// </summary>
        private static readonly Dictionary<string, Func<IIntextAttribute>> intextAttributesDict =
            new Dictionary<string, Func<IIntextAttribute>>
            {
                {"cps", () => new CpsAttribute() },
                {"done", () => new DoneAttribute() },
                {"fast",  () => new FastAttribute() },
                {"w", () => new WaitAttribute() },
                {"voice", () => new VoiceAttribute() },
            };


        /// <summary>
        /// This specifies the default settings for Tween's used throughout Character Line View.
        /// Having this globally accessible method makes it easy to have
        /// consistent settings throughout this package,
        /// without resorting to changing any global settings for all of DoTween.
        ///
        /// If any settings are important to the tweens, explicitely set them here.
        /// It is NOT safe to assume that DoTween's global settings are the
        /// same as the default ones specified on their website
        /// (in all likelihood, they've been altered to fit the current game).
        /// </summary>
        /// <param name="tween"> </param>
        public static void ApplyTweenDefaultSettings(Tween tween)
        {
            tween.SetEase(Ease.Linear);
            tween.SetAutoKill(true);

            // Since we can't set "SetAutoPlay", this is used instead
            tween.Pause();
        }


        /// <summary>
        /// Can be used to access the command associated with an intext attribute.
        /// </summary>
        /// <param name="attribute"> The attribute to get a command from. </param>
        /// <returns> A delegate that returns an IIntextAttribute object for the attribute's command.
        ///     If no attribute is found, returns null. </returns>
        public static Func<IIntextAttribute> getIntextAttributeCommand(string attribute)
        {
            // If the command doesn't exist, log an error
            if(!hasIntextAttribute(attribute.ToLower()))
            {
                Debug.LogError($"Cannot access the non existent in-text attribute \"{attribute}\".");
                return null;
            }

            return intextAttributesDict[attribute.ToLower()];
        }

        /// <summary>
        /// Can be used to ask whether a certain in-text attribute exists.
        /// </summary>
        /// <param name="attribute"> The attribute to check. </param>
        /// <returns> True if the attribute exists. False otherwise. </returns>
        public static bool hasIntextAttribute(string attribute)
        {
            return intextAttributesDict.ContainsKey(attribute.ToLower());
        }
    }
}
