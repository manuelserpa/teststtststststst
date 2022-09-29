# Configuration Entries

## Custom Configuration Entries

The following table describes the configuration entries created.

|           Name                 |                      Path                        | Type         | Initial Value                  | Description                                        |
| ------------------------------ | ------------------------------------------------ | :----------: | :-----------:                  | -------------------------------------------------- |
| GenericRequestTimeout          | /Cmf/Custom/Automation/GenericRequestTimeout/    | Int32        | 30000                          | Generic IoT Request Timeout in milliseconds        |
| TrackInTimeout                 | /Cmf/Custom/Automation/TrackInTimeout/           | Int32        | 60000                          | TrackIn IoT Request Timeout in milliseconds        |
| GoodsIssue                     | /Cmf/Custom/ERP/MovementType/GoodsIssue          | String       | 261                            |                                                    |
| Space                          | //Protocol/Space                         | String       | Empty                          | Default Protocol when sending information to Space |
| Active                         | /amsOSRAM/FDC/Active/                            | Boolean      | True                           | Enables the Onto FDC integration                   |
| Mandatory                      | /amsOSRAM/FDC/Mandatory/                         | Boolean      | False                          | FDC mandatory                                      |
| Server                         | /amsOSRAM/FDC/Server/                            | String       | lnx-klm37.int.osram-light.com  | FDC server                                         |
| Port                           | /amsOSRAM/FDC/Port/                              | Int          | 1600                           | FDC port                                           |
| VendorContainerTypes           | /amsOSRAM/Container/VendorContainerTypes/        | String       | Empty                          | Vendor Container Types splitted by ','             |
| LotNameAllowedCharacters       | /amsOSRAM/Material/LotNameAllowedCharacters/     | String       | 0123456789ACFHLMNRTUX          | Allowed alphanumerical digits for lot names        |
