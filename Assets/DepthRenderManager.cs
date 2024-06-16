using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class DepthRenderManager : MonoBehaviour
{
    public RenderTexture m_kRtDepthTexture;

    public Texture2D m_kDepthTexture;

    // Update is called once per frame
    void Update()
    {
        var cam = GetComponent<Camera>();
        if(cam == null)
        {
            return;
        }
        Shader.SetGlobalMatrix(Shader.PropertyToID("_gLightVP"), GL.GetGPUProjectionMatrix(cam.projectionMatrix,true) * cam.worldToCameraMatrix);
        if(m_kDepthTexture != null)
        {
            Shader.SetGlobalTexture(Shader.PropertyToID("_gDepthTex"), m_kDepthTexture);
        }
        else
        {
            Shader.SetGlobalTexture(Shader.PropertyToID("_gDepthTex"), m_kRtDepthTexture);
        }
    }
    #if UNITY_EDITOR
    [ContextMenu("Save Depth Texture")]
    void SaveDepthTexture()
    {
        var cam = GetComponent<Camera>();
        if(cam == null)
        {
            return;
        }
        RenderTexture.active = m_kRtDepthTexture;
        Texture2D tex = new Texture2D(m_kRtDepthTexture.width, m_kRtDepthTexture.height, TextureFormat.RHalf, false);
        tex.ReadPixels(new Rect(0, 0, m_kRtDepthTexture.width, m_kRtDepthTexture.height), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToEXR(Texture2D.EXRFlags.None);

        Scene curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        string scenePath = curScene.path;
        
        System.IO.File.WriteAllBytes(scenePath.Replace(".unity","") + "_lightdepth.exr", bytes);
        UnityEditor.AssetDatabase.Refresh();
    }
    #endif
    
}
