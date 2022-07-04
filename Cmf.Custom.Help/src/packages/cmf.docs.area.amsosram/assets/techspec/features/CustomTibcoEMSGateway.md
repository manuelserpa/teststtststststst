# Tibco EMS Gateway

## Requirement Specification

Windows Service used to send messages to Tibco EMS.

## Design Specification

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name  | Type | Description |
| :---- | :--: | :---------- |
| [CustomTibcoEMSGatewayResolver](/AMSOsram/techspec>artifacts>generictables>CustomTibcoEMSGatewayResolver) | Generic Table | Contains the configurations of messages from MES that need to be sent to Tibco. |
| [CustomInvalidateCache](/AMSOsram/techspec>artifacts>deeactions>CustomInvalidateCache) | DEE Action | Action to invalidate cache for Generic Table Tibco Resolver. |
| [CustomGetTibcoConfigurations](/AMSOsram/techspec>artifacts>deeactions>CustomGetTibcoConfigurations) | DEE Action | Action to get Tibco configurations. |
| /AMSOsram/TibcoEMS/IsEnabled/ | Config | Configuration to set TibcoEMS availability. |
| /AMSOsram/TibcoEMS/Host/ | Config | Configuration to set TibcoEMS Host value. |
| /AMSOsram/TibcoEMS/Username/ | Config | Configuration to set TibcoEMS Username value. |
| /AMSOsram/TibcoEMS/Password/ | Config | Configuration to set TibcoEMS Password value. |

### How it works

The Generic Table CustomTibcoEMSGatewayResolver contains all the subjects that are necessary to subscribe on Message Bus.

* When the service start execution it will:
  * Try to subscribe all subjects that exists on CustomTibcoEMSGatewayResolver and the subject CustomTibcoEMSGatewayInvalidateCache.
  * After that the service will listen to the subjects subscribed.
* When one of the subscribed subjects publish a message on the Message Bus:
  * If the configuration in Generic Table CustomTibcoEMSGatewayResolver **has** an Action associated:
    * The Action is executed, and a message is returned.
      * The message returned will be sent to Tibco along the corresponding Topic.
  * If the configuration in Generic Table CustomTibcoEMSGatewayResolver **does not have** an Action associated:
    * The message received from the Message Bus is directly sent to Tibco along the corresponding Topic.

When the subject CustomTibcoEMSGatewayInvalidateCache is received the cache with the subscribed subjects will be reloaded from MES.

The following behavior will be applied:

* When a **record is inserted** in Generic Table **the service will subscribe** the corresponding subject on the Message Bus.
* When any **record is updated** in Generic Table **the service will reload the corresponding configuration** on the Message Bus.
* When any **record is deleted** from Generic Table **the service will unsubscribe** the corresponding subject on the Message Bus.

## Technical Configurations

### Installation

When installing Windows Service, the following configurations are required:

* The *Cmf.Custom.TibcoEMS.Service.dll.config* file needs to be configured with the environment settings.
* The Windows Service must point to the folder where the *Cmf.Custom.TibcoEMS.Service* package was unpacked.

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                        | Description |
| :--------- | :--------- | :--------------------------- | :---------- |
| 153091     | User Story | Create CMF Tibco EMS gateway |             |
