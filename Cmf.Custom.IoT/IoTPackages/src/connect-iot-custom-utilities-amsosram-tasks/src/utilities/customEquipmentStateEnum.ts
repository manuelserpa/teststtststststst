export enum CustomEquipmentStateEnum {
    MaterialArrived = "Material Arrived", // CarrierArrived workflow
    TransferBlocked = "Transfer Blocked", //
    CarrierClamped = "Carrier Clamped", //
    CarrierIDRead = "Carrier ID Read", // CarrierIdRead workflow
    InvalidCarrierDocked = "Invalid Carrier Docked",
    CarrierOpen = "Carrier Open", //
    CassetteMap = "Cassette Map", //
    CassetteSlotMap = "Cassette Slot Map", // SlotMapReceived
    CassetteLoaded = "Cassette Loaded", //
    TrackInFailed = "Track In Failed", //
    TrackInSuccessful = "Track In Successful", // to be triggered at track in end successful
    PPSelectCommandSucceeded = "PP-Select Command Succeeded", // to be triggered on pp-select command succeeded event
    PPSelectCommandFailed = "PP-Select Command Failed", // to be triggered by failed pp-select
    StartCommandSent = "Start Command Sent", // to be triggered on start command sent
    StartCommandFailed = "Start Command Failed", // to be triggered on start command failed
    ProcessStarted = "Process Started", // ProcessStarted workflow
    ProcessComplete = "Process Complete", // ProcessComplete workflow
    CarrierClosed = "Carrier Closed", //
    CassetteUnloaded = "Cassette Unloaded", //
    CarrierUnclamped = "Carrier Unclamped", //
    ReadyToUnload = "Ready To Unload", // ReadyToUnload workflow
    MaterialRemoved = "Material Removed", // CarrierRemoved workflow
    ReadyToLoad = "Ready To Load" //
}
