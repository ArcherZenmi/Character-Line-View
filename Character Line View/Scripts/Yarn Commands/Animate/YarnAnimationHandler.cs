/// <summary> 
/// Author:    Archer Zenmi
/// Package:   Character Line View
/// 
/// File Contents 
/// 
///    This file contains the "animation" command to be used in YarnScript.
///    It's not called directly by YarnScript, rather, this is called by
///    YarnAnimationCommand whenever this game object/handler is set
///    as the "current animation handler" (see YarnAnimationCommand for details).
///
///     Simply pass in a list of TimelineAssets, then call PlayAnimation() to play it.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yarn.Unity;

namespace CharacterLineView
{
    public class YarnAnimationHandler : MonoBehaviour
    {
        [SerializeField]
        private bool setAsCurrentHandler = false;

        [SerializeField]
        private CharacterLineView dialogueBox;

        [SerializeField]
        private TimelineAsset[] animationTimelines;

        private PlayableDirector animationDirector;
        private Dictionary<string, TimelineAsset> animationTimelineDict;


        /// <inheritdoc/>
        /// <summary>
        /// Set this AnimationHandler as the main one to be used by YarnScript,
        /// as well as initialize some internally used variables.
        /// </summary>
        public void Start()
        {
            // Set this as the main animation handler for YarnScript
            if(setAsCurrentHandler)
                SetAsCurrentHandler();

            // Find the PlayableDirector on this game object
            animationDirector = gameObject.GetComponent<PlayableDirector>();

            // Convert the array of animations to a dictionary
            animationTimelineDict = new Dictionary<string, TimelineAsset>();
            foreach (TimelineAsset timeline in animationTimelines)
            {
                animationTimelineDict.Add(timeline.name, timeline);
            }
        }


        /// <summary>
        /// The implementation of an "animate" command that'll be called by YarnAnimationCommand
        /// when a command is passed through YarnScript.
        /// </summary>
        /// <param name="animationName"> The name of the animation to be played. </param>
        /// <param name="async"> True if dialogue should continue during the animation, 
        ///     false if dialogue should wait for animation to finish. </param>
        public IEnumerator PlayAnimation(string animationName, bool async = false)
        {
            // Error case: The animation doesn't exist
            if (!animationTimelineDict.ContainsKey(animationName))
            {
                Debug.LogWarning($"The animation, \"{animationName}\", does not exist or hasn't been set. " +
                    "Animation will be skipped.");
                yield break;
            }

            // Edge case: If an animation is already playing,
            // force it to speed up the animation and end immediately
            if(animationDirector.state == PlayState.Playing)
            {
                animationDirector.time = animationDirector.duration;
                animationDirector.Evaluate();
                animationDirector.Stop(); // This also destroys the internal playable/graph
            }

            // If the command is synchronous, wait for the dialogue box to close
            if (!async)
                yield return new WaitForSeconds(dialogueBox.Hide());

            // Set the animation and play it.
            animationDirector.playableAsset = animationTimelineDict[animationName];
            animationDirector.Play();
            animationDirector.Evaluate(); // Evaluate it immediately so as to avoid potential bugs (e.g reading its duration)

            // If the command requests syncronous animation (async = false),
            // force the dialogue lines to wait for the animation to finish.
            if (!async)
            {
                // Force the dialogue to wait for the animation
                yield return new WaitForSeconds((float)animationDirector.duration);
            }
        }


        /// <summary>
        /// Sets this YarnAnimationHandler as the one used by YarnAnimationCommand.
        /// That's to say, any animations passed in by YarnScript will be
        /// handled by this object.
        /// </summary>
        public void SetAsCurrentHandler()
        {
            YarnAnimationSingleton.SetCurrentAnimationHandler(this);
        }
    }
}