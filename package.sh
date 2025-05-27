#!/bin/bash

#Script to package MovementFunk in a thunderstore-valid format.
#This assumes you have already built the project once.

cp bin/Debug/netstandard2.0/MovementFunk.dll Thunderstore/plugins/MovementFunk
cd Thunderstore
zip -r ../MovementFunk.zip *
cd -
