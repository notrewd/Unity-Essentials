using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.GameSounds
{
    [CustomEditor(typeof(GameSoundsData))]
    public class GameSoundsDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUIStyle titleStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                }
            };

            GUIStyle descriptionStyle = new GUIStyle()
            {
                wordWrap = true,
                normal = new GUIStyleState()
                {
                    textColor = Color.yellow
                }
            };

            GUILayout.Label("Game Sounds Data", titleStyle);

            GUI.color = Color.yellow;
            GUILayout.Label("This asset serves as a data container for Game Sounds settings and is not meant to be edited directly. Please use the Game Sounds window instead.", descriptionStyle);
            GUI.color = Color.white;

            GUILayout.Space(5f);

            GUI.enabled = false;
            base.OnInspectorGUI();
        }
    }
}