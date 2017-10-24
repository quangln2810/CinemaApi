#!/bin/bash
cd ~/CinemaApi
git pull
dotnet restore
dotnet publish
if [ ! -d /var/CinemaApi ]
then
  sudo mkdir /var/CinemaApi
fi
yes | sudo cp -ripfv ~/CinemaApi/bin/Debug/netcoreapp2.0/publish/* /var/CinemaApi
if [ -e /var/CinemaApi/Cinema.db ]
then
	echo "check database ok"
else
	rm ~/CinemaApi/Cinema.db
	dotnet ef database update
	sudo cp -a ~/CinemaApi/Cinema.db /var/CinemaApi
	sudo chown -R ubuntu /var/CinemaApi
fi
sudo service supervisor stop
sudo service supervisor start
#sudo tail -f /var/log/CinemaApi.out.log

#
#
#
#
#
#
#
#
#

