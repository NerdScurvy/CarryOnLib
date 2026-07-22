# CarryOnLib

CarryOnLib is the shared API and library for the [CarryOn mod](https://github.com/Nerdscurvy/CarryOn) for Vintage Story. It provides stable interfaces and core models for modders and extensions to interact with carryable blocks, entities, and inventory systems.

CarryOnLib is designed to be used as a dependency by other mods that want to integrate optional or mandatory CarryOn functionality. If CarryOn is not loaded, `CarryOnLibSystem.CarryManager` will be null — integrating mods should check for this.

## Key Interfaces

| Interface | Purpose |
|---|---|
| `ICarryManager` | Central API for carry operations: `TryPickUp`, `TryPlaceDown`, `GetCarried`, `SetCarried`, `RemoveCarried`, `SwapCarried`, `DropCarried`, plus resolver registration |
| `ICarryableTransfer` | Contract for behaviors that support transferring carryable items between carried state and block entity inventories (e.g., shelving, displays) |
| `ICarryableHintPolicy` | Optional policy hook to dynamically filter which interaction hints are shown to the player |
| `ICarryEventHandler` | Bootstrap interface for event subscribers to register handlers during initialization |
| `IConditionalBlockBehavior` | Marker for block behaviors that should only be active under certain conditions |
| `IRootTransformGroupResolver` | Implement to provide dynamic transform group candidates for the root carried block during rendering |
| `IAttachmentTransformGroupResolver` | Implement to provide dynamic transform group candidates for attached/cluster children during rendering |

## Key Models

| Model | Purpose |
|---|---|
| `CarriedBlock` | Represents a carried block with stack, entity data, slot, attached children, and original block code/mesh angle |
| `AttachedCarriedBlock` | A child block attached to a primary carried block, with relative offset and original face |
| `CarrySlot` | Enum: `Hands`, `Back`, `Attached` |
| `CarryHintType` | Flags enum: `None`, `BasePickup`, `ForcePickup`, `TransferPut`, `TransferTake` |
| `CarryHintContext` | Context object passed to `ICarryableHintPolicy` describing the current interaction state |
| `CarriedGroupCandidateSet` | Immutable candidate set: groups (via constructor), rendering hints, and asset fallbacks for transform group resolvers |
| `AttachmentResolveResult` | Immutable attachment resolver output: candidate sets (via constructor) and vertex warp flag |
| `CarryConstants` | Static constants: `ModId`, `FailureCodes`, `HotKeyCodes`, `AttributeKeys` |
| `CarriedGroupAssetType` | Enum: `None`, `Block`, `Item` — asset type for transform group resolution |
| `TreeValueAttribute` | Attribute for mapping C# properties to attribute tree keys during serialization |

## Events

Access events via `ICarryManager.CarryEvents`:

| Event | Delegate | Triggered When |
|---|---|---|
| `BeforePickUpBlock` | `BeforePickUpBlockDelegate` | Before a block is picked up — subscribers can veto the pickup |
| `BeforeRemoveBlockFromWorld` | `BeforeRemoveBlockDelegate` | Before a carried block is removed from the world |
| `BeforeRestoreBlockEntityData` | `BlockEntityDataDelegate` | Before block entity data is restored during placement |
| `CheckPermissionAt` | `CheckPermissionAtDelegate` | Custom permission check for carrying at a position |
| `BlockDropped` | `EventHandler<BlockDroppedEventArgs>` | After a carried block is dropped |
| `BlockRemoved` | `EventHandler<BlockRemovedEventArgs>` | After a block is removed from the world |

## Resolver Registration

Register custom transform group resolvers to control which render groups are used for carried blocks:

```csharp
// Root resolver (selects primary group candidates for the carried block)
carryManager.RegisterRootTransformGroupResolver("yourmod", new MyRootResolver());

// Attachment resolver (selects transform groups for attached children)
carryManager.RegisterAttachmentTransformGroupResolver("yourmod", new MyAttachmentResolver());
```

## Utilities

CarryOnLib includes utility classes for common operations:

| Utility | Purpose |
|---|---|
| `BlockUtils` | Block operations: create carried blocks from world positions, multiblock origin resolution, container slot access |
| `CarriedBlockTreeSerializer` | Serialize/deserialize `CarriedBlock` to/from attribute trees |
| `TreeSerializer` | Generic serialization of config objects to attribute trees using `[TreeValue]` attributes |
| `ModelTransformParser` | Parse `ModelTransform` from JSON with individual component overrides |
| `AttributeTreeExtensions` | Extension methods for navigating and manipulating nested attribute trees |
| `AssetResolutionHelper` | Resolve fallback asset types and names from item stacks |
| `BehaviorRegistrationExtensions` | Generic registration helpers for block and entity behaviors |
| `InventoryConverter` | Convert between block container and backpack inventory formats |
| `JsonHelper` | Typed JSON extraction helpers for `JsonObject` and `JToken` |
| `CarryManagerExtensions` | Convenience overloads for `ICarryManager` methods that suppress failure codes |
| `CarryInputExtensions` | Client-side helpers for checking carry key states |
| `JTokenExtensions` | Extension methods on `Dictionary<string, JToken>` for typed JSON extraction |

## Usage

Reference CarryOnLib in your mod project to:

- Access carry operations via `ICarryManager`
- Listen for and handle carry events
- Implement custom transform group resolvers
- Implement `ICarryableTransfer` for block entity inventory integration
- Use inventory conversion and attribute tree utilities
- Register carryable block behaviors

Note: `CarryOnConfig`, `CarryCodes`, and related configuration types live in the [CarryOn mod](https://github.com/Nerdscurvy/CarryOn) itself, not in CarryOnLib. CarryOnLib provides `CarryConstants` as a small subset of codes for modder use.

## License

CarryOnLib is distributed under the UNLICENSE. See the UNLICENSE file for details.
