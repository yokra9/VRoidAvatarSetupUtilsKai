using UnityEngine;
using System.Collections;
using UnityEditor;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Editor;
#elif VRC_SDK_VRCSDK2
#endif
using VRC.SDKBase.Editor;
using Jirko.Unity.VRoidAvatarUtils;

namespace Jirko.Unity.VRoidAvatarUtils
{
    public class CopyAvatarParametersUI : EditorWindow
    {
        bool folding = true;
        Vector2 scrollPosition = new Vector2(0, 0);
        public GameObject sourceObject = null;
        public GameObject targetObject = null;

        public int avatarMode = 0;
        public bool viewPosition = true;
#if VRC_SDK_VRCSDK3
        public bool eyeMovements = true;
        public bool baseAnimationLayers = true;
        public bool specialAnimationLayers = true;
        public bool expressionsMenu = true;
        public bool expressionParameters = true;
#elif VRC_SDK_VRCSDK2
        public bool eyeMovements = false;
        public bool baseAnimationLayers = false;
        public bool specialAnimationLayers = false;
        public bool expressionsMenu = false;
        public bool expressionParameters = false;
#endif
        public bool rotationStates = true;
        public bool blueprintId = true;
        public bool dynamicBones = true;
        public bool dynamicBones_hair = true;
        public bool dynamicBones_skirt = true;
        public bool dynamicBones_bust = true;
        public bool dynamicBones_sleeve = true;
        public bool dynamicBones_other = true;

        public bool dynamicBoneColiders = true;
        public bool objects = false;

        public string messages = "";
        public string errors = "";

        private VRoidAvatar sourceAvatarDTO = null;

        [MenuItem("VRoidAvatarSetup/Open CopyAvatarParameters Wizard", priority = 1)]
        static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<CopyAvatarParametersUI>();
            window.minSize = new Vector2(400, 500);
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("VRC Avatar", EditorStyles.boldLabel);
            sourceObject = (GameObject)EditorGUILayout.ObjectField("Source Avatar", sourceObject, typeof(GameObject), true);
            targetObject = (GameObject)EditorGUILayout.ObjectField("Destination Avatar", targetObject, typeof(GameObject), true);
            avatarMode = EditorGUILayout.IntPopup("Avatar Type", avatarMode, new string[] { "VRoid Avatar", "Other Avatar" }, new int[] { 0, 1 });
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRC Avatar Descripter", EditorStyles.boldLabel);

            viewPosition = EditorGUILayout.Toggle("View Position", viewPosition);

#if VRC_SDK_VRCSDK3
            if (folding = EditorGUILayout.Foldout(folding, "Eye Look"))
            {
                eyeMovements = EditorGUILayout.Toggle("Eye Movements", eyeMovements);
                rotationStates = EditorGUILayout.Toggle("Rotation States", rotationStates);

            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Playable Layers", EditorStyles.boldLabel);
            baseAnimationLayers = EditorGUILayout.Toggle("Base", baseAnimationLayers);
            specialAnimationLayers = EditorGUILayout.Toggle("Special", specialAnimationLayers);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Expressions", EditorStyles.boldLabel);
            expressionsMenu = EditorGUILayout.Toggle("Menu", expressionsMenu);
            expressionParameters = EditorGUILayout.Toggle("Parameters", expressionParameters);
#elif VRC_SDK_VRCSDK2
#endif
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pipeline Manager", EditorStyles.boldLabel);
            blueprintId = EditorGUILayout.Toggle("Blueprint ID", blueprintId);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dynamic Bone", EditorStyles.boldLabel);

            if (avatarMode == 0)
            {
                EditorGUILayout.LabelField("Dynamic Bones");
                dynamicBones_hair = EditorGUILayout.Toggle("　髪の毛", dynamicBones_hair);
                dynamicBones_skirt = EditorGUILayout.Toggle("　スカート", dynamicBones_skirt);
                dynamicBones_sleeve = EditorGUILayout.Toggle("　袖", dynamicBones_sleeve);
                dynamicBones_bust = EditorGUILayout.Toggle("　胸", dynamicBones_bust);
                dynamicBones_other = EditorGUILayout.Toggle("　その他", dynamicBones_other);
            }
            else
            {
                dynamicBones = EditorGUILayout.Toggle("Dynamic Bones", dynamicBones);
            }
            dynamicBoneColiders = EditorGUILayout.Toggle("Dynamic Bone Coliders", dynamicBoneColiders);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object", EditorStyles.boldLabel);
            objects = EditorGUILayout.Toggle("Objects", objects);
            EditorGUILayout.Space();

            if (sourceObject == null || targetObject == null)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            if (GUILayout.Button("Copy"))
            {
                sourceAvatarDTO = new VRoidAvatar(sourceObject);

                sourceAvatarDTO.avatarMode = avatarMode;
                sourceAvatarDTO.viewPosition = viewPosition;
                sourceAvatarDTO.eyeMovements = eyeMovements;
                sourceAvatarDTO.baseAnimationLayers = baseAnimationLayers;
                sourceAvatarDTO.specialAnimationLayers = specialAnimationLayers;
                sourceAvatarDTO.expressionsMenu = expressionsMenu;
                sourceAvatarDTO.expressionParameters = expressionParameters;
                sourceAvatarDTO.rotationStates = rotationStates;
                sourceAvatarDTO.blueprintId = blueprintId;
                sourceAvatarDTO.dynamicBones = dynamicBones;
                sourceAvatarDTO.dynamicBones_hair = dynamicBones_hair;
                sourceAvatarDTO.dynamicBones_skirt = dynamicBones_skirt;
                sourceAvatarDTO.dynamicBones_bust = dynamicBones_bust;
                sourceAvatarDTO.dynamicBones_sleeve = dynamicBones_sleeve;
                sourceAvatarDTO.dynamicBones_other = dynamicBones_other;
                sourceAvatarDTO.dynamicBoneColiders = dynamicBoneColiders;
                sourceAvatarDTO.objects = objects;

                Undo.RecordObject(targetObject, "Copy Parameters " + targetObject.name);
                EditorUtility.SetDirty(targetObject);
                messages = "";
                sourceAvatarDTO.CopyToTarget(targetObject);

                if (sourceAvatarDTO.messages.Count > 0)
                {
                    messages = "---messages-----------\n" + string.Join("\n", sourceAvatarDTO.messages) + "\nコピーが完了しました。";
                }

            }
            if (sourceObject == null || targetObject == null)
            {
                EditorGUI.EndDisabledGroup();
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label(messages);
            EditorGUILayout.EndScrollView();
        }
    }
}