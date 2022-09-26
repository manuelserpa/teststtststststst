# CustomGenerateSplitLotNames

## Overview

Dee Action used to generate Materials name for split lots.

## Action Groups

* N/A

## Pre Conditions

* Material form is Lot
* Configuration Entry **/amsOSRAM/Material/LotNameAllowedCharacters** must be configured with aceptable characters.

## Action

This rule is triggered when the Name Generator **[CustomGenerateSplitLotNames](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.namegenerators>CustomGenerateSplitLotNames.md)** is executed. 

It will take the first 8 digits from the parent Material's name (Lot) and concatenate two alphanumeric characters generated in sequence using the allowed characters. 

The aceptable characters must be configured in the Configuration Entry **LotNameAllowedCharacters**.

Example:

* Material: **UA00000100**
  * Split Material **UA00000100**: UA00000101
  * Split Material **UA00000101**: UA00000102
