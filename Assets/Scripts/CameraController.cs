using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public const int MinFieldOfView = 80;

    public GameObject Hex;

    private HexesStoreScript _hexesStore;
    private Vector3 _offset;


    void Start()
    {
        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        _offset = transform.position - Hex.transform.position;
    }

    public void UpdateCamera()
    {
        List<Vector3> hexPositionVectors = new List<Vector3>();
        hexPositionVectors.AddRange(GetHexesOnBoard(true));
        hexPositionVectors.AddRange(GetHexesOnBoard(false));
        UpdatePosition(hexPositionVectors);
        UpdateZoom(hexPositionVectors);
    }

    private void UpdatePosition(List<Vector3> hexPositionVectors)
    {
        Vector3 nextPosition = GetCenterOfMass(hexPositionVectors);
        transform.position = nextPosition + _offset;
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

    private Vector3 GetCenterOfMass(List<Vector3> positionsVectors)
    {
        Vector3 vectorSum = new Vector3(0, 0, 0);
        positionsVectors.ForEach(vector => vectorSum += vector);

        return vectorSum / positionsVectors.Count;
    }

    private void UpdateZoom(List<Vector3> hexPositionVectors)
    {
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
    }

    private bool AreAllHexesWithMarginVisible(List<Vector3> hexPositionVectors)
    {
        foreach (var hexPositionVector in hexPositionVectors)
        {
            Vector3 positionPlusDelta = hexPositionVector + new Vector3(1, 0, 1);
            Vector3 positionMinusDelta = hexPositionVector + new Vector3(-1, 0, -1);
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
