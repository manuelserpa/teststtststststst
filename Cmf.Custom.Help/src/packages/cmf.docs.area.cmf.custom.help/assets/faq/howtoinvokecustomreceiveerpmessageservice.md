# How to invoke CustomReceiveERPMessage service?

## Overview

This section aims to demonstrate all the necessary steps to invoke **CustomReceiveERPMessage** service.

## Invoke CustomReceiveERPMessage service

The code bellow is used to invoke a specific service exposed on API:

```bash
$ curl --location --request POST 'http://<HostURL>:<HostPort>/api/amsOSRAM/CustomReceiveERPMessage' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer <Token>' \
--data-raw '{
    "$type": "Cmf.Custom.amsOSRAM.Orchestration.InputObjects.CustomReceiveERPMessageInput, Cmf.Custom.amsOSRAM.Orchestration",
    "Message": "<Message>",
    "MessageType": "<MessageType>"
}'
```

### Arguments

|  Name                           | Description                          |
| ------------------------------- | ------------------------------------ |
| `<HostURL>`                     | Server address                       |
| `<HostPort>`                    | Server port configured to access API |
| `<Token>`                       | Alphanumeric code generated          |
| `<Message>`                     | Message                              |
| `<MessageType>`                 | Message Type                         |
