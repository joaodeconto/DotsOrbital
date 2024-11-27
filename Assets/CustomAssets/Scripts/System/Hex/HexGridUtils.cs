using Unity.Collections;
using Unity.Mathematics;

public static class HexGridUtils
{
    // Neighbor offsets for east-west configuration (default staggered rows)
    public static readonly int2[] EastWestEvenOffsets = new int2[]
    {

        new int2(0, 1),    // North-East  
        new int2(1, 0),    // East        
        new int2(0, -1),   // South-East        
        new int2(-1, -1),   // South-West
        new int2(-1, 0),   // West      
        new int2(-1, 1)   // North-West
    };

    public static readonly int2[] EastWestOddOffsets = new int2[]
    {

        new int2(1, 1),    // North-East  
        new int2(1, 0),    // East        
        new int2(1, -1),    // South-East        
        new int2(0,-1),   // South-West
        new int2(-1, 0),   // West      
        new int2(0, 1)    // North-West
    };

    // Neighbor offsets for north-south configuration (default staggered columns)
    public static readonly int2[] NorthSouthEvenOffsets = new int2[]
    {
        new int2(1, 0),    // North-East
        new int2(-1, 0),   // North-West
        new int2(1, -1),   // South-East
        new int2(-1, -1),  // South-West
        new int2(0, 1),    // North
        new int2(0, -1)    // South
    };

    public static readonly int2[] NorthSouthOddOffsets = new int2[]
    {
        new int2(1, 1),    // North-East
        new int2(-1, 1),   // North-West
        new int2(1, 0),    // South-East
        new int2(-1, 0),   // South-West
        new int2(0, 1),    // North
        new int2(0, -1)    // South
    };

    /// <summary>
    /// Get the neighboring tiles for a given tile based on grid orientation and stagger.
    /// </summary>
    /// <param name="tile">The tile's axial coordinates (q, r).</param>
    /// <param name="allocator">Allocator for the native list.</param>
    /// <param name="useEastWest">If true, use east-west orientation. Otherwise, use north-south.</param>
    /// <returns>A list of neighboring tiles in axial coordinates.</returns>
    public static NativeList<int2> GetNeighbors(int2 tile, Allocator allocator, bool useEastWest = true)
    {
        NativeList<int2> neighbors = new NativeList<int2>(6, allocator);

        var isEven = tile.y % 2 == 0; // Determine if the x-coordinate is even
        var offsets = useEastWest
            ? (isEven ? EastWestEvenOffsets : EastWestOddOffsets)
            : (isEven ? NorthSouthEvenOffsets : NorthSouthOddOffsets);

        foreach (var offset in offsets)
        {
            neighbors.Add(tile + offset);
        }

        return neighbors;
    }
}
