using System.Collections.Generic;
using UnityEngine;
using TMPro;
//ここはクラスを跨いで細々とした処理を記述する所です。
public static class Utility
{
    public static void ChangeEnabledColliders(List<Collider> colliders,bool isEnable)
    {
        foreach(Collider collider in colliders)
        {
            if(collider == null) continue; 
            collider.enabled = isEnable;
        }
    }
    public static void ChangeEnabledColliders(Collider[] colliders,bool isEnable)
    {
        foreach(Collider collider in colliders)
        {
            if(collider == null) continue; 
            collider.enabled = isEnable;
        }
    }
    public static void ChangeEnabledMeshRenderers(List<MeshRenderer> meshRenderers,bool isEnable)
    {
        foreach(MeshRenderer meshRenderer in meshRenderers)
        {
            if(meshRenderer == null) continue; 
            meshRenderer.enabled = isEnable;
        }
    }
    public static void ChangeEnabledMeshRenderers(MeshRenderer[] meshRenderers,bool isEnable)
    {
        foreach(MeshRenderer meshRenderer in meshRenderers)
        {
            if(meshRenderer == null) continue; 
            meshRenderer.enabled = isEnable;
        }
    }
    public static void ChangeEnabledTMPorMeshRenderers(List<TMPandMeshRenderer> tMPandMeshRenderers,bool isEnable)
    {
        foreach(TMPandMeshRenderer tMPandMeshRenderer in tMPandMeshRenderers)
        {
            if(tMPandMeshRenderers == null) continue; 
            foreach(MeshRenderer meshRenderer in tMPandMeshRenderer.MeshRendererList)
            {
                if(meshRenderer == null) continue; 
                meshRenderer.enabled = isEnable;
            }
            foreach(TextMeshPro tmp in tMPandMeshRenderer.TextMeshProList)
            {
                if(tmp == null) continue; 
                tmp.enabled = isEnable;
            }

        }
    }

    public static GameObject GetCollisionGameObject(Collision collision)
    {
        if(collision.collider.attachedRigidbody == null)
        {
            return collision.collider.gameObject;
        }
        else
        {
            return collision.collider.attachedRigidbody.gameObject;
        }
    }


}
