/// <summary> 
/// Author:    Archer Zenmi
/// Package:   N/A
/// 
/// File Contents 
/// 
///     This class specifies the behavior of "Character.SerializableTransform"
///         in the inspector.
///
///     (Currently this is NOT part of the standard CLV package,
///         but hopefully it will be soon).
///    
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CharacterLineView
{
    [CustomPropertyDrawer(typeof(Character.CharacterPosition))]
    public class CharacterPositionEditor : PropertyDrawer
    {
        /// <summary>
        /// Specify how the property is displayed in the inspector
        /// </summary>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create a new VisualElement to be the root the property UI
            var container = new VisualElement();

            // Set everything in a foldout menu labelled with the position's name
            var foldout = new Foldout();
            foldout.value = false;
            string name = property.FindPropertyRelative("name").stringValue;
            if (name != string.Empty)
                foldout.text = name;
            else
                foldout.text = "unnamed";

            // Display the normal properties
            foldout.Add(new PropertyField(property.FindPropertyRelative("name"), "Name"));
            foldout.Add(new PropertyField(property.FindPropertyRelative("position"), "Position"));
            foldout.Add(new PropertyField(property.FindPropertyRelative("rotation"), "Rotation"));
            foldout.Add(new PropertyField(property.FindPropertyRelative("scale"), "Scale"));

            // Add a button to copy the transform info from the current transform
            var copyButton = new Button(() => CopyFromTransform(property));
            copyButton.text = "Copy From Transform";
            foldout.Add(copyButton);

            // Add a button to paste our transform info to the current transform
            var pasteButton = new Button(() => PasteToTransform(property));
            pasteButton.text = "Paste To Transform";
            foldout.Add(pasteButton);

            // Add the foldout to the root
            container.Add(foldout);

            // Return the finished UI
            return container;
        }


        /// <summary>
        /// Called whenever the copy button is pushed.
        /// Copy's the info from the GameObject's transform and stores it.
        /// </summary>
        private void CopyFromTransform(SerializedProperty property)
        {
            // Find the GameObject's transform
            Character target = (Character)property.serializedObject.targetObject;
            Transform objectTransform = target.transform;

            // Copy the info
            property.FindPropertyRelative("position").vector3Value = objectTransform.position;
            property.FindPropertyRelative("rotation").vector3Value = objectTransform.localEulerAngles;
            property.FindPropertyRelative("scale").vector3Value = objectTransform.localScale;

            // Apply the changes
            property.serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Called whenever the paste button is pushed.
        /// Paste's our stored info info into the GameObject's transform.
        /// </summary>
        private void PasteToTransform(SerializedProperty property)
        {
            // Find the GameObject's transform
            Character target = (Character)property.serializedObject.targetObject;
            Transform objectTransform = target.transform;

            // Paste the info
            objectTransform.position = property.FindPropertyRelative("position").vector3Value;
            objectTransform.localEulerAngles = property.FindPropertyRelative("rotation").vector3Value;
            objectTransform.localScale = property.FindPropertyRelative("scale").vector3Value;
        }
    }
}
