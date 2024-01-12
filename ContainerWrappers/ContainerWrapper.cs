using System.Collections.Generic;
using UnityEngine;

namespace AutomaticFuel.IContainers;

/**
 * A wrapper for a container.
 * It can be a good idea to override the Equals and GetHashCode methods to use the original ones from the wrapped container.
 */
public interface ContainerWrapper
{
    /**
     * Returns a list of all items in the container
     */
    public List<ItemDrop.ItemData> GetItems();

    /**
     * Returns the amount of items in the container.
     * Returns -1 (negative number less than 0) if the given item is not in the container.
     * Some containers might support having 0 of an item (like the kg_ItemDrawer)
     */
    public int ContainsItem(ItemDrop.ItemData item);

    /**
     * Return true if the item was added, false if the item was not added
     */
    public bool AddItem(ItemDrop.ItemData item, int amount);

    /**
     * Returns true if the item was removed, false if the item was not removed
     */
    public bool RemoveItem(ItemDrop.ItemData item, int amount);

    /**
     * Returns the position of the container
     */
    public Vector3 GetPosition();

    /**
     * Returns the object that is being wrapped.
     * remember to cast it to the correct type after checking the type
     */
    public object Unwrap();
}