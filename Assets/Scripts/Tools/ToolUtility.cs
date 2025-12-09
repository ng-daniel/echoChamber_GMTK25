using System;
using UnityEngine;


namespace Tools
{
    public struct AimToolOutput
    {
        public Quaternion gunRotation;
        public bool flipToolY;
    }

    public static class ToolUtility
    {
        public static AimToolOutput AimTool(Vector2 direction)
        {
            AimToolOutput output;

            // rotation
            float angleDegRaw = Vector2.Angle(direction, Vector2.right);
            float angleDeg = direction.y > 0 ? angleDegRaw : 180 + (180 - angleDegRaw);
            output.gunRotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));

            // flip character and gun graphics accordingly
            bool shouldFlip = direction.x < 0;
            output.flipToolY = shouldFlip;

            return output;
        }

        public static void AimToolAutoApply(Vector2 direction, Transform toolTransform, SpriteRenderer toolSprite)
        {
            var aimOutput = AimTool(direction);
            toolTransform.rotation = aimOutput.gunRotation;
            toolSprite.flipY = aimOutput.flipToolY;
        }

        public static void SetDistFromBody(Transform user, Transform tool, Vector2 direction, float distanceFromUser)
        {
            Vector2 offset = direction.normalized * distanceFromUser;
            tool.position = (Vector2)user.position + offset;
        }
    }
}