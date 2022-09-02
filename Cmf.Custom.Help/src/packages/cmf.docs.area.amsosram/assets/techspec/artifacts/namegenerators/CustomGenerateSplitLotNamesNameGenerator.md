# Custom Generate Split Lot Names Name Generator

## Overview

This generator is used when spliting a Material.

On the splits the name generation should take the first 8 digits of the parent material followed by two digits that are alphanumeric running numbers:

* UA00000100 -> UA00000101 [8 digits of parent material][2 digit alphanumeric running number]

## Pre Conditions

N/A.

## Tokens

| Name             | Calculation Method | Value                                                                                                  | Format |
| :--------------- | :----------------- | :----------------------------------------------------------------------------------------------------- | :----- |
| Dee              | Dee                | [Custom Generate Split Lot Names](/AMSOsram/tecspecs>artifacts>deeactions>CustomGenerateSplitLotNames) |        |
