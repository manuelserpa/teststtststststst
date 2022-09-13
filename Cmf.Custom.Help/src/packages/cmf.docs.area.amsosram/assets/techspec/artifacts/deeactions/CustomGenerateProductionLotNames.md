# Custom Generate Production Lot Names

## Overview

DEE Action used to generate new Lot names.

## Action Groups

NA

## Pre Conditions

* Material Name is empty.
* Material Form is Lot.
* ProductionLine attribute is filled.

## Action

When Material is being created and its name is empty (null) then the System will resolve and trigger this Name Generator to fill the name.

This action considers the attribute ProductionLine associated to the entity Product to query Generic Table **CustomProductionLineConversion** to get the Site and Facility names associated.

The generated Lot name will consist of the first letter of the Site and Facility name followed by six digits that will be generated considering a set of characters defined in system Configuration (/AMSOsram/Material/LotNameAllowedCharacters). Since it is a Lot it will always contain the constant value "00" at the end.

The first 2 letters from the lot name (Site + Facility) will stored in the Name Generator context so that it can be incremented each time a new Lot is created.
