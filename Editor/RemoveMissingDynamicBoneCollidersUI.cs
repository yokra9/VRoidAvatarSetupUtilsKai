using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Jirko.Unity.VRoidAvatarUtils
{
    public class RemoveMissingPhysBoneCollidersUI : EditorWindow
    {
        Vector2 scrollPosition = new Vector2(0, 0);
        public GameObject targetObject = null;

        private List<string> messages_list;
        private string messages = "";

        [MenuItem("VRoidAvatarSetup/Remove MissingPhysBoneColliders", priority = 20)]
        static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<RemoveMissingPhysBoneCollidersUI>();
            window.minSize = new Vector2(400, 500);
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("VRC Avatar", EditorStyles.boldLabel);
            targetObject = (GameObject)EditorGUILayout.ObjectField("Destination Avatar", targetObject, typeof(GameObject), true);

            if (targetObject == null)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            if (GUILayout.Button("Delete"))
            {
                messages = "";
                messages_list = new List<string>();
                RemoveMissingPhysBoneColiders(targetObject);

                if (messages_list.Count > 0)
                {
                    messages = "---messages-----------\n" + string.Join("\n", messages_list);
                }

            }
            if (targetObject == null)
            {
                EditorGUI.EndDisabledGroup();
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label(messages);
            EditorGUILayout.EndScrollView();
        }
        private void RemoveMissingPhysBoneColiders(GameObject targetObject)
        {
            VRCPhysBone[] PhysBones = targetObject.GetComponentsInChildren<VRCPhysBone>();
            foreach (var PhysBone in PhysBones)
            {
                PhysBone.colliders.RemoveAll(item => item == null);
            }
            messages_list.Add("空のPhysBoneColidersを削除しました。");
        }
    }
}