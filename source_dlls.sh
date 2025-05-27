#!/bin/bash

GAMEDIR=""
MODS=""
DLL_DEST='lib/'
if [ $# -eq 0 ] 
  then
    #TODO: Figure out if game is installed in the same place as native steam, not just flatpak steam
    GAMEDIR="$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/BombRushCyberfunk/"
    #TODO: Don't assume default r2mm profile
    MODS="$HOME/.var/app/com.valvesoftware.Steam/.config/r2modmanPlus-local/BombRushCyberfunk/profiles/Default/"
else
    GAMEDIR=$1
fi

mkdir -p $DLL_DEST

cp -u $MODS/BepInEx/core/0Harmony.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/Assembly-CSharp-firstpass.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/Assembly-CSharp.dll $DLL_DEST #publicize this
cp -u $MODS/BepInEx/core/BepInEx.dll $DLL_DEST
cp -u $MODS/BepInEx/plugins/LazyDuchess-CommonAPI/CommonAPI.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.AnimationModule.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.AudioModule.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.CoreModule.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.IMGUIModule.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.InputLegacyModule.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.ParticleSystemModule.dll $DLL_DEST
cp -u $GAMEDIR/'Bomb Rush Cyberfunk_Data'/Managed/UnityEngine.PhysicsModule.dll $DLL_DEST

 
