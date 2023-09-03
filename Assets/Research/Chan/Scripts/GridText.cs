using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridText : MonoBehaviour
{
    public Tile tile;
    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap.SetTile(new Vector3Int(0, 0, 0), tile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
