#!/bin/bash

GAMEDIR=""
MODS=""
DLLS='lib'
if [ $# -eq 0 ] 
  then
    #TODO: Figure out if game is installed in the same place as native steam, not just flatpak steam
    GAMEDIR="$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/BombRushCyberfunk/"
    #TODO: Don't assume default r2mm profile
    MODS="$HOME/.var/app/com.valvesoftware.Steam/.config/r2modmanPlus-local/BombRushCyberfunk/profiles/Default/"
else
    GAMEDIR=$1
fi

cp -u $MODS/BepInEx/core/0Harmony.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/Assembly-CSharp-firstpass.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/Assembly-CSharp.dll lib/ #publicize this
cp -u $MODS/BepInEx/core/BepInEx.dll lib/
cp -u $MODS/BepInEx/plugins/LazyDuchess-CommonAPI/CommonAPI.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.AnimationModule.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.AudioModule.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.CoreModule.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.IMGUIModule.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.InputLegacyModule.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.ParticleSystemModule.dll lib/
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.PhysicsModule.dll lib/

 
