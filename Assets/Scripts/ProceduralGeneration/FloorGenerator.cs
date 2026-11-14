using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the layout algorithm: grows the map one unit at a time from open slots, then fills in
/// distances and room numbers. The only class in this system that uses the RngStream
/// </summary>
public static class FloorGenerator
{
    /// <summary>
    /// Builds a floor layout. The RNG is used in a fixed order so the same stream state always
    /// gives the same map: first the room pool shuffle, then the layout growth. The boss is on
    /// hold, so no unit is given the Boss type
    /// </summary>
    /// <param name="rng">The stream to draw every random choice from</param>
    /// <param name="layoutParams">The settings for this run, such as how many units to place</param>
    /// <param name="poolSize">How many rooms are in the run's pool. If there are more units
    /// than rooms, the shuffled pool is reused from the start</param>
    /// <returns>
    /// (FloorLayout) the finished map with distances and room numbers filled in
    /// </returns>
    public static FloorLayout Generate(RngStream rng, LayoutParams layoutParams, int poolSize)
    {
        ValidateParams(rng, layoutParams, poolSize);

        int[] shuffledPool = ShufflePool(poolSize, rng);

        FloorLayout layout = new FloorLayout();
        List<LayoutNode> placementOrder = new List<LayoutNode>();
        List<OpenSlot> openSlots = new List<OpenSlot>();

        PlaceStart(layout, placementOrder, openSlots);
        ExpandAll(layout, layoutParams.roomCount, rng, placementOrder, openSlots);
        ComputeDistances(layout);
        AssignRooms(placementOrder, shuffledPool);

        return layout;
    }

    /// <summary>
    /// Rejects settings that cannot produce a map
    /// </summary>
    /// <param name="rng">The stream, which must not be null</param>
    /// <param name="layoutParams">The settings, which must not be null and must ask for at
    /// least one unit</param>
    /// <param name="poolSize">The number of rooms in the pool, which must be at least one</param>
    private static void ValidateParams(RngStream rng, LayoutParams layoutParams, int poolSize)
    {
        if (rng == null)
        {
            throw new ArgumentNullException(nameof(rng));
        }

        if (layoutParams == null)
        {
            throw new ArgumentNullException(nameof(layoutParams));
        }

        if (layoutParams.roomCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(layoutParams), "roomCount must be at least 1");
        }

