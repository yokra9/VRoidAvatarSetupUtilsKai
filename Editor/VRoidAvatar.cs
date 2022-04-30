using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.CustomEyeLookSettings;

namespace Jirko.Unity.VRoidAvatarUtils
{
    public class VRoidAvatar
    {
        private GameObject cloneGameObject = null;
        private Dictionary<string, List<VRCPhysBone>> physBoneDict;
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

        public VRCAvatarDescriptor.CustomAnimLayer[] srcBaseAnimationLayers;
        public VRCAvatarDescriptor.CustomAnimLayer[] srcSpecialAnimationLayers;
        public VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu srcExpressionsMenu;
        public VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters srcExpressionParameters;

        public int avatarMode = 0;
        public bool viewPosition = true;
        public bool eyeMovements = true;
        public bool rotationStates = true;
        public bool blueprintId = true;
        public bool physBones = true;
        public bool physBones_hair = true;
        public bool physBones_skirt = true;
        public bool physBones_bust = true;
        public bool physBones_sleeve = true;
        public bool physBones_other = true;
        public bool physBoneColiders = true;
        public bool objects = true;
        public bool baseAnimationLayers = true;
        public bool specialAnimationLayers = true;
        public bool expressionsMenu = true;
        public bool expressionParameters = true;
        public bool aimConstraint = true;
        public bool lookAtConstraint = true;
        public bool parentConstraint = true;
        public bool positionConstraint = true;
        public bool rotationConstraint = true;
        public bool scaleConstraint = true;

        public VRoidAvatar(GameObject gameObject)
        {
            cloneGameObject = UnityEngine.Object.Instantiate(gameObject);
            this.InitDTO();
        }
        void InitDTO()
        {

            VRCAvatarDescriptor sourceAvatarDescriptor = null;
            sourceAvatarDescriptor = cloneGameObject.GetComponent<VRCAvatarDescriptor>();

            viewX = sourceAvatarDescriptor.ViewPosition.x;
            viewY = sourceAvatarDescriptor.ViewPosition.y;
            viewZ = sourceAvatarDescriptor.ViewPosition.z;

            confidence = sourceAvatarDescriptor.customEyeLookSettings.eyeMovement.confidence;
            excitement = sourceAvatarDescriptor.customEyeLookSettings.eyeMovement.excitement;
            srcBaseAnimationLayers = sourceAvatarDescriptor.baseAnimationLayers;
            srcSpecialAnimationLayers = sourceAvatarDescriptor.specialAnimationLayers;
            srcExpressionsMenu = sourceAvatarDescriptor.expressionsMenu;
            srcExpressionParameters = sourceAvatarDescriptor.expressionParameters;

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

            VRC.Core.PipelineManager sourcePipelineManager = cloneGameObject.GetComponent<VRC.Core.PipelineManager>();

            physBoneDict = new Dictionary<string, List<VRCPhysBone>>();

            VRCPhysBone[] physBones;
            physBones = cloneGameObject.GetComponentsInChildren<VRCPhysBone>();
            foreach (var ph in physBones)
            {
                if (!physBoneDict.ContainsKey(ph.gameObject.GetFullPath()))
                {
                    physBoneDict.Add(ph.gameObject.GetFullPath(), new List<VRCPhysBone>());
                }
                physBoneDict[ph.gameObject.GetFullPath()].Add(ph);
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

            if (!physBones_hair)
            {
                exclusionBoneName.Add("Hair");
            }
            if (!physBones_skirt)
            {
                exclusionBoneName.Add("Skirt");
            }
            if (!physBones_bust)
            {
                exclusionBoneName.Add("Bust");
            }
            if (!physBones_sleeve)
            {
                exclusionBoneName.Add("Sleeve");
            }

            VRCAvatarDescriptor targetAvatarDescriptor = gameObject.GetComponent<VRCAvatarDescriptor>();

            if (viewPosition)
            {
                messages.Add("View Positionをコピー");

                targetAvatarDescriptor.ViewPosition.x = viewX;
                targetAvatarDescriptor.ViewPosition.y = viewY;
                targetAvatarDescriptor.ViewPosition.z = viewZ;
            }

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

            if (baseAnimationLayers)
            {
                messages.Add("BaseAnimationLayersをコピー");
                foreach (var layer in srcBaseAnimationLayers)
                {
                    switch (layer.type)
                    {
                        case VRCAvatarDescriptor.AnimLayerType.Base:
                            targetAvatarDescriptor.baseAnimationLayers[0] = layer;
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.Additive:
                            targetAvatarDescriptor.baseAnimationLayers[1] = layer;
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.Gesture:
                            targetAvatarDescriptor.baseAnimationLayers[2] = layer;
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.Action:
                            targetAvatarDescriptor.baseAnimationLayers[3] = layer;
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.FX:
                            targetAvatarDescriptor.baseAnimationLayers[4] = layer;
                            break;
                    }
                }
            }

            if (specialAnimationLayers)
            {
                messages.Add("SpecialAnimationLayersをコピー");
                foreach (var layer in srcSpecialAnimationLayers)
                {
                    switch (layer.type)
                    {
                        case VRCAvatarDescriptor.AnimLayerType.Sitting:
                            targetAvatarDescriptor.specialAnimationLayers[0] = layer;
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.TPose:
                            targetAvatarDescriptor.specialAnimationLayers[1] = layer;
                            break;
                        case VRCAvatarDescriptor.AnimLayerType.IKPose:
                            targetAvatarDescriptor.specialAnimationLayers[2] = layer;
                            break;
                    }
                }
            }

            if (expressionsMenu)
            {
                messages.Add("ExpressionsMenuをコピー");
                targetAvatarDescriptor.expressionsMenu = srcExpressionsMenu;
            }

            if (expressionParameters)
            {
                messages.Add("ExpressionParametersをコピー");
                targetAvatarDescriptor.expressionParameters = srcExpressionParameters;
            }

            if (blueprintId)
            {
                messages.Add("BlueprintIDをコピー");

                VRC.Core.PipelineManager targetPipelineManager = gameObject.GetComponent<VRC.Core.PipelineManager>();
                targetPipelineManager.blueprintId = srcBlueprintId;
            }

            if (objects)
            {
                int obj_count = 0;
                const bool includeInactive = true;
                Transform[] allchildren = cloneGameObject.GetComponentsInChildren<Transform>(includeInactive);
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

            VRCPhysBoneCollider[] physBoneColliders_array = null;
            if (physBoneColiders)
            {
                int col_count = 0;

                physBoneColliders_array = gameObject.GetComponentsInChildren<VRCPhysBoneCollider>();
                VRCPhysBoneCollider[] physBoneColliders = null;
                physBoneColliders = cloneGameObject.GetComponentsInChildren<VRCPhysBoneCollider>();
                foreach (var col in physBoneColliders)
                {
                    ComponentUtility.CopyComponent(col);
                    VRCPhysBoneCollider target = gameObject.transform.Find(col.gameObject.GetFullPath()).gameObject.AddComponent<VRCPhysBoneCollider>();
                    ComponentUtility.PasteComponentValues(target);
                    if (col.rootTransform) target.rootTransform = gameObject.transform.Find(col.rootTransform.GetFullPath());
                    col_count++;
                }
                messages.Add("Phys Bone Coliderをコピー（" + col_count + "件）");
            }

            if ((avatarMode == 1 && physBones) || (avatarMode == 0 && (exclusionBoneName.Count > 0 || physBones_other)))
            {

                int bone_count = 0;
                Dictionary<string, List<VRCPhysBone>> targetBoneDict = new Dictionary<string, List<VRCPhysBone>>();

                VRCPhysBone[] physBones;
                physBones = gameObject.GetComponentsInChildren<VRCPhysBone>();
                foreach (var phy in physBones)
                {
                    if (avatarMode == 0)
                    {
                        if (phy.rootTransform != null && checkExclutionPhysBoneContain(phy.rootTransform.name))
                        {
                            continue;
                        }
                        else if (phy.rootTransform != null && !checkPhysBoneContain(phy.rootTransform.name) && !physBones_other)
                        {
                            continue;
                        }
                    }
                    UnityEngine.Object.DestroyImmediate(phy);

                }

                physBones = cloneGameObject.GetComponentsInChildren<VRCPhysBone>();
                foreach (var phy in physBones)
                {
                    if (avatarMode == 0)
                    {
                        if (checkExclutionPhysBoneContain(phy.rootTransform.name))
                        {
                            continue;
                        }
                        else if (!checkPhysBoneContain(phy.rootTransform.name) && !physBones_other)
                        {
                            continue;
                        }
                    }
                    ComponentUtility.CopyComponent(phy);
                    GameObject targetObj = gameObject.transform.Find(phy.gameObject.GetFullPath()).gameObject;
                    ComponentUtility.PasteComponentAsNew(targetObj);

                    VRCPhysBone[] p = targetObj.GetComponents<VRCPhysBone>();
                    VRCPhysBone newPhysBone = p[p.Length - 1];

                    newPhysBone.rootTransform = gameObject.transform.Find(newPhysBone.rootTransform.gameObject.GetFullPath()).gameObject.transform;

                    List<VRCPhysBoneColliderBase> new_coliders = new List<VRCPhysBoneColliderBase>();
                    foreach (var tarcol in newPhysBone.colliders)
                    {
                        if (tarcol != null)
                        {
                            new_coliders.Add(gameObject.transform.Find(tarcol.gameObject.GetFullPath()).gameObject.GetComponent<VRCPhysBoneColliderBase>());
                        }
                    }
                    List<Transform> new_ignoreTransforms = new List<Transform>();
                    foreach (var tarexc in newPhysBone.ignoreTransforms)
                    {
                        if (tarexc != null)
                        {
                            new_ignoreTransforms.Add(tarexc);
                        }
                    }
                    newPhysBone.colliders = new_coliders;
                    newPhysBone.ignoreTransforms = new_ignoreTransforms;

                    bone_count++;
                }
                if (bone_count > 0)
                {
                    messages.Add("Phys Boneをコピー（" + bone_count + "件）");
                }
                if (physBoneColiders && physBoneColliders_array != null && physBoneColliders_array.Length > 0)
                {

                    VRCPhysBone[] allPhysBone = gameObject.GetComponentsInChildren<VRCPhysBone>();
                    List<Dictionary<string, object>> allBoneList = new List<Dictionary<string, object>>();
                    foreach (var bone in allPhysBone)
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        dict.Add("bone", bone);

                        if (bone.rootTransform != null)
                        {
                            dict.Add("rootTransform", (object)bone.rootTransform.gameObject.GetFullPath());
                        }
                        else
                        {
                            dict.Add("rootTransform", null);
                        }

                        List<string> colliders_list = new List<string>();
                        foreach (var col in bone.colliders)
                        {
                            colliders_list.Add(col.gameObject.GetFullPath());
                        }
                        dict.Add("colliders", colliders_list);

                        List<string> ignoreTransforms_list = new List<string>();
                        foreach (var exc in bone.ignoreTransforms)
                        {
                            ignoreTransforms_list.Add(exc.gameObject.GetFullPath());
                        }
                        dict.Add("ignoreTransforms", ignoreTransforms_list);

                        allBoneList.Add(dict);
                    }

                    foreach (var col in physBoneColliders_array)
                    {
                        UnityEngine.Object.DestroyImmediate(col);

                    }
                    foreach (var bonedic in allBoneList)
                    {
                        VRCPhysBone bone = (VRCPhysBone)bonedic["bone"];
                        string new_rootTransform = (string)bonedic["rootTransform"];
                        bone.rootTransform = gameObject.transform.Find(new_rootTransform).gameObject.transform;

                        List<VRCPhysBoneColliderBase> new_coliders = new List<VRCPhysBoneColliderBase>();
                        List<string> new_coliders_string = (List<string>)bonedic["colliders"];
                        foreach (var tarcol_string in new_coliders_string)
                        {
                            if (tarcol_string != null)
                            {
                                new_coliders.Add(gameObject.transform.Find(tarcol_string).gameObject.GetComponent<VRCPhysBoneColliderBase>());
                            }
                        }

                        List<Transform> new_ignoreTransforms = new List<Transform>();
                        List<string> new_ignoreTransforms_string = (List<string>)bonedic["ignoreTransforms"];
                        foreach (var tarexc_string in new_ignoreTransforms_string)
                        {
                            if (tarexc_string != null)
                            {
                                new_ignoreTransforms.Add(gameObject.transform.Find(tarexc_string));
                            }
                        }
                        bone.colliders = new_coliders;
                        bone.ignoreTransforms = new_ignoreTransforms;

                    }
                }
            }

            if (aimConstraint || lookAtConstraint || parentConstraint || positionConstraint || rotationConstraint || scaleConstraint)
            {
                int constraints_count = 0;
                void setConstraint<T>(T[] constraints) where T : Behaviour, IConstraint
                {
                    foreach (T constraint in constraints)
                    {
                        List<ConstraintSource> dest = new List<ConstraintSource>();
                        List<ConstraintSource> from = new List<ConstraintSource>();
                        constraint.GetSources(from);

                        foreach (ConstraintSource f in from)
                        {
                            ConstraintSource d = new ConstraintSource();
                            d.sourceTransform = gameObject.transform.Find(f.sourceTransform.gameObject.GetFullPath());
                            d.weight = f.weight;
                            dest.Add(d);
                        }

                        Transform target = gameObject.transform.Find(constraint.gameObject.GetFullPath());
                        target.GetComponent<T>().SetSources(dest);

                        constraints_count++;
                    }
                }

                bool includeInactive = true;
                if (aimConstraint) setConstraint(cloneGameObject.GetComponentsInChildren<AimConstraint>(includeInactive));
                if (lookAtConstraint) setConstraint(cloneGameObject.GetComponentsInChildren<LookAtConstraint>(includeInactive));
                if (parentConstraint) setConstraint(cloneGameObject.GetComponentsInChildren<ParentConstraint>(includeInactive));
                if (positionConstraint) setConstraint(cloneGameObject.GetComponentsInChildren<PositionConstraint>(includeInactive));
                if (rotationConstraint) setConstraint(cloneGameObject.GetComponentsInChildren<RotationConstraint>(includeInactive));
                if (scaleConstraint) setConstraint(cloneGameObject.GetComponentsInChildren<ScaleConstraint>(includeInactive));

                messages.Add("Constraintsをコピー（" + constraints_count + "件）");
            }

            UnityEngine.Object.DestroyImmediate(cloneGameObject);
        }
        public void Log()
        {
            Debug.Log("viewX : " + viewX);
            Debug.Log("viewY : " + viewY);
            Debug.Log("viewZ : " + viewZ);
        }
        private bool checkExclutionPhysBoneContain(string str)
        {
            bool rtn = false;
            foreach (var name in exclusionBoneName)
            {
                rtn = rtn || str.Contains(name);
            }
            return rtn;
        }
        private bool checkPhysBoneContain(string str)
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