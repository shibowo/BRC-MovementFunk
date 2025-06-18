using Reptile;
using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class RailCollision
    {
        private static Vector3 previousPosition;
        private static Vector3 currentPosition;
        private static float playerSpeed;

        public static void Update()
        {
            if (MovementFunkPlugin.player.isAI || !MovementFunkPlugin.MovementSettings.RailGeneral.Detection.Value) { return; }
            previousPosition = currentPosition;
            currentPosition = MovementFunkPlugin.player.transform.position;

            playerSpeed = MovementFunkPlugin.player.GetTotalSpeed();

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

                    if (grindLine != null && MovementFunkPlugin.player.grindAbility.CanSetToLine(grindLine))
                    {
                        float distanceToRail = Vector3.Distance(currentPosition, hit.point);

                        if (distanceToRail <= playerSpeed * Time.deltaTime)
                        {
                            MovementFunkPlugin.player.grindAbility.SetToLine(grindLine);
                            break;
                        }
                    }
                }
            }
        }
    }
}
