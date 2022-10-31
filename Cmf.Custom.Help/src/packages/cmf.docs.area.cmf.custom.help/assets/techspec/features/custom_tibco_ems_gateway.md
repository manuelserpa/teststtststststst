# Tibco EMS Gateway

## Requirement Specification

Windows Service used to send messages to Tibco EMS.

## Design Specification

On the Generic Table **CustomTibcoEMSGatewayResolver** it is possible to configure if:

- The subject to listen on MessageBus.
- The specific Queue or Tibco Topics, depending on the flag, that the message will be sent to.
- The specific Queue or Tibco Topics, depending on the flag, to listen for a reply to the message sent.
- The message will be sent to Queues (set _QueueMessage_ with true) or to Tibco Topics (set _QueueMessage_ with false).
- The message will be sent as a Text message (set _TextMessage_ with true) or as a Map message (set _MapMessage_ with false).
- The message content should be compressed (set _CompressMessage_ with true) or not (set _CompressMessage_ with false). This option is not considered in Map message.

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name                                                                                                             |     Type      | Description                                                                     |
| :--------------------------------------------------------------------------------------------------------------- | :-----------: | :------------------------------------------------------------------------------ |
| [CustomTibcoEMSGatewayResolver](/cmf.custom.help/techspec>artifacts>generictables>CustomTibcoEMSGatewayResolver) | Generic Table | Contains the configurations of messages from MES that need to be sent to Tibco. |
| [CustomInvalidateCache](/cmf.custom.help/techspec>artifacts>deeactions>CustomInvalidateCache)                    |  DEE Action   | Action to invalidate cache for Generic Table Tibco Resolver.                    |
| [CustomGetTibcoConfigurations](/cmf.custom.help/techspec>artifacts>deeactions>CustomGetTibcoConfigurations)      |  DEE Action   | Action to get Tibco configurations.                                             |
| [CustomTibcoEMSReplyHandler](/cmf.custom.help/techspec>artifacts>deeactions>CustomTibcoEMSReplyHandler)          |  DEE Action   | Action to handle Tibco replies.                                                 |
| /amsOSRAM/TibcoEMS/IsEnabled/                                                                                    |    Config     | Configuration to set TibcoEMS availability.                                     |
| /amsOSRAM/TibcoEMS/Host/                                                                                         |    Config     | Configuration to set TibcoEMS Host value.                                       |
| /amsOSRAM/TibcoEMS/Username/                                                                                     |    Config     | Configuration to set TibcoEMS Username value.                                   |
| /amsOSRAM/TibcoEMS/Password/                                                                                     |    Config     | Configuration to set TibcoEMS Password value.                                   |

### How it works

The Generic Table CustomTibcoEMSGatewayResolver contains all the subjects that are necessary to subscribe on Message Bus.

- When the service start execution it will:
  - Try to subscribe all subjects that exists on CustomTibcoEMSGatewayResolver and the subject CustomTibcoEMSGatewayInvalidateCache.
  - After that the service will listen to the subjects subscribed.
- When one of the subscribed subjects publish a message on the Message Bus:
  - If the configuration in Generic Table CustomTibcoEMSGatewayResolver **has** an Action associated:
    - The Action is executed, and a message is returned.
      - The message returned will be sent to Tibco along the corresponding Topic or Queue.
  - If the configuration in Generic Table CustomTibcoEMSGatewayResolver **does not have** an Action associated:
    - The message received from the Message Bus is directly sent to Tibco along the corresponding Topic or Queue.
  - If the configuration in Generic Table CustomTibcoEMSGatewayResolver **has** a ReplyTo associated:
    - The Tibco gateway will create a listener to that specific Topic/Queue and it will wait for a reply.
    - As soon as the reply arrives it will call the DEE Action (CustomTibcoEMSReplyHandler).

When the subject CustomTibcoEMSGatewayInvalidateCache is received the cache with the subscribed subjects will be reloaded from MES.

The following behavior will be applied:

- When a **record is inserted** in Generic Table **the service will subscribe** the corresponding subject on the Message Bus.
- When any **record is updated** in Generic Table **the service will reload the corresponding configuration** on the Message Bus.
- When any **record is deleted** from Generic Table **the service will unsubscribe** the corresponding subject on the Message Bus.

The Reply must have the following structure to handle the reply accordingly:

- ReplyMessage (mandatory, string)
- Context
  - Subject (mandatory, string)

This context is set when sending the message to Tibco EMS. This middleware will save the context and attach it to the reply.

## Technical Configurations

### Installation

When installing Windows Service, the following configurations are required:

- The _Cmf.Custom.TibcoEMS.Service.dll.config_ file needs to be configured with the environment settings.
- The Windows Service must point to the folder where the _Cmf.Custom.TibcoEMS.Service_ package was unpacked.

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                                                                         | Description |
| :--------- | :--------- | :---------------------------------------------------------------------------- | :---------- |
| 153091     | User Story | Create CMF Tibco EMS gateway                                                  |             |
| 186907     | User Story | 1st Additions to EPIC 406: SPACE - Lot/Wafer SPC Data Interface via TIBCO EMS |             |