        if (poolSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(poolSize), "poolSize must be at least 1");
        }
    }

    /// <summary>
    /// Makes a list of every room number in the pool, in a random order
    /// </summary>
    /// <param name="poolSize">How many rooms are in the pool</param>
    /// <param name="rng">The stream to shuffle with</param>
    /// <returns>
    /// (int[]) the numbers 0 up to poolSize - 1, shuffled
    /// </returns>
    private static int[] ShufflePool(int poolSize, RngStream rng)
    {
        int[] pool = new int[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = i;
        }

        for (int i = 0; i < poolSize - 1; i++)
        {
            int j = rng.Range(i, poolSize);
            int temp = pool[i];
            pool[i] = pool[j];
            pool[j] = temp;
        }

        return pool;
    }

    /// <summary>
    /// Places the first unit at (0, 0) and turns its empty neighbors into open slots. The player
    /// spawns here, so there is no entry door and all four directions are open to grow into
    /// </summary>
    /// <param name="layout">The map being built</param>
    /// <param name="placementOrder">The list of units in the order they were placed</param>
    /// <param name="openSlots">The list of open slots to add to</param>
    private static void PlaceStart(FloorLayout layout, List<LayoutNode> placementOrder, List<OpenSlot> openSlots)
    {
        LayoutNode start = layout.PlaceNode(new Vector2Int(0, 0), RoomType.Start);
        placementOrder.Add(start);
        AddOpenSlots(layout, start, openSlots);
    }

    /// <summary>
    /// Keeps placing units until the layout has roomCount of them. Each round picks one open slot
    /// at random, places a unit there, drops every slot that pointed at that position, and adds
    /// the new unit's own empty neighbors
    /// </summary>
    /// <param name="layout">The map being built</param>
    /// <param name="roomCount">How many units the finished map should have, counting the start</param>
    /// <param name="rng">The stream used to pick open slots</param>
    /// <param name="placementOrder">The list of units in the order they were placed</param>
    /// <param name="openSlots">The list of open slots, which this changes as units are placed</param>
    private static void ExpandAll(FloorLayout layout, int roomCount, RngStream rng, List<LayoutNode> placementOrder, List<OpenSlot> openSlots)
    {
        while (layout.nodes.Count < roomCount)
        {
            if (openSlots.Count == 0)
            {
                throw new InvalidOperationException("Ran out of open slots before placing every unit");
            }

            int pick = rng.Range(0, openSlots.Count);
            OpenSlot slot = openSlots[pick];

            LayoutNode node = layout.PlaceNode(slot.gridPosition, RoomType.Normal);
            placementOrder.Add(node);

            RemoveSlotsAt(openSlots, slot.gridPosition);
            AddOpenSlots(layout, node, openSlots);
        }
    }

    /// <summary>
    /// Adds an open slot for every side of a unit that has nothing next to it. The slots go on the
    /// end of the list in the fixed order of Directions.All, so picks are repeatable
    /// </summary>
    /// <param name="layout">The map being built</param>
    /// <param name="node">The unit whose empty neighbors become open slots</param>
    /// <param name="openSlots">The list of open slots to add to</param>
    private static void AddOpenSlots(FloorLayout layout, LayoutNode node, List<OpenSlot> openSlots)
    {
        foreach (Direction dir in Directions.All)
        {
            Vector2Int neighborPosition = dir.GetNeighbor(node.gridPosition);
            if (!layout.IsOccupied(neighborPosition))
            {
                openSlots.Add(new OpenSlot(neighborPosition, dir));
            }
        }
    }

    /// <summary>
    /// Removes every open slot that points at a grid position. The remaining slots keep their
    /// order, which keeps picks repeatable
    /// </summary>
    /// <param name="openSlots">The list of open slots to remove from</param>
    /// <param name="gridPosition">The position that is no longer empty</param>
    private static void RemoveSlotsAt(List<OpenSlot> openSlots, Vector2Int gridPosition)
    {
        for (int i = openSlots.Count - 1; i >= 0; i--)
        {
            if (openSlots[i].gridPosition == gridPosition)
            {
                openSlots.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Fills in distanceFromStart on every unit with a breadth-first search that only walks
    /// through connected sides. The start is 0
    /// </summary>
    /// <param name="layout">The finished map, which must have a start unit</param>
    private static void ComputeDistances(FloorLayout layout)
    {
        Queue<LayoutNode> queue = new Queue<LayoutNode>();
        layout.startNode.distanceFromStart = 0;
        queue.Enqueue(layout.startNode);

        while (queue.Count > 0)
        {
            LayoutNode node = queue.Dequeue();

            foreach (Direction dir in Directions.All)
            {
                if (node.doors.IsOpen(dir))
                {
                    continue;
                }

                if (layout.TryGetNode(dir.GetNeighbor(node.gridPosition), out LayoutNode neighbor)
                    && neighbor.distanceFromStart == -1)
                {
                    neighbor.distanceFromStart = node.distanceFromStart + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// Deals room numbers from the shuffled pool to the units in the order they were placed. If
    /// there are more units than rooms, the pool is reused from the start
    /// </summary>
    /// <param name="placementOrder">The units in the order they were placed</param>
    /// <param name="shuffledPool">The room numbers in a random order</param>
    private static void AssignRooms(List<LayoutNode> placementOrder, int[] shuffledPool)
    {
        for (int i = 0; i < placementOrder.Count; i++)
        {
            placementOrder[i].roomNum = shuffledPool[i % shuffledPool.Length];
        }
    }
}
