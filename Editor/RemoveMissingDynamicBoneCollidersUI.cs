﻿using UnityEngine;
using System.Collections;
using UnityEditor;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Editor;
#elif VRC_SDK_VRCSDK2
#endif
using VRC.SDKBase.Editor;
using Jirko.Unity.VRoidAvatarUtils;
using System.Collections.Generic;

namespace Jirko.Unity.VRoidAvatarUtils
{
    public class RemoveMissingDynamicBoneCollidersUI : EditorWindow
    {
        Vector2 scrollPosition = new Vector2(0, 0);
        public GameObject targetObject = null;

        private List<string> messages_list;
        private string messages = "";

        private VRoidAvatar sourceAvatarDTO = null;

        [MenuItem("VRoidAvatarSetup/Remove MissingDynamicBoneColliders", priority = 20)]
        static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<RemoveMissingDynamicBoneCollidersUI>();
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
                RemoveMissingDynamicBoneColiders(targetObject);

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
        private void RemoveMissingDynamicBoneColiders(GameObject targetObject)
        {
            DynamicBone[] dynamicBones = targetObject.GetComponentsInChildren<DynamicBone>();
            foreach (var dynamicBone in dynamicBones)
            {
                dynamicBone.m_Colliders.RemoveAll(item => item == null);
            }
            messages_list.Add("空のDynamicBoneColidersを削除しました。");
        }
    }
}