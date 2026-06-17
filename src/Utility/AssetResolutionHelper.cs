using CarryOn.API.Common.Models;
using Vintagestory.API.Common;

namespace CarryOn.Utility
{
    public static class AssetResolutionHelper
    {
        public static bool TryResolveFallbackAsset(ICoreAPI api, ItemStack itemStack, out CarriedGroupAssetType assetType, out string? assetName)
        {
            assetType = CarriedGroupAssetType.None;
            assetName = null;

            if (api?.World == null || itemStack == null)
            {
                return false;
            }

            if (itemStack.Class == EnumItemClass.Item)
            {
                var item = api.World.GetItem(itemStack.Id);
                if (item?.Code == null)
                {
                    return false;
                }

                assetType = CarriedGroupAssetType.Item;
                assetName = item.Code.ToString();
                return true;
            }

            if (itemStack.Class == EnumItemClass.Block)
            {
                var block = api.World.GetBlock(itemStack.Id);
                if (block?.Code == null)
                {
                    return false;
                }

                assetType = CarriedGroupAssetType.Block;
                assetName = block.Code.ToString();
                return true;
            }

            return false;
        }
    }
}
