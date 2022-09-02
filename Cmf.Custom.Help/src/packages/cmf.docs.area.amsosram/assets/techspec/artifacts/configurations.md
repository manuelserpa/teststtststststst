# Configuration Entries

## Custom Configuration Entries

The following table describes the configuration entries created.

|           Name                 |                      Path                        | Type         | Initial Value                  | Description                                        |
| ------------------------------ | ------------------------------------------------ | :----------: | :-----------:                  | -------------------------------------------------- |
| GenericRequestTimeout          | /Cmf/Custom/Automation/GenericRequestTimeout/    | Int32        | 30000                          | Generic IoT Request Timeout in milliseconds        |
| TrackInTimeout                 | /Cmf/Custom/Automation/TrackInTimeout/           | Int32        | 60000                          | TrackIn IoT Request Timeout in milliseconds        |
| GoodsIssue                     | /Cmf/Custom/ERP/MovementType/GoodsIssue          | String       | 261                            |                                                    |
| Space                          | /AMSOsram/Protocol/Space                         | String       | Empty                          | Default Protocol when sending information to Space |
| Active                         | /AMSOsram/FDC/Active/                            | Boolean      | True                           | Enables the Onto FDC integration                   |
| Mandatory                      | /AMSOsram/FDC/Mandatory/                         | Boolean      | False                          | FDC mandatory                                      |
| Server                         | /AMSOsram/FDC/Server/                            | String       | lnx-klm37.int.osram-light.com  | FDC server                                         |
| Port                           | /AMSOsram/FDC/Port/                              | Int          | 1600                           | FDC port                                           |
| VendorContainerTypes           | /AMSOsram/Container/VendorContainerTypes/        | String       | Empty                          | Vendor Container Types splitted by ','             |
| AlphanumericAllowedDigits      | /AMSOsram/NameGenerators/ProductionLotName       | String       | 0123456789ACFHLMNRTUX          | Allowed alphanumerical digits for lot names        |