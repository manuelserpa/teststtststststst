# Custom Generate Split Lot Names

## Overview

Dee Action used to generate splited Materials name.

## Action Groups

* N/A

## Pre Conditions

* Material
* Configuration Entry AlphanumericAllowedDigits must be configured properly

## Action

This rule is triggered when the Name Generator **[Custom Generate Split Lot Names Name Generator](/AMSOsram/techspec>artifacts>namegenerators>CustomGenerateSplitLotNamesNameGenerator)** is executed. It will take the main part of the original Material (first 8 digits), and generate the final two digits in an alphanumeric running number. The available characters for the alphanumeric numbers is found in the Configuration Entry **LotNameAllowedCharacters**.

Example:

* Material: **UA00000100**
  * Split Material **UA00000100**: UA00000101
  * Split Material **UA00000101**: UA00000102