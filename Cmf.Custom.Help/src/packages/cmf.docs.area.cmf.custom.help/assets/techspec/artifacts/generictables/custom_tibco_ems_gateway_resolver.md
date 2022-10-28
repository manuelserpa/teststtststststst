# CustomTibcoEMSGatewayResolver

## Overview

Generic Table used to map MES subjects to Tibco topics.

## Table Columns

The following table describes the table columns:

| Name            | Data Type | Reference | Description                                                                                  |
| :-------------- | :-------: | :-------: | :------------------------------------------------------------------------------------------- |
| Subject         |  String   |   None    | MES Subject to Subscribe.                                                                    |
| Topic           |  String   |   None    | Target Tibco Topic.                                                                          |
| ReplyTo         |  String   |   None    | Reply to Topic/Queue.                                                                        |
| Rule            |  String   |   None    | Dee Rule Message Parser that aims to parse the message to Tibco format.                      |
| IsEnabled       |  Boolean  |   None    | Informs if the record is enabled or not.                                                     |
| QueueMessage    |  Boolean  |   None    | If true, then send the message to a queue. Otherwise, send the message to Tibco topics.      |
| TextMessage     |  Boolean  |   None    | If true, then send the message as a Text message. Otherwise, send the message as MapMessage. |
| CompressMessage |  Boolean  |   None    | If true, then compresses the content of the message.                                         |
