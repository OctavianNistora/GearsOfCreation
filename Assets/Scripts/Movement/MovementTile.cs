using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Movement Tile")]
public class MovementTile : Tile
{
    public MovementStrategy strategy;
}
