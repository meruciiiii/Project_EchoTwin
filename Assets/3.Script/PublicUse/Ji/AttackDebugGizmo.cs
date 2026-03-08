using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum AttackShape
{
    box,
    sphere,
    sector,
}

[System.Serializable]
public struct AttackDebugInfo
{
    public AttackShape shape;
    public Vector3 center;
    public Vector3 size; // box : extents , sphere/sector : radius
    public Quaternion rotation;
    public Color color;

    public float angle;
    public Vector3 direction;

    [Range(0f, 1f)]
    public float ratio;
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
            DrawAttack(mainWeapon.DebugInfo);
        }

        //if (subWeapon != null)
        //{
        //    IReadOnlyList<AttackDebugInfo> echos = subWeapon.EchoAttackInfos;
        //    if (echos != null)
        //    {
        //        foreach (AttackDebugInfo info in echos)
        //        {
        //            DrawBox(info);
        //        }
        //    }
        //
        //}

        if (subWeapon != null && subWeapon.EchoAttackInfos != null)
        {
            foreach (AttackDebugInfo info in subWeapon.EchoAttackInfos)
            {
                DrawAttack(info);
            }
        }

        if (enemy != null)
        {
            foreach (AttackDebugInfo info in enemy.getAllDebugInfo())
            {
                DrawAttack(info);
            }
        }
    }

    private void DrawAttack(AttackDebugInfo info)
    {
        if (info.shape == AttackShape.box &&
        info.rotation.x == 0f &&
        info.rotation.y == 0f &&
        info.rotation.z == 0f &&
        info.rotation.w == 0f)
        {
            return;
        }

        Gizmos.color = info.color;
        Matrix4x4 old = Gizmos.matrix;

        switch (info.shape)
        {
            case AttackShape.box:
                Gizmos.matrix = Matrix4x4.TRS(info.center, info.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, info.size * 2f);

                if (info.ratio > 0)
                {
                    Gizmos.color = new Color(info.color.r, info.color.g, info.color.b, 0.3f * info.ratio);
                    Gizmos.DrawCube(Vector3.zero, info.size * 2f);
                }
                break;
            case AttackShape.sphere:
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawWireSphere(info.center, info.size.x);

                if (info.ratio > 0)
                {
                    Gizmos.color = new Color(info.color.r, info.color.g, info.color.b, 0.3f * info.ratio);
                    Gizmos.DrawSphere(info.center, info.size.x);
                }
                break;
            case AttackShape.sector:
                DrawSectorGizmo(info);
                break;
        }

        Gizmos.matrix = old;


        //Gizmos.matrix = Matrix4x4.TRS(info.center, info.rotation, Vector3.one);

        //Gizmos.DrawWireCube(Vector3.zero, info.halfExtents * 2f);
        //Gizmos.matrix = old;
    }

    private void DrawSectorGizmo(AttackDebugInfo info)
    {
#if UNITY_EDITOR
        Handles.color = info.color;

        Vector3 startDir = Quaternion.AngleAxis(-info.angle * 0.5f, Vector3.up) * info.direction;//ºÎÃ¤²Ã ¿ÞÂÊ ±âµÕ
        Handles.DrawWireArc(info.center, Vector3.up, startDir, info.angle, info.size.x);

        Vector3 endDir = Quaternion.AngleAxis(info.angle, Vector3.up) * startDir;//ºÎÃ¤²Ã ¿À¸¥ÂÊ ±âµÕ
        Handles.DrawLine(info.center, info.center + startDir * info.size.x);
        Handles.DrawLine(info.center, info.center + endDir * info.size.x);

        if (info.ratio > 0)
        {
            Color fillColor = info.color;
            fillColor.a = 0.4f;
            Handles.color = fillColor;

            Handles.DrawSolidArc(info.center, Vector3.up, startDir, info.angle, info.size.x * info.ratio);
        }
#endif
    }

    //    private void DrawCircle(AttackDebugInfo info)
    //    {
    //#if UNITY_EDITOR
    //        UnityEditor.Handles.color = new Color(info.color.r, info.color.g, info.color.b, 0.5f);

    //        Vector3 startDir = Quaternion.AngleAxis(-info.angle * 0.5f, Vector3.up) * info.direction;

    //        UnityEditor.Handles.DrawSolidArc(info.center, Vector3.up, startDir, info.angle, info.halfExtents.x);

    //        UnityEditor.Handles.color = info.color;
    //        UnityEditor.Handles.DrawWireArc(info.center, Vector3.up, startDir, info.angle, info.halfExtents.x);
    //#endif
    //    }

    //    private void DrawGizmo(AttackDebugInfo info)
    //    {
    //#if UNITY_EDITOR
    //        UnityEditor.Handles.color = new Color(info.color.r, info.color.g, info.color.b, 0.5f);

    //        Vector3 startDir = Quaternion.AngleAxis(-info.angle * 0.5f, Vector3.up) * info.direction;

    //        UnityEditor.Handles.DrawSolidArc(info.center, Vector3.up, startDir, info.angle, info.halfExtents.x);

    //        UnityEditor.Handles.color = info.color;
    //        UnityEditor.Handles.DrawWireArc(info.center, Vector3.up, startDir, info.angle, info.halfExtents.x);
    //#endif
    //    }
}
