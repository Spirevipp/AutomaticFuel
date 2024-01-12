using System.Collections.Generic;
using AutomaticFuel.API;
using UnityEngine;

namespace AutomaticFuel.IContainers;

public class kgDrawer(ItemDrawers_API.Drawer _drawer) : ContainerWrapper
{
    // convert item to name and amount
    // ItemDrop.m_dropPrefab.name, ItemDrop.m_stack


    public bool CheckItemPrefabName(string prefab)
    {
        return _drawer.Prefab == prefab;
    }

    public int GetAmount()
    {
        return _drawer.Amount;
    }

    public Vector3 GetPosition() => _drawer.Position;

    public object Unwrap()
    {
        return _drawer;
    }

    public static kgDrawer Create(ItemDrawers_API.Drawer drawer) => new(drawer);

    public List<ItemDrop.ItemData> GetItems()
    {
        List<ItemDrop.ItemData> items = [];

        if (GetAmount() < 1)
        {
            return items;
        }

        // i have no idea if this is legal
        var item = new ItemDrop.ItemData
        {
            m_dropPrefab =
            {
                name = _drawer.Prefab
            },
            m_stack = _drawer.Amount
        };
        items.Add(item);
        return items;
    }

    public int ContainsItem(ItemDrop.ItemData item)
    {
        if (CheckItemPrefabName(item.m_dropPrefab.name))
        {
            return GetAmount();
        }
        else
        {
            return -1;
        }
    }

    public bool AddItem(ItemDrop.ItemData item, int amount)
    {
        if (amount < 1)
        {
            return false;
        }

        if (ContainsItem(item) < 0)
        {
            return false;
        }

        _drawer.Add(amount);
        return true;
    }

    public bool RemoveItem(ItemDrop.ItemData item, int amount)
    {
        if (amount < 1)
        {
            return false;
        }

        if (ContainsItem(item) < 1)
        {
            return false;
        }

        _drawer.Remove(amount);
        return true;
    }
    
    public override bool Equals(object obj)
    {
        return _drawer.Equals(obj);
    }
    
    public override int GetHashCode()
    {
        return _drawer.GetHashCode();
    }
}