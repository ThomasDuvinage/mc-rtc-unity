using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace McRtc
{
    [ExecuteAlways]
    public class Robot : Element
    {
        public float alpha = 1.0f;

        private GameObject AddEmptyMesh(GameObject body, string name)
        {
            GameObject mesh = new GameObject(name, typeof(Tag));
            mesh.transform.parent = body.transform;
            mesh.GetComponent<Tag>().mcTag = "McRtcRobotMesh";
            return mesh;
        }

        private GameObject ImportMesh(GameObject body, string name, string path, float scale)
        {
            Transform mesh_tf = body.transform.Find(name);
            if (mesh_tf)
            {
                return mesh_tf.gameObject;
            }
            if (System.IO.File.Exists(path + ".fbx"))
            {
                return ImportMesh(body, name, path + ".fbx", scale);
            }
            if (!System.IO.File.Exists(path))
            {
                Debug.LogError($"Cannot load mest at {path} for robot {this.name}");
                return AddEmptyMesh(body, name);
            }
            string mesh = System.IO.Path.GetFileName(path);
            string destination_path = $"{Application.dataPath}/McRtc/{this.name}/{mesh}";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination_path));
            if (!System.IO.File.Exists(destination_path))
            {
                System.IO.File.Copy(path, destination_path);
                string mesh_path = System.IO.Path.GetDirectoryName(path);
                var textures = System.IO.Directory.EnumerateFiles(mesh_path, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) || s.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase));
                foreach (var text in textures)
                {
                    var text_out = text.Replace(mesh_path, "");
                    destination_path = $"{Application.dataPath}/McRtc/{this.name}/{text_out}";
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination_path));
                    if (!System.IO.File.Exists(destination_path))
                    {
                        System.IO.File.Copy(text, destination_path);
                    }
                }
                AssetDatabase.Refresh();
                ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath($"Assets/McRtc/{this.name}/{mesh}");
                if (importer)
                {
                    importer.bakeAxisConversion = true;
                    importer.globalScale = scale;
                    importer.SaveAndReimport();
                    AssetDatabase.Refresh();
                }
            }
            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/McRtc/{this.name}/{mesh}", typeof(GameObject));
            if (!obj)
            {
                return AddEmptyMesh(body, name);
            }
            GameObject m = Object.Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
            m.name = name;
            m.transform.parent = body.transform;
            m.AddComponent<Tag>().mcTag = "McRtcRobotMesh";
            return m;
        }

        private GameObject FindBody(string body)
        {
            Transform body_tf = transform.Find(body);
            if (body_tf)
            {
                return body_tf.gameObject;
            }
            GameObject obj = new GameObject(body, typeof(Tag));
            obj.transform.parent = transform;
            obj.GetComponent<Tag>().mcTag = "McRtcRobotBody";
            return obj;
        }

        public void UpdateBody(string body, PTransform X_0_body)
        {
            X_0_body.SetLocalTransform(FindBody(body));
        }

        public void UpdateMesh(string body, string name, string path, float scale, PTransform X_body_visual)
        {
            GameObject mesh = ImportMesh(FindBody(body), name, path, scale);
            X_body_visual.SetLocalTransform(mesh);
            MeshRenderer[] renderers = null;
            MeshRenderer r = mesh.GetComponent<MeshRenderer>();
            if (r)
            {
                renderers = new MeshRenderer[]{r};
            }
            else
            {
                renderers = mesh.GetComponentsInChildren<MeshRenderer>();
            }
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer == null)
                {
                    return;
                }
                Material material = null;
                if (!Application.IsPlaying(this))
                {
                    material = renderer.sharedMaterial;
                }
                else
                {
                    material = renderer.material;
                }
                if (!material)
                {
                    return;
                }
                // Material setting from https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs
                if (!material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON") && alpha < 1.0F)
                {
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetFloat("_Mode", 3);
                    material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_ZWrite", 0.0f);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else if (material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON") && alpha >= 1.0F)
                {
                    material.SetOverrideTag("RenderType", "");
                    material.SetFloat("_Mode", 0);
                    material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat("_ZWrite", 1.0f);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                }
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }

        protected override void OnDisconnect()
        {
            Tag[] tags = GetComponentsInChildren<Tag>();
            foreach(Tag tag in tags)
            {
                if(tag.mcTag == "McRtcRobotBody")
                {
                    GameObject.DestroyImmediate(tag.gameObject);
                }
            }
        }
    }
}