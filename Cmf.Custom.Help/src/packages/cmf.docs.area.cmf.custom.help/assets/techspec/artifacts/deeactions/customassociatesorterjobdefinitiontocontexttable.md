# CustomAssociateSorterJobDefinitionToContextTable

## Overview

DEE action to create CustomSorterJobDefinition and associate to the context on CustomSorterJobDefinitionContext smart table.

## Action Groups

## Pre Conditions

* This DEE action must be enabled before using. It is disabled by default.

## Action

This DEE will create the **CustomSorterJobDefinition** with all the wafer's relevant **movements** and add an entry in the **CustomSorterJobDefinitionContext** smart table for a specific **lot**.

This DEE action shall be called when **CustomSorterJobDefinition** needs to be created and added to the context to be processed on a sorter.

Example: It can be used in the automatic grading, wafer scrap, split and merging processes.
