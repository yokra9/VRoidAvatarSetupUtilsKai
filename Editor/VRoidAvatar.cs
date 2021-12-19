using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.CustomEyeLookSettings;
#elif VRC_SDK_VRCSDK2
using VRCSDK2;
#endif
using System.Collections.Generic;
using UnityEditorInternal;

namespace Jirko.Unity.VRoidAvatarUtils
{
    public class VRoidAvatar
    {
        private GameObject cloneGameObject = null;
        private Dictionary<string, List<DynamicBone>> dynamicBoneDict;
        private List<string> boneName;
        private List<string> exclusionBoneName;
        public List<string> messages;
        public List<string> errors;
        public float viewX = 0f;
        public float viewY = 0f;
        public float viewZ = 0f;
        public float confidence = 0f;
        public float excitement = 0f;
        public Quaternion quaterRL;
        public Quaternion quaterRR;
        public Quaternion quaterLL;
        public Quaternion quaterLR;
        public Quaternion quaterUL;
        public Quaternion quaterUR;
        public Quaternion quaterDL;
        public Quaternion quaterDR;
        public Quaternion quaterSL;
        public Quaternion quaterSR;
        public string srcBlueprintId;

        public int avatarMode = 0;
        public bool viewPosition = true;
        public bool eyeMovements = true;
        public bool rotationStates = true;
        public bool blueprintId = true;
        public bool dynamicBones = true;
        public bool dynamicBones_hair = true;
        public bool dynamicBones_skirt = true;
        public bool dynamicBones_bust = true;
        public bool dynamicBones_sleeve = true;
        public bool dynamicBones_other = true;
        public bool dynamicBoneColiders = true;
        public bool objects = true;

