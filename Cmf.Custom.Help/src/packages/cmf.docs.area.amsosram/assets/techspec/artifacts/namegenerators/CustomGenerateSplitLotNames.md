# Custom Generate Split Lot Names

## Overview

This generator is used when spliting a Material.

On the splits the name generation should always keep the original lot name adding a 2-digit counter:

* T2143001 -> T2143001.01 [Original Lot Name].[2 digit counter]

## Pre Conditions

N/A.

## Tokens

| Name             | Calculation Method | Value                                                                                                  |
| :--------------- | :----------------- | :----------------------------------------------------------------------------------------------------- |
| Dee              | Dee                | [Custom Generate Split Lot Names](/AMSOsram/tecspecs>artifacts>deeactions>CustomGenerateSplitLotNames) |
| SeparatorChar    | Constant           |                                                                                                        |
| CounterFormat    | Rollover Counter   |                                                                                                        |
