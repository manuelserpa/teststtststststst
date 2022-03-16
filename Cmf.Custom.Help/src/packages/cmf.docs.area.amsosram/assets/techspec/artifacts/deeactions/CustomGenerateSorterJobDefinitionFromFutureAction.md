# CustomGenerateSorterJobDefinitionFromFutureAction

## Overview

Dee action to Generate a Custom Sorter Job Definition if exists a Required Future Action for a given material.

## Action Groups

* MaterialManagement.MaterialManagementOrchestration.MoveMaterialsToNextStep.Post
* MaterialManagement.MaterialManagementOrchestration.ReleaseMaterial.Post
* MaterialManagement.MaterialManagementOrchestration.SpecialReleaseMaterial.Post
* MaterialManagement.MaterialManagementOrchestration.ChangeMaterialFlowAndStep.Post

## Pre Conditions

* Material has a required future action of type Split or Merge
* Future Action Execution mode is set to Manual

## Action
Converts the required future action into a Custom Sorter Job Definition and adds it to the smart table 'CustomSorterJobDefinitionContext', if successful, terminates the future action.