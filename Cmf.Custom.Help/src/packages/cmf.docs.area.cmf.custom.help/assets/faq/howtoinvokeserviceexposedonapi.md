# How to inkove service exposed on API?

## Overview

This section aims to demonstrate the steps to create an Access Token and invoke a service exposed on the API endpoint using a created alphanumeric token.

## Create Access Token by User

Below you will find the steps to Create Access Token:

1. Go to the *Administration* menu and select **Security**.
   - Select **Users**.
1. On **Users** tab select the user will be associate *Access Token*.
   - Go to *Access Token* and select **Create**.
   - Fill the fields:
     - *Name*: Token Name.
     - *Expires on*: Token validity.

![CreateAccessToken](gif/CreateAccessToken.gif)

## Invoke service exposed on API

The code bellow is used to invoke a service exposed on API:

```bash
$ curl --location --request POST 'http://<HostURL>:<HostPort>/api/amsOSRAM/<ServiceName>' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer <Token>' \
--data-raw '{
    "$type": "Cmf.Custom.amsOSRAM.Orchestration.InputObjects.<CustomInputEntity>, Cmf.Custom.amsOSRAM.Orchestration",
    <CustomInputEntityProperties>
}'
```

### Arguments

|  Name                           | Description                                                                    |
| ------------------------------- | ------------------------------------------------------------------------------ |
| `<HostURL>`                     | Server address                                                                 |
| `<HostPort>`                    | Server port configured to access API                                           |
| `<ServiceName>`                 | Name of the [service](/cmf.custom.help/techspec>artifacts>services )                |
| `<Token>`                       | Alphanumeric code generated on previous [topic](#create-access-token-by-user)  |
| `<CustomInputEntity>`           | Entity associated with the service                                              |
| `<CustomInputEntityProperties>` | All properties associated with the Entity                                      |
