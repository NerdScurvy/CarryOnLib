# CarryOnLib

CarryOnLib is the shared API and library for the CarryOn mod for Vintage Story. It provides stable interfaces and core models for modders and extensions to interact with carryable blocks, entities, and inventory systems.

## Key Interfaces

| Interface | Purpose |
|---|---|
| `ICarryManager` | Central API for carry operations: `TryPickUp`, `TryPlaceDown`, `GetCarried`, `DropCarried`, `SwapBack`, `Transfer`, plus resolver registration |
| `IInventoryConverter` | Inventory conversion between block/entity/itemstack inventories |
| `IRootTransformGroupResolver` | Implement to provide dynamic transform group candidates for the root carried block |
| `IAttachmentTransformGroupResolver` | Implement to provide dynamic transform group candidates for attached/cluster children |

## Key Models

| Model | Purpose |
|---|---|
| `CarryOnConfig` | Root server config with sections: `Carryables`, `CarryablesOnBack`, `Interactables`, `CarryWalkSpeed`, `CarryHungerRate`, `DropCarriedOnDamage`, `CarryOptions`, `CarryablesFilters`, `CarriedBlockEntity`, `BackpackTypes`, `DebuggingOptions` |
| `CarriedBlock` | Represents a carried block with stack, entity data, slot, and attached children |
| `CarrySlot` | Enum: `Hands`, `Back` |
| `CarriedGroupCandidateSet` | Per-child candidate groups and rendering hints for attachment resolvers |
| `AttachmentResolveResult` | Attachment resolver output with candidate sets and vertex warp flag |
| `BackpackSelectionMode` | Enum: `LastFound`, `FirstFound`, `FirstOnly` |
| `DropMode` | Enum: `Items`, `EntityOnFailedPlacement`, `EntityAlways` |
| `PickupAccess` | Enum: `Anyone`, `OwnerOnly`, `OwnerFirst` |

## Events

| Event | Triggered When |
|---|---|
| `CarryEvents.BlockPickedUp` | A block is picked up for carrying |
| `CarryEvents.BlockPlacedDown` | A carried block is placed down |
| `CarryEvents.BlockSwappedBack` | A carried block is swapped to/from the back slot |
| `CarryEvents.BlockTransferred` | A block is transferred to another container |
| `CarryEvents.BlockDropped` | A carried block is dropped (on damage, etc.) |

## Resolver Registration

Register custom resolver implementations in client startup:

```csharp
// Root resolver (selects primary group candidates for the carried block)
carryManager.RegisterRootTransformGroupResolver("yourmod", new MyRootResolver());

// Attachment resolver (selects transform groups for attached children)
carryManager.RegisterAttachmentTransformGroupResolver("yourmod", new MyAttachmentResolver());
```

## Usage

Reference CarryOnLib in your mod project to:

- Access and modify `CarryOnConfig` sections for server/world config
- Register new carryable blocks and behaviors
- Listen for and handle carry events
- Implement custom transform group resolvers
- Use inventory conversion and attribute utilities

## Documentation

See the [CarryOn mod documentation](https://github.com/Nerdscurvy/CarryOn/tree/main/docs) for detailed guides on:

- [Modding Guide](https://github.com/Nerdscurvy/CarryOn/blob/main/docs/modding-guide.md)
- [CarryOnConfig Reference](https://github.com/Nerdscurvy/CarryOn/blob/main/docs/carryon-config.md)
- [Transform Template and Group Guide](https://github.com/Nerdscurvy/CarryOn/blob/main/docs/transform-templates-and-groups-guide.md)
- [Transform Group Resolver Guide](https://github.com/Nerdscurvy/CarryOn/blob/main/docs/transform-group-resolvers-guide.md)

## License

CarryOnLib is distributed under the UNLICENSE. See the UNLICENSE file for details.
