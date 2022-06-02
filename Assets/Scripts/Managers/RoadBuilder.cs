using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class RoadBuilder : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _cortege;

        [Space(10)]
        [SerializeField] private Transform _roadContainer;
        [SerializeField] private Transform _startRoadPoint;

        [Space(10)]
        [SerializeField] private List<GameObject> _roadPartPrefabs;
        #endregion

        #region FIELDS PRIVATE
        private GameObject _lastPartRoad;
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            _lastPartRoad = CreateRoadPart(_startRoadPoint.position);
        }

        private void FixedUpdate()
        {
            BuildRoad();
        }
        #endregion

        #region METHODS PRIVATE
        private void BuildRoad()
        {
            var distance = Vector3.Distance(_cortege.position, _lastPartRoad.transform.position);
            while (distance < 100f)
            {
                var roadRenderer = _lastPartRoad.GetComponentInChildren<Renderer>();
                var roadPartPosition = _lastPartRoad.transform.position;
                roadPartPosition.z += roadRenderer.bounds.size.z;
                _lastPartRoad = CreateRoadPart(roadPartPosition);

                distance = Vector3.Distance(_cortege.position, _lastPartRoad.transform.position);
            }
        }

        private GameObject CreateRoadPart(Vector3 roadPartPosition)
        {
            var roadPrefab = _roadPartPrefabs[Random.Range(0, _roadPartPrefabs.Count)];
            var roadPart = Instantiate(roadPrefab, roadPartPosition, transform.rotation);
            roadPart.transform.SetParent(_roadContainer);

            return roadPart;
        }
        #endregion

    }
}
