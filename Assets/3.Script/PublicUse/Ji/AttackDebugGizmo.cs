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

    public float angle;
    public Vector3 direction;
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
            DrawGizmo(enemy.DebugInfo);
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

    private void DrawGizmo(AttackDebugInfo info)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(info.color.r, info.color.g, info.color.b, 0.3f);

        Vector3 startDir = Quaternion.AngleAxis(-info.angle * 0.5f, Vector3.up) * info.direction;

        UnityEditor.Handles.DrawSolidArc(info.center, Vector3.up, startDir, info.angle, info.halfExtents.x);

        UnityEditor.Handles.color = info.color;
        UnityEditor.Handles.DrawWireArc(info.center, Vector3.up, startDir, info.angle, info.halfExtents.x);
#endif
    }
}
