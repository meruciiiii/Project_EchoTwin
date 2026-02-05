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

    public EnemyStateAbstract enemy;

    private void OnDrawGizmos()
    {
        if (mainWeapon != null && mainWeapon.HasDebugInfo)
        {
            DrawBox(mainWeapon.DebugInfo);
        }

        if (subWeapon != null)
        {
            IReadOnlyList<AttackDebugInfo> echos = subWeapon.EchoAttackInfos;
            if (echos != null)
            {
                foreach (AttackDebugInfo info in echos)
                {
                    DrawBox(info);
                }
            }

        }

        if (enemy != null && enemy.HasDebugInfo)
        {
            DrawSphere(enemy.DebugInfo);
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

    private void DrawSphere(AttackDebugInfo info)
    {
        Gizmos.color = info.color;

        Gizmos.DrawSphere(info.center, info.halfExtents.x);
    }
}
