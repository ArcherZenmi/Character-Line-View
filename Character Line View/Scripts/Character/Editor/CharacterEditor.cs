/// <summary> 
/// Author:    Archer Zenmi
/// Package:   N/A
/// 
/// File Contents 
/// 
///     This class specifies the behavior of "Character.cs" in the inspector.
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

namespace CharacterLineView
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : Editor
    {
        // The UXML file that specifies the GUI
        public VisualTreeAsset UXMLFile;

        // Causes the inspector to be overridden with whatever
        // UXML file is specified
        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement myInspector = new VisualElement();

            // Load from default reference
            UXMLFile.CloneTree(myInspector);

            // Return the finished inspector UI
            return myInspector;
        }
    }
}
