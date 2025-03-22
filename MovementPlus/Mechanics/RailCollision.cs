using Reptile;
using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class RailCollision
    {
        private static Vector3 previousPosition;
        private static Vector3 currentPosition;
        private static float playerSpeed;

        public static void Update()
        {
            if (MovementPlusPlugin.player.isAI || !MovementPlusPlugin.ConfigSettings.RailGeneral.Detection.Value) { return; }
            previousPosition = currentPosition;
            currentPosition = MovementPlusPlugin.player.transform.position;

            playerSpeed = MovementPlusPlugin.player.GetTotalSpeed();

            RaycastHit[] hits = LogHitsBetweenPositions();
            DetectMissedGrinds(hits);
        }

        private static RaycastHit[] LogHitsBetweenPositions()
        {
            Vector3 direction = currentPosition - previousPosition;
            float distance = direction.magnitude;
            return Physics.RaycastAll(previousPosition, direction, distance);
        }

        private static void DetectMissedGrinds(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 11)
                {
                    GrindLine grindLine = hit.collider.gameObject.GetComponent<GrindLine>();

                    if (grindLine != null && MovementPlusPlugin.player.grindAbility.CanSetToLine(grindLine))
                    {
                        float distanceToRail = Vector3.Distance(currentPosition, hit.point);

                        if (distanceToRail <= playerSpeed * Time.deltaTime)
                        {
                            MovementPlusPlugin.player.grindAbility.SetToLine(grindLine);
                            break;
                        }
                    }
                }
            }
        }
    }
}