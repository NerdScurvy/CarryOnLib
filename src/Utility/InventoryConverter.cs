using Vintagestory.API.Datastructures;

namespace CarryOn.Utility
{
    public static class InventoryConverter
    {
        /// <summary>
        /// Converts block-entity inventory data into the backpack attribute format used
        /// by attachable entities. Returns null when the inventory is null or has no
        /// slots, preserving the "uninitialized" state so vanilla can lazily initialize
        /// the container when it is first opened.
        /// </summary>
        public static ITreeAttribute? ConvertBlockInventoryToBackpack(ITreeAttribute? blockInventory)
        {
            if (blockInventory == null) return null;

            var slotCount = blockInventory.GetInt("qslots");
            if (slotCount <= 0) return null;

            var slots = blockInventory.GetTreeAttribute("slots");

            var backpack = new TreeAttribute();
            var backpackSlots = new TreeAttribute();
            for (int i = 0; i < slotCount; i++)
            {
                var slotKey = $"slot-{i}";
                var itemstack = slots?.GetItemstack(i.ToString())?.Clone();
                backpackSlots.SetItemstack(slotKey, itemstack);
            }

            backpack.SetAttribute("slots", backpackSlots);
            return backpack;
        }

        /// <summary>
        /// Converts the backpack attribute from an attachable entity itemstack back into
        /// block-entity inventory data. Returns null when the backpack is null or empty,
        /// preserving the "uninitialized" state so the container gets its default slots
        /// when placed as a block or re-attached without having been opened.
        /// </summary>
        public static ITreeAttribute? ConvertBackpackToBlockInventory(ITreeAttribute? backpack)
        {
            if (backpack == null || backpack.Count == 0) return null;

            var backpackSlots = backpack.GetTreeAttribute("slots");
            if (backpackSlots == null) return null;

            var count = backpackSlots.Count;

            var blockInventory = new TreeAttribute();
            blockInventory.SetInt("qslots", count);
            var slotsAttribute = new TreeAttribute();

            for (var i = 0; i < count; i++)
            {
                var value = backpackSlots.Values[i];
                if (value?.GetValue() == null) continue;
                slotsAttribute.SetAttribute(i.ToString(), value.Clone());
            }

            blockInventory.SetAttribute("slots", slotsAttribute);
            return blockInventory;
        }
    }
}
