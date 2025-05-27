#!/bin/bash

gamedirectory=""
mods=""
dll_dest='lib/'

brc_data_managed='Bomb Rush Cyberfunk_Data/Managed/'
bepinex_core='BepInEx/core/'
bepinex_plugins='BepInEx/plugins'

help(){
  echo "Usage: ./source_dlls.sh [GAMEDIR] [MOD_DIR]"
  echo "You can call this script without arguments if you have installed BRC with steam"
  echo "and are using r2modman as your mod manager, but otherwise you'll need to define a GAMEDIR"
  echo "and a MOD_DIR. Note that if you define GAMEDIR you also have to define MOD_DIR."
  return 0
}

determine_mod_dir_r2mm_steam(){
  gamedir=$2
  #Assumes you're modding the game with r2mm.
  r2mmprofiles="$1/.config/r2modmanPlus-local/BombRushCyberfunk/profiles/"
  r2mmprofileslist=[]

  echo "Profile List:" >&2
  for profile in "$(ls "$r2mmprofiles")";
  do
    r2mmprofileslist+=("${profile}")
    echo "${profile}" >&2
  done

  selectedprofile=""
  isprofilevalid=0
  read -p "Select R2MM Profile: " selectedprofile
  for profile in "${r2mmprofileslist[@]}";
  do
    if [ "$profile" == "$selectedprofile" ];
    then
      isprofilevalid=1
    fi
  done
  if [ $isprofilevalid == 1 ];
  then
    printf "$r2mmprofiles/$selectedprofile"
  else
    return 2
  fi
}
#BRC on linux is only really possible though steam as of now
#~/.steam/root/ should be a symlink pointing to your steam install

if [ $# -eq 0 ]; 
then
  steamroot="$HOME/.steam/root/"
  if [ ! -L "${steamroot}" ] && [ ! -d "${steamroot}" ];
  then
    printf "\033[031mYour ~/.steam/root symlink seems to be broken! Aborting..."
    exit 1
  fi
  gamedir="${steamroot}steamapps/common/BombRushCyberfunk"
  steamdir="$(readlink -e "$steamroot")"
  steamdir+="/../../../"
  mods="$(determine_mod_dir_r2mm_steam $steamdir $gamedir)"
  if [ $? -eq 2 ]; 
  then
    printf "\033[031mProfile does not exist! Aborting..."
    exit 2
  fi
else
  gamedir=$1
  if [ $# -eq 2 ];
  then
    mods=$2
  else
    printf "\033[031mPlease provide a MOD_DIR! Aborting..."
    exit 2
  fi
  if [ ! -d $dll_dest ];
  then
    mkdir -p $dll_dest
    if [ ! -d $dll_dest ];
    then
      printf "\033[031mFailed to create directory \"" + $selectedprofile + "\"! Aborting..."
      exit 3
    fi
    touch $dll_dest/.gitignore
    echo '*.dll' >> $dll_dest/.gitignore
  fi
fi

############################################
# DLL list: add the paths to your DLLs here
############################################
dll_list=("$mods/BepInEx/core/0Harmony.dll")
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/Assembly-CSharp-firstpass.dll")
dll_list+=("$mods/BepInEx/core/BepInEx.dll")
dll_list+=("$mods/BepInEx/plugins/LazyDuchess-CommonAPI/CommonAPI.dll" )
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.AnimationModule.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.AudioModule.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.CoreModule.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.IMGUIModule.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.InputLegacyModule.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.ParticleSystemModule.dll") 
dll_list+=("$gamedir/Bomb Rush Cyberfunk_Data/Managed/UnityEngine.PhysicsModule.dll")

for dll in "${dll_list[@]}";
do
  cp -u "$dll" "$dll_dest"
done

