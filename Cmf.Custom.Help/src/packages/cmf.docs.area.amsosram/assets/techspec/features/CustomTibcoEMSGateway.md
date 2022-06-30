# Tibco EMS Gateway

## Requirement Specification

Windows Service used to send messages to Tibco EMS.

## Design Specification

### Relevant Artifacts

The table below describes the properties for this entity type:

| Name  | Type | Is Mandatory | Data Type | Description |
| :---- | :--: | :----------: | :-------: | :---------- |
| [CustomTibcoEMSGatewayResolver](/AMSOsram/techspec>artifacts>generictables>CustomTibcoEMSGatewayResolver) | Generic Table | Yes | - | Contains the configurations of messages from MES that need to be sent to Tibco. |
| [CustomInvalidateCache](/AMSOsram/techspec>artifacts>deeactions>CustomInvalidateCache) | DEE Action | Yes | - | Action to invalidade cache for Generic Table Tibco Resolver. |
| [CustomGetTibcoConfigurations](/AMSOsram/techspec>artifacts>deeactions>CustomGetTibcoConfigurations) | DEE Action | Yes | - | Action to get Tibco configurations. |

### How it works

The Generic Table CustomTibcoEMSGatewayResolver contains all the subjects that are necessary to subscribe on Message Bus.

* When the service start execution it will:
  * Try to subscribe all subjects that exists on CustomTibcoEMSGatewayResolver and the subject CustomTibcoEMSGatewayInvalidateCache.
  * After that the service will listen to the subjects subscribed.
* When one of the subscribed subjects publish a message on the Message Bus:
  * If the configuration in Generic Table CustomTibcoEMSGatewayResolver **has** an Action associated:
    * The Action is executed and a message is returned.
      * The message returned will be sent to Tibco along the corresponding Topic.
  * If the configuration in Generic Table CustomTibcoEMSGatewayResolver **does not have** an Action associated:
    * The message received from the Message Bus is directly sent to Tibco along the corresponding Topic.

### Assumptions

The subject CustomTibcoEMSGatewayInvalidateCache has a defined default action.

There are three types of possible behaviour:

* When a **record is inserted** in Generic Table **the service will subscribe** the corresponding subject on the Message Bus.
* When any **record is updated** in Generic Table **the service will reload the corresponding configuration** on the Message Bus.
* When any **record is deleted** from Generic Table **the service will unsubscribe** the corresponding subject on the Message Bus.

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                        | Description |
| :--------- | :--------- | :--------------------------- | :---------- |
| 153091     | User Story | Create CMF Tibco EMS gateway |             |