        public VRoidAvatar(GameObject gameObject)
        {
            cloneGameObject = UnityEngine.Object.Instantiate(gameObject);
            this.InitDTO();
        }
        void InitDTO()
        {

#if VRC_SDK_VRCSDK3
            VRCAvatarDescriptor sourceAvatarDescriptor = null;
            sourceAvatarDescriptor = cloneGameObject.GetComponent<VRCAvatarDescriptor>();
#elif VRC_SDK_VRCSDK2
            VRC_AvatarDescriptor sourceAvatarDescriptor = null;
            sourceAvatarDescriptor = cloneGameObject.GetComponent<VRC_AvatarDescriptor>();
#endif
            viewX = sourceAvatarDescriptor.ViewPosition.x;
            viewY = sourceAvatarDescriptor.ViewPosition.y;
            viewZ = sourceAvatarDescriptor.ViewPosition.z;


#if VRC_SDK_VRCSDK3
            confidence = sourceAvatarDescriptor.customEyeLookSettings.eyeMovement.confidence;
            excitement = sourceAvatarDescriptor.customEyeLookSettings.eyeMovement.excitement;

            EyeRotations eyesLooking = sourceAvatarDescriptor.customEyeLookSettings.eyesLookingRight;
            quaterRL = new Quaternion(eyesLooking.left.x, eyesLooking.left.y, eyesLooking.left.z, eyesLooking.left.w);
            quaterRL = new Quaternion(eyesLooking.right.x, eyesLooking.right.y, eyesLooking.right.z, eyesLooking.right.w);
            eyesLooking = sourceAvatarDescriptor.customEyeLookSettings.eyesLookingLeft;
            quaterLL = new Quaternion(eyesLooking.left.x, eyesLooking.left.y, eyesLooking.left.z, eyesLooking.left.w);
            quaterLR = new Quaternion(eyesLooking.right.x, eyesLooking.right.y, eyesLooking.right.z, eyesLooking.right.w);
            eyesLooking = sourceAvatarDescriptor.customEyeLookSettings.eyesLookingDown;
            quaterDL = new Quaternion(eyesLooking.left.x, eyesLooking.left.y, eyesLooking.left.z, eyesLooking.left.w);
            quaterDR = new Quaternion(eyesLooking.right.x, eyesLooking.right.y, eyesLooking.right.z, eyesLooking.right.w);
            eyesLooking = sourceAvatarDescriptor.customEyeLookSettings.eyesLookingUp;
            quaterUL = new Quaternion(eyesLooking.left.x, eyesLooking.left.y, eyesLooking.left.z, eyesLooking.left.w);
            quaterUR = new Quaternion(eyesLooking.right.x, eyesLooking.right.y, eyesLooking.right.z, eyesLooking.right.w);
            eyesLooking = sourceAvatarDescriptor.customEyeLookSettings.eyesLookingStraight;
            quaterSL = new Quaternion(eyesLooking.left.x, eyesLooking.left.y, eyesLooking.left.z, eyesLooking.left.w);
            quaterSR = new Quaternion(eyesLooking.right.x, eyesLooking.right.y, eyesLooking.right.z, eyesLooking.right.w);
#elif VRC_SDK_VRCSDK2
#endif

            VRC.Core.PipelineManager sourcePipelineManager = cloneGameObject.GetComponent<VRC.Core.PipelineManager>();

            dynamicBoneDict = new Dictionary<string, List<DynamicBone>>();

            DynamicBone[] dynamicBones;
            dynamicBones = cloneGameObject.GetComponentsInChildren<DynamicBone>();
            foreach (var dy in dynamicBones)
            {
                if (!dynamicBoneDict.ContainsKey(dy.gameObject.GetFullPath()))
                {
                    dynamicBoneDict.Add(dy.gameObject.GetFullPath(), new List<DynamicBone>());
                }
                dynamicBoneDict[dy.gameObject.GetFullPath()].Add(dy);
            }
            srcBlueprintId = sourcePipelineManager.blueprintId;


        }
        public void CopyToTarget(GameObject gameObject)
        {
            this.messages = new List<string>();
            this.errors = new List<string>();
            this.boneName = new List<string>();
            this.exclusionBoneName = new List<string>();

            boneName.Add("Hair");
            boneName.Add("Skirt");
            boneName.Add("Bust");
            boneName.Add("Sleeve");

            if (!dynamicBones_hair)
            {
                exclusionBoneName.Add("Hair");
            }
            if (!dynamicBones_skirt)
            {
                exclusionBoneName.Add("Skirt");
            }
            if (!dynamicBones_bust)
            {
                exclusionBoneName.Add("Bust");
            }
            if (!dynamicBones_sleeve)
            {
                exclusionBoneName.Add("Sleeve");
            }

#if VRC_SDK_VRCSDK3
            VRCAvatarDescriptor targetAvatarDescriptor = gameObject.GetComponent<VRCAvatarDescriptor>();
#elif VRC_SDK_VRCSDK2
            VRC_AvatarDescriptor targetAvatarDescriptor = gameObject.GetComponent<VRC_AvatarDescriptor>();
#endif

            if (viewPosition)
            {
                messages.Add("View Positionをコピー");

                targetAvatarDescriptor.ViewPosition.x = viewX;
                targetAvatarDescriptor.ViewPosition.y = viewY;
                targetAvatarDescriptor.ViewPosition.z = viewZ;
            }

#if VRC_SDK_VRCSDK3
            if (eyeMovements)
            {
                messages.Add("Eye Look - Eye Movementsをコピー");

                targetAvatarDescriptor.customEyeLookSettings.eyeMovement.confidence = confidence;
                targetAvatarDescriptor.customEyeLookSettings.eyeMovement.excitement = excitement;
            }

            if (rotationStates)
            {
                messages.Add("Eye Look - Rotation Statesをコピー");

                targetAvatarDescriptor.customEyeLookSettings.eyesLookingRight.left = quaterRL;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingRight.right = quaterRR;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingLeft.left = quaterLL;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingLeft.right = quaterLR;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingDown.left = quaterDL;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingDown.right = quaterDR;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingUp.left = quaterUL;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingUp.right = quaterUR;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingStraight.left = quaterSL;
                targetAvatarDescriptor.customEyeLookSettings.eyesLookingStraight.right = quaterSR;
            }
#elif VRC_SDK_VRCSDK2
#endif

            if (blueprintId)
            {
                messages.Add("BlueprintIDをコピー");

                VRC.Core.PipelineManager targetPipelineManager = gameObject.GetComponent<VRC.Core.PipelineManager>();
                targetPipelineManager.blueprintId = srcBlueprintId;
            }

            if (objects)
            {
                int obj_count = 0;
                Transform[] allchildren = cloneGameObject.GetComponentsInChildren<Transform>();
                foreach (Transform child in allchildren)
                {
                    if (child.gameObject.transform.parent != null)
                    {
                        if ((child.gameObject.transform.parent.gameObject.GetFullPath()).Equals(cloneGameObject.transform.name))
                        {
                            // 1階層目
                            if (gameObject.transform.Find(child.gameObject.name) == null)
                            {
                                GameObject new_child = UnityEngine.Object.Instantiate(child.gameObject, new Vector3(child.gameObject.transform.position.x, child.gameObject.transform.position.y, child.gameObject.transform.position.z), Quaternion.identity);
                                new_child.transform.rotation = new Quaternion(child.gameObject.transform.rotation.x, child.gameObject.transform.rotation.y, child.gameObject.transform.rotation.z, child.gameObject.transform.rotation.w);
                                new_child.transform.parent = gameObject.transform;
                                new_child.name = child.gameObject.name;
                                obj_count++;
                            }
                        }
                        else
                        {
                            // 2階層以下
                            if (gameObject.transform.Find(child.gameObject.GetFullPath()) == null)
                            {
                                GameObject new_child = UnityEngine.Object.Instantiate(child.gameObject, new Vector3(child.gameObject.transform.position.x, child.gameObject.transform.position.y, child.gameObject.transform.position.z), Quaternion.identity);
                                new_child.transform.rotation = new Quaternion(child.gameObject.transform.rotation.x, child.gameObject.transform.rotation.y, child.gameObject.transform.rotation.z, child.gameObject.transform.rotation.w);
                                new_child.transform.parent = gameObject.transform.Find(child.gameObject.transform.parent.GetFullPath()).transform;
                                new_child.name = child.gameObject.name;
                                obj_count++;
                            }
                        }
                    }
                }
                messages.Add("Objectをコピー（" + obj_count + "件）");
            }

            DynamicBoneCollider[] dynamicBoneColliders_array = null;
            if (dynamicBoneColiders)
            {
                int col_count = 0;

                dynamicBoneColliders_array = gameObject.GetComponentsInChildren<DynamicBoneCollider>();
                DynamicBoneCollider[] dynamicBoneColliders = null;
                dynamicBoneColliders = cloneGameObject.GetComponentsInChildren<DynamicBoneCollider>();
                foreach (var col in dynamicBoneColliders)
                {
                    ComponentUtility.CopyComponent(col);
                    ComponentUtility.PasteComponentAsNew(gameObject.transform.Find(col.gameObject.GetFullPath()).gameObject);
                    col_count++;
                }
                messages.Add("Dynamic Bone Coliderをコピー（" + col_count + "件）");
            }

            if ((avatarMode == 1 && dynamicBones) || (avatarMode == 0 && (exclusionBoneName.Count > 0 || dynamicBones_other)))
            {

                int bone_count = 0;
                Dictionary<string, List<DynamicBone>> targetBoneDict = new Dictionary<string, List<DynamicBone>>();

                DynamicBone[] dynamicBones;
                dynamicBones = gameObject.GetComponentsInChildren<DynamicBone>();
                foreach (var dyn in dynamicBones)
                {
                    if (avatarMode == 0)
                    {
                        if (dyn.m_Root != null && checkExclutionDynamicBoneContain(dyn.m_Root.name))
                        {
                            continue;
                        }
                        else if (dyn.m_Root != null && !checkDynamicBoneContain(dyn.m_Root.name) && !dynamicBones_other)
                        {
                            continue;
                        }
                    }
                    UnityEngine.Object.DestroyImmediate(dyn);

                }

                dynamicBones = cloneGameObject.GetComponentsInChildren<DynamicBone>();
                foreach (var dyn in dynamicBones)
                {
                    if (avatarMode == 0)
                    {
                        if (checkExclutionDynamicBoneContain(dyn.m_Root.name))
                        {
                            continue;
                        }
                        else if (!checkDynamicBoneContain(dyn.m_Root.name) && !dynamicBones_other)
                        {
                            continue;
                        }
                    }
                    ComponentUtility.CopyComponent(dyn);
                    GameObject targetObj = gameObject.transform.Find(dyn.gameObject.GetFullPath()).gameObject;
                    ComponentUtility.PasteComponentAsNew(targetObj);

                    DynamicBone[] d = targetObj.GetComponents<DynamicBone>();
                    DynamicBone newDynamicBone = d[d.Length - 1];

                    newDynamicBone.m_Root = gameObject.transform.Find(newDynamicBone.m_Root.gameObject.GetFullPath()).gameObject.transform;

                    List<DynamicBoneColliderBase> new_coliders = new List<DynamicBoneColliderBase>();
                    foreach (var tarcol in newDynamicBone.m_Colliders)
                    {
                        if (tarcol != null)
                        {
                            new_coliders.Add(gameObject.transform.Find(tarcol.gameObject.GetFullPath()).gameObject.GetComponent<DynamicBoneColliderBase>());
                        }
                    }
                    List<Transform> new_exclusions = new List<Transform>();
                    foreach (var tarexc in newDynamicBone.m_Exclusions)
                    {
                        if (tarexc != null)
                        {
                            new_exclusions.Add(tarexc);
                        }
                    }
                    newDynamicBone.m_Colliders = new_coliders;
                    newDynamicBone.m_Exclusions = new_exclusions;

                    if (newDynamicBone.m_ReferenceObject != null)
                    {
                        newDynamicBone.m_ReferenceObject = gameObject.transform.Find(newDynamicBone.m_ReferenceObject.gameObject.GetFullPath()).gameObject.transform;

                    }
                    bone_count++;
                }
                if (bone_count > 0)
                {
                    messages.Add("Dynamic Boneをコピー（" + bone_count + "件）");
                }
                if (dynamicBoneColiders && dynamicBoneColliders_array != null && dynamicBoneColliders_array.Length > 0)
                {

                    DynamicBone[] allDynamicBone = gameObject.GetComponentsInChildren<DynamicBone>();
                    List<Dictionary<string, object>> allBoneList = new List<Dictionary<string, object>>();
                    foreach (var bone in allDynamicBone)
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        dict.Add("bone", bone);

                        if (bone.m_Root != null)
                        {
                            dict.Add("m_Root", (object)bone.m_Root.gameObject.GetFullPath());
                        }
                        else
                        {
                            dict.Add("m_Root", null);
                        }

                        List<string> m_Colliders_list = new List<string>();
                        foreach (var col in bone.m_Colliders)
                        {
                            m_Colliders_list.Add(col.gameObject.GetFullPath());
                        }
                        dict.Add("m_Colliders", m_Colliders_list);

                        List<string> m_Exclusions_list = new List<string>();
                        foreach (var exc in bone.m_Exclusions)
                        {
                            m_Exclusions_list.Add(exc.gameObject.GetFullPath());
                        }
                        dict.Add("m_Exclusions", m_Exclusions_list);

                        if (bone.m_ReferenceObject != null)
                        {
                            dict.Add("m_ReferenceObject", (object)bone.m_ReferenceObject.gameObject.GetFullPath());
                        }
                        else
                        {
                            dict.Add("m_ReferenceObject", null);
                        }
                        allBoneList.Add(dict);
                    }

                    foreach (var col in dynamicBoneColliders_array)
                    {
                        UnityEngine.Object.DestroyImmediate(col);

                    }
                    foreach (var bonedic in allBoneList)
                    {
                        DynamicBone bone = (DynamicBone)bonedic["bone"];
                        string new_m_Root = (string)bonedic["m_Root"];
                        bone.m_Root = gameObject.transform.Find(new_m_Root).gameObject.transform;

                        List<DynamicBoneColliderBase> new_coliders = new List<DynamicBoneColliderBase>();
                        List<string> new_coliders_string = (List<string>)bonedic["m_Colliders"];
                        foreach (var tarcol_string in new_coliders_string)
                        {
                            if (tarcol_string != null)
                            {
                                new_coliders.Add(gameObject.transform.Find(tarcol_string).gameObject.GetComponent<DynamicBoneColliderBase>());
                            }
                        }

                        List<Transform> new_exclusions = new List<Transform>();
                        List<string> new_exclusions_string = (List<string>)bonedic["m_Exclusions"];
                        foreach (var tarexc_string in new_exclusions_string)
                        {
                            if (tarexc_string != null)
                            {
                                new_exclusions.Add(gameObject.transform.Find(tarexc_string));
                            }
                        }
                        bone.m_Colliders = new_coliders;
                        bone.m_Exclusions = new_exclusions;

                        string new_m_ReferenceObject = (string)bonedic["m_ReferenceObject"];
                        if (new_m_ReferenceObject != null)
                        {
                            bone.m_ReferenceObject = gameObject.transform.Find(new_m_ReferenceObject).gameObject.transform;

                        }
                    }
                }
            }

            UnityEngine.Object.DestroyImmediate(cloneGameObject);
        }
        public void Log()
        {
            Debug.Log("viewX : " + viewX);
            Debug.Log("viewY : " + viewY);
            Debug.Log("viewZ : " + viewZ);
        }
        private bool checkExclutionDynamicBoneContain(string str)
        {
            bool rtn = false;
            foreach (var name in exclusionBoneName)
            {
                rtn = rtn || str.Contains(name);
            }
            return rtn;
        }
        private bool checkDynamicBoneContain(string str)
        {
            bool rtn = false;
            foreach (var name in boneName)
            {
                rtn = rtn || str.Contains(name);
            }
            return rtn;
        }
    }
    public static class Extensions
    {
        public static string GetFullPath(this GameObject obj)
        {
            return GetFullPath(obj.transform);
        }

        public static string GetFullPath(this Transform t)
        {
            string path = t.name;
            var parent = t.parent;
            while (parent)
            {
                if (parent.parent == null)
                {
                    return path;
                }
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }
    }
}