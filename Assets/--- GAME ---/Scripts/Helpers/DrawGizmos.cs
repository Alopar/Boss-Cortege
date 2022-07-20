using UnityEngine;

namespace BossCortege
{
    public class DrawGizmos : MonoBehaviour
    {
        [SerializeField] private bool _on;
        [SerializeField] private Color _color;
        [SerializeField] private float _radius;

        private void OnDrawGizmos()
        {
            if (!_on) return;

            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}
