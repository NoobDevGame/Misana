using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class InventoryComponent : Component<InventoryComponent>
    {
        public InventoryComponent()
        {
            const int n = 50;
            Slots = new List<InventorySlot>(n);

            for (int i = 0; i < n; i++)
            {
                Slots.Add(new InventorySlot());
            }
        }

        public List<InventorySlot> Slots;
        public override void CopyTo(InventoryComponent other)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ItemDescriptionComponent : Component<ItemDescriptionComponent>
    {
        public string Name;

        public override void CopyTo(ItemDescriptionComponent other)
        {
            throw new System.NotImplementedException();
        }
    }

    public class InventorySlot
    {
        public bool Empty;
        public Item Item;
    }

    public class Item
    {
        public ItemDescriptionComponent Description;
    }

    public class GenericItem : Item
    {
        public int Count;
        public EntityBuilder Builder;
        public float Scale;
    }

    public class SpecialItem : Item
    {
        
    }
}