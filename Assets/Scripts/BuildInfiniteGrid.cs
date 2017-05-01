using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInfiniteGrid : MonoBehaviour {

    public LevelGenerator levelgen;

	void Start () {
        // get sub-grids (childTransforms[0] is this.transform)
        Transform[] childTransforms = GetComponentsInChildren<Transform>();

        // move 4 child grids around the level
        var numSquaresOnTexture = 45;
        var hBZ = levelgen.blockSize / 2;

        var toMoveX = (levelgen.rows + numSquaresOnTexture) / 2 * levelgen.blockSize + hBZ;
        var toMoveZ = (levelgen.lines + numSquaresOnTexture) / 2 * levelgen.blockSize + hBZ;

        childTransforms[1].localPosition = new Vector3(toMoveX, hBZ, hBZ);
        childTransforms[2].localPosition = new Vector3(-toMoveX, hBZ, hBZ);
        childTransforms[3].localPosition = new Vector3(hBZ, hBZ, toMoveZ);
        childTransforms[4].localPosition = new Vector3(hBZ, hBZ, -toMoveZ);
    }

}
