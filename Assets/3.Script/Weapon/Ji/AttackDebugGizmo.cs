using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct AttackDebugInfo
{
    public Vector3 center;
    public Vector3 halfExtents;
    public Quaternion rotation;
    public Color color;
}
public class AttackDebugGizmo : MonoBehaviour
{
    public WeaponAbstract mainWeapon;
    public WeaponAbstract subWeapon;

    private void OnDrawGizmosSelected()
    {
        if (mainWeapon == null) return;
        if (!mainWeapon.HasDebugInfo) return;

        Debug.Log("gizmo");

        AttackDebugInfo info = mainWeapon.DebugInfo;

        Gizmos.color = info.color;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(info.center, info.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, info.halfExtents * 2f);
        Gizmos.matrix = old;
    }

    private void OnDrawGizmos()
    {
        if (subWeapon == null) return;

        if(mainWeapon.HasDebugInfo)
        {
            DrawBox(mainWeapon.DebugInfo);
        }

        IReadOnlyList<AttackDebugInfo> echos = subWeapon.EchoAttackInfos;
        if (echos == null) return;

        foreach(AttackDebugInfo info in echos)
        {
            DrawBox(info);
        }
    }

    private void DrawBox(AttackDebugInfo info)
    {
        Gizmos.color = info.color;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(info.center, info.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, info.halfExtents * 2f);
        Gizmos.matrix = old;
    }
}
