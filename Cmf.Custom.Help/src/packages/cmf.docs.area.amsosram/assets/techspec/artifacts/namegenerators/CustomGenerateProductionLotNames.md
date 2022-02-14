# Custom Generate Production Lot Names

## Overview

Name generator for Production Lot Names. This generator is used when creating the main material.

The name generation should be as follows:
- [Site][2 digits for the fiscal year][2 digits for the fiscal week][Alphanumeric running number]
- R2143001….. R214300A
For the Site it is used the constant T.

*Note:*
Alphanumeric need to exclude the following letters B,D,E,G,I,J,K,O,P,Q,S,V,W,Y,Z

## Pre Conditions

* NA

## Tokens

| Name              | Calculation Method | Value                                                                                                        |
| :---------------- | :----------------- | :----------------------------------------------------------------------------------------------------------- |
|       Dee         |       Dee          | [CustomGenerateProductionLotNames](/AMSOsram/tecspecs>artifacts>deeactions>CustomGenerateProductionLotNames) |