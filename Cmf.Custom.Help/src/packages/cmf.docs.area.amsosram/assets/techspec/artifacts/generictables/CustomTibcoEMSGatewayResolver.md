# CustomTibcoEMSGatewayResolver

## Overview

Generic Table used to map the configurations of messages from MES that need to be sent to Tibco.

## Table Columns

The following table describes the table columns:

| Name      | Data Type | Reference  | Description                                                             |
| :-------- | :-------: | :--------: | :---------------------------------------------------------------------- |
| Subject   | String    | None       | MES Subject to Subscribe.                                               |
| Topic     | String    | None       | Target Tibco Topic.                                                     |
| Rule      | String    | None       | Dee Rule Message Parser that aims to parse the message to TIBCO format. |
| IsEnabled | Boolean   | None       | Informs if the record is enabled or not.                                |
