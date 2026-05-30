using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Animated Movement Tile")]
public class AnimatedMovementTile : AnimatedTile
{
    public MovementStrategy strategy;
}
