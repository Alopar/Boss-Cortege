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
        #endregion

        #region FIELDS PRIVATE
        private GameObject _startPartRoad;
        private List<GameObject> _roadParts;

        private static RoadBuilder _instance;
        #endregion

        #region PROPERTIES
        public static RoadBuilder Instance => _instance;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            _roadParts = new List<GameObject>();
            _startPartRoad = CreateRoadPart(_startRoadPoint.position);
        }
        #endregion

        #region METHODS PRIVATE
        private Vector3 GetCurrentPartPosition()
        {
            var lastPartRoad = _roadParts.Count == 0 ? _startPartRoad : _roadParts[_roadParts.Count - 1];
            var roadRenderer = lastPartRoad.GetComponentInChildren<Renderer>();
            var roadPartPosition = lastPartRoad.transform.position;
            roadPartPosition.z += roadRenderer.bounds.size.z;

            return roadPartPosition;
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

        private void ClearRoad()
        {
            foreach (var roadPart in _roadParts)
            {
                Destroy(roadPart);
            }

            _roadParts.Clear();
        }
        #endregion

        #region METHODS PUBLIC
        public void BuildRoad(float maxDistance)
        {
            ClearRoad();

            var distance = 0f;
            while (distance < maxDistance)
            {
                var lastPartRoad = CreateRoadPart(GetCurrentPartPosition());
                _roadParts.Add(lastPartRoad);

                distance = Vector3.Distance(_cortege.position, lastPartRoad.transform.position);
            }

            var finalPartRoad = CreateFinalPart(GetCurrentPartPosition());
            _roadParts.Add(finalPartRoad);
        }
        #endregion
    }
}
