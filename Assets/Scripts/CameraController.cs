using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class CameraController : MonoBehaviour
    {
        private float _startFieldOfView;
        private float _targetFieldOfView;
        private Vector3 _startPosition;

        private const int MinFieldOfView = 80;
        private const float ZoomSpeed = 0.05f;
        private const float MovingTime = 2f;

        private HexesStoreScript _hexesStore;

        //private bool _isCameraMoving = false;

        void Start()
        {
            GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
            _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

            _startPosition = transform.position;
            _startFieldOfView = Camera.main.fieldOfView;
            _targetFieldOfView = _startFieldOfView;
        }

        void Update()
        {
            if (!Mathf.Approximately(Camera.main.fieldOfView, _targetFieldOfView))
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _targetFieldOfView, ZoomSpeed);
            }
        }

        public void ResetCamera()
        {
            iTween.StopByName("CameraPositionUpdate");
            Camera.main.fieldOfView = _startFieldOfView;
            transform.position = _startPosition;
            _targetFieldOfView = _startFieldOfView;
        }

        public void UpdateCamera()
        {
            List<Vector3> hexPositionVectors = new List<Vector3>();
            hexPositionVectors.AddRange(GetHexesOnBoard(true));
            hexPositionVectors.AddRange(GetHexesOnBoard(false));
            UpdatePosition(hexPositionVectors);
            UpdateZoom(hexPositionVectors);
        }

        private List<Vector3> GetHexesOnBoard(bool white)
        {
            List<Vector3> hexesOnBoardPositionsVectors = new List<Vector3>();
            var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
            var hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
            hexesOnBoardIds.ForEach(hexOnBoardId =>
            {
                var hexOnBoard = hexes
                    .FindLast(hex => hex.GetComponent<HexWrapperController>().HexId == hexOnBoardId);
                hexesOnBoardPositionsVectors.Add(hexOnBoard.transform.position);
            });

            return hexesOnBoardPositionsVectors;
        }

        private void UpdatePosition(List<Vector3> hexPositionVectors)
        {
            Vector3 hexesCenterPosition = GetCenterOfMass(hexPositionVectors);
            //_isCameraMoving = true;
            iTween.MoveTo(gameObject, iTween.Hash(
                    "name", "CameraPositionUpdate",
                    "position", _startPosition + hexesCenterPosition,
                    "time", MovingTime,
                    "easytype", iTween.EaseType.linear,
                    "oncomplete", "UnsetCameraMoving"));
        }

        //private void UnsetCameraMoving()
        //{
        //    _isCameraMoving = false;
        //}

        private Vector3 GetCenterOfMass(List<Vector3> positionsVectors)
        {
            Vector3 vectorSum = new Vector3(0, 0, 0);
            positionsVectors.ForEach(vector => vectorSum += vector);

            return vectorSum / positionsVectors.Count;
        }

        private void UpdateZoom(List<Vector3> hexPositionVectors)
        {
            float startFieldOfView = Camera.main.fieldOfView;
            if (AreAllHexesWithMarginVisible(hexPositionVectors))
            {
                while (AreAllHexesWithMarginVisible(hexPositionVectors) && Camera.main.fieldOfView > MinFieldOfView)
                    Camera.main.fieldOfView = Camera.main.fieldOfView - 1;
                Camera.main.fieldOfView = Camera.main.fieldOfView + 1;
            }
            else
            {
                while (!AreAllHexesWithMarginVisible(hexPositionVectors))
                    Camera.main.fieldOfView = Camera.main.fieldOfView + 1;
            }

            _targetFieldOfView = Camera.main.fieldOfView;
            Camera.main.fieldOfView = startFieldOfView;
        }

        private bool AreAllHexesWithMarginVisible(List<Vector3> hexPositionVectors)
        {
            foreach (var hexPositionVector in hexPositionVectors)
            {
                Vector3 positionPlusDelta = hexPositionVector + new Vector3(5, 0, 5);
                Vector3 positionMinusDelta = hexPositionVector + new Vector3(-5, 0, -5);
                if (!IsPositionInCameraView(positionPlusDelta)
                    || !IsPositionInCameraView(positionMinusDelta))
                    return false;
            }
            return true;
        }

        private bool IsPositionInCameraView(Vector3 position)
        {
            Vector3 viewPosition = Camera.main.WorldToViewportPoint(position);
            if (viewPosition.x < 0 || viewPosition.x > 1 ||
                viewPosition.y < 0 || viewPosition.y > 1 ||
                viewPosition.z < 0)
                return false;
            return true;
        }
    }
}
