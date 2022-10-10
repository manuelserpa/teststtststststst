Setup
============
This section describe the setup for the Equipment Type

Setup Configuration
===================

As configuration it is necessary to set the following Resource attributes with valid values to reach out the equipment (as well as to create conditions for that, such as inbound and outbound rules in firewall, add services in antivirus whitelist, free ports in switches and others that may be necessary to unblock data traffic):

## Connection Configurations

> 1. *AutomationEquipmentAddress*, stands for the Equipment Network Address.
> 2. *AutomationEquipmentDeviceId*, stands for the Equipment Device Id.
> 3. *AutomationEquipmentIPPort*, stands for the Equipment Network Port.

## Setup Procedure Configurations

> 1. *AutomationEquipmentSkipEstablishCommunication*, if set Establish Communication Request (S1F13) will not be sent.
> 2. *AutomationEquipmentSkipSetOnline*, if set Set Online Request (S1F17) will not be sent.
> 3. *AutomationEquipmentDeleteReportMode*, defines Delete and Define Reports mode.
> 4. *AutomationEquipmentSkipDefineReportMode*, if set skips Report Definition.
> 5. *AutomationEquipmentSkipLinkEvents*, if set skips Link Event Request.
> 6. *AutomationEquipmentEnableDisableEventsMode*, defines Enabling and Disabling Events behavior at setup time.
> 7. *AutomationEquipmentEnableDisableAlarmsMode*, defines Enabling and Disabling Alarms behavior at setup time.
> 8. *AutomationEquipmentWaitOnSetupTimeOutAndRetryIfErrorOccurs*, if set on the occurrence of an error during the setup phase, set will be retried without closing the connection until setup timeout is reached.

For this equipment type the following configurations are mandatory:
NA

Non-referenced configurations must be kept default, with the exception of *AutomationEquipmentWaitOnSetupTimeOutAndRetryIfErrorOccurs* which does not affect directly the equipment configuration.
