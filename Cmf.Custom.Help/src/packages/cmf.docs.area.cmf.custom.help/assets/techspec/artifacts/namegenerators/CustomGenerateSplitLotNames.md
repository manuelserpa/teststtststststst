# CustomGenerateSplitLotNames

## Overview

This generator is executed when a Material is split, and generates a name for the splitted Materials. It calls the DEE action **[CustomGenerateSplitLotNames](/cmf.custom.help/techspec>artifacts>deeactions>CustomGenerateSplitLotNames)**.

## Details

On the splits the name generation should take the first 8 digits of the parent material followed by two digits that are alphanumeric running numbers:

* UA00000100 -> UA00000101 [8 digits of parent material][2 digit alphanumeric running number]

## Pre Conditions

N/A.

## Tokens

| Name             | Calculation Method | Value                                                                                                  | Format |
| :--------------- | :----------------- | :----------------------------------------------------------------------------------------------------- | :----- |
| Dee              | Dee                | [CustomGenerateSplitLotNames](/cmf.custom.help/techspec>artifacts>deeactions>CustomGenerateSplitLotNames) |        |
