using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Hex;

    private HexesStoreScript _hexesStore;
    private Vector3 _offset;


    void Start()
    {
        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        _offset = transform.position - Hex.transform.position;
    }

    public void UpdatePosition()
    {
        List<Vector3> hexPositionVectors = new List<Vector3>();
        hexPositionVectors.AddRange(GetHexesOnBoard(true));
        hexPositionVectors.AddRange(GetHexesOnBoard(false));
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
}
