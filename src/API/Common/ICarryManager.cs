using System.Collections.Generic;
using CarryOn.API.Event;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace CarryOn.API.Common
{

    public interface ICarryManager
    {
        ICoreAPI Api { get; }

        CarryEvents CarryEvents { get; }

        /// <summary>
        /// Checks if the entity has permission to carry the block at the specified position.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool HasPermissionToCarry(Entity entity, BlockPos pos);

        /// <summary>
        /// Gets all carried blocks for the specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IEnumerable<CarriedBlock> GetAllCarried(Entity entity);

        /// <summary>
        /// Gets the CarriedBlock carried by the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="slot">CarrySlot</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        CarriedBlock GetCarried(Entity entity, CarrySlot slot);

        /// <summary>
        /// Sets the CarriedBlock for the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="carriedBlock"></param>
        void SetCarried(Entity entity, CarriedBlock carriedBlock);

        /// <summary>
        /// Sets the CarriedBlock for the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="slot"></param>
        /// <param name="stack"></param>
        /// <param name="blockEntityData"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void SetCarried(Entity entity, CarrySlot slot, ItemStack itemStack, ITreeAttribute blockEntityData);

        /// <summary>
        /// Removes the CarriedBlock from the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="slot"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void RemoveCarried(Entity entity, CarrySlot slot);

        /// <summary>
        ///   Attempts to swap the <see cref="CarriedBlock"/>s currently carried in the
        ///   entity's <paramref name="first"/> and <paramref name="second"/> slots.
        /// </summary>
        /// <param name="entity">The entity whose carried blocks are being swapped.</param>
        /// <param name="first">The first carry slot.</param>
        /// <param name="second">The second carry slot.</param>
        /// <exception cref="ArgumentNullException"> Thrown if entity is null. </exception>
        bool SwapCarried(Entity entity, CarrySlot first, CarrySlot second);

        /// <summary>
        /// Tries to place the carriedBlock in the world, removing from entity if successful
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="carriedBlock"></param>
        /// <param name="selection"></param>
        /// <param name="dropped">If <c>true</c>, the block is set in world instead of placed, bypassing some checks.</param>
        /// <param name="playSound"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true);

        /// <summary>
        /// Tries to place the carriedBlock in the world, removing from entity if successful
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="carriedBlock"></param>
        /// <param name="selection"></param>
        /// <param name="dropped">If <c>true</c>, the block is set in world instead of placed, bypassing some checks.</param>
        /// <param name="playSound"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, ref string failureCode, bool dropped = false, bool playSound = true);

        /// <summary>
        /// Tries to carry a block from the specified position.
        /// Will check carry permissions and if the block is carryable.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        /// <param name="slot"></param>
        /// <param name="checkIsCarryable"></param>
        /// <param name="playSound"></param>
        /// <returns></returns>
        bool TryPickUp(Entity entity, BlockPos pos, CarrySlot slot, bool checkIsCarryable = true, bool playSound = true);

        /// <summary>
        /// Tries to place the carried block at the specified position.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="carried"></param>
        /// <param name="selection"></param>
        /// <param name="placedAt">Position of where the block was placed. It may have replaced the selected block.</param>
        /// <param name="failureCode"></param>
        /// <returns></returns>
        bool TryPlaceDownAt(IPlayer player, CarriedBlock carried,
                                     BlockSelection selection, out BlockPos placedAt, ref string failureCode);

        /// <summary>
        /// Sends a message to the player to lock the hotbar slots.
        /// </summary>
        /// <param name="player"></param>
        public void LockHotbarSlots(IServerPlayer player);

    }
}