// Copyright (C) 2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;

public class MerUtilities
{
    // Calculates the world space center and radius of a given collider.
    public static void GetCenterAndRadiusOfCollider(Collider collider, out Vector3 center, out float radius)
    {
        Vector3 scale = collider.transform.localScale;  // TODO: Is this correct?!
        if (collider is SphereCollider)
        {
            SphereCollider sphereCollider = (SphereCollider)collider;
            Bounds bounds = collider.bounds;
            center = bounds.center;
            radius = sphereCollider.radius * Mathf.Max(Mathf.Max(scale.x, scale.y), scale.z);
        }
        else
        {
            Bounds bounds = collider.bounds;
            center = bounds.center;
            Vector3 extents = bounds.extents;
            radius = extents.magnitude;
        }
    }

    // Calculates the Entity center and radius from a collider
    public static void GetMercunaEntityCenterAndRadiusFromCollider(Collider collider,
        out Vector3 center, out float radius)
    {
        GetCenterAndRadiusOfCollider(collider, out Vector3 colliderCenter, out float colliderRadius);

        if (collider.attachedRigidbody && !collider.attachedRigidbody.isKinematic)
        {
            center = collider.attachedRigidbody.worldCenterOfMass;
            radius = (colliderCenter - center).magnitude + colliderRadius;
        }
        else
        {
            center = collider.transform.position;
            radius = (colliderCenter - center).magnitude + colliderRadius;
        }
    }
}
