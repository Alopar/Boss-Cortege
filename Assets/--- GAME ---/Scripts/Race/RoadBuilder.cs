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

        [Space(10)]
        [SerializeField] private List<GameObject> _finalPartPrefabs;

        [Space(10)]
        [SerializeField] private bool _infinityBuilder = true;
        [SerializeField] private float _baseRoadDistance = 0;
        #endregion

        #region FIELDS PRIVATE
        private GameObject _lastPartRoad;
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            _lastPartRoad = CreateRoadPart(_startRoadPoint.position);

            if (!_infinityBuilder)
            {
                BuildRoad(_baseRoadDistance);
                CreateFinalPart(GetCurrentPartPosition());
            }
        }

        private void FixedUpdate()
        {
            if (_infinityBuilder)
            {
                BuildRoad(100);
            }
        }
        #endregion

        #region METHODS PRIVATE
        private Vector3 GetCurrentPartPosition()
        {
            var roadRenderer = _lastPartRoad.GetComponentInChildren<Renderer>();
            var roadPartPosition = _lastPartRoad.transform.position;
            roadPartPosition.z += roadRenderer.bounds.size.z;

            return roadPartPosition;
        }

        private void BuildRoad(float maxDistance)
        {
            var distance = Vector3.Distance(_cortege.position, _lastPartRoad.transform.position);
            while (distance < maxDistance)
            {
                _lastPartRoad = CreateRoadPart(GetCurrentPartPosition());
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

        private GameObject CreateFinalPart(Vector3 roadPartPosition)
        {
            var roadPrefab = _finalPartPrefabs[Random.Range(0, _finalPartPrefabs.Count)];
            var roadPart = Instantiate(roadPrefab, roadPartPosition, transform.rotation);
            roadPart.transform.SetParent(_roadContainer);

            return roadPart;
        }
        #endregion
    }
}
