using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This script is used for testing multiplayer as I do not have more than 1 VR device
/// This script contains custom editor behaviour
/// </summary>

[CustomEditor(typeof(LoginManager))]
public class LoginManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("This script is responsible for connecting to photon servers.", MessageType.Info);

        //target inherited from editor class and it refers to the inspected object
        LoginManager loginManager = (LoginManager)target;

        if (GUILayout.Button("Connect Anonymously"))
        {
            loginManager.ConnectToPhotonServer();
        }
    }

}
