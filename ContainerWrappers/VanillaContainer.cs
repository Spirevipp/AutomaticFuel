using System.Collections.Generic;
using UnityEngine;

namespace AutomaticFuel.IContainers;

public class VanillaContainer(Container _container) : ContainerWrapper
{
    public object Unwrap()
    {
        return _container;
    }

    public static VanillaContainer Create(Container container) => new(container);

    public List<ItemDrop.ItemData> GetItems()
    {
        throw new System.NotImplementedException();
    }

    public int ContainsItem(ItemDrop.ItemData item)
    {
        throw new System.NotImplementedException();
    }

    public bool AddItem(ItemDrop.ItemData item, int amount)
    {
        throw new System.NotImplementedException();
    }

    public bool RemoveItem(ItemDrop.ItemData item, int amount)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetPosition()
    {
        throw new System.NotImplementedException();
    }
    
    public override bool Equals(object obj)
    {
        return _container.Equals(obj);
    }
    
    public override int GetHashCode()
    {
        return _container.GetHashCode();
    }
}