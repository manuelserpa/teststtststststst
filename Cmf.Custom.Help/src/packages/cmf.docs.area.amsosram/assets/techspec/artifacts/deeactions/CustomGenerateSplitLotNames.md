# Custom Generate Split Lot Names

## Overview

Dee Action used to generate splited Materials name.

## Action Groups

* N/A

## Pre Conditions

* Material
* Configuration Entry AlphanumericAllowedDigits must be configured properly

## Action

When a material is splitted this Dee Action is executed and returns the Main Material name.

Example:

* Material: **UA00000100**
  * Split Material **UA00000100**: UA00000101
  * Split Material **UA00000101**: UA00000102