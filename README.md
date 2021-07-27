# 📝 DemeoStripper &nbsp;[![Actions Status](https://github.com/JoeZwet/DemeoStripper/workflows/.NET%20Build/badge.svg)](https://github.com/lolPants/BeatStripper/actions)
_Generate stripped Demeo DLLs for use in CI_

## 🔧 About
This program resolves your Demeo install directory, then generates virtualised and stripped DLLs for use in CI. It currently only strips core game and Unity assemblies (feel free to PR the name whitelist if a DLL you need is being ignored).

Modified from [lolPants/BeatStripper](https://github.com/lolPants/BeatStripper).

## 🚀 Usage
Download the latest artifact from [CI autobuild](https://github.com/JoeZwet/DemeoStripper/actions) and run the `.exe`. Stripped DLLs are output to a folder named `stripped` in the working directory of the `.exe`, with a game version subfolder.

## ⚠ Legal Disclaimer
Obviously these DLLs are the copyright of Beat Games and Unity respectively. Whether function signatures are copyrightable is down to a lawyer, which I am not. Only distribute stripped DLLs if you have the legal right to do so. By using this tool, you agree that I hold no responsibility for any legal trouble you may get into for distributing stripped DLLs.
