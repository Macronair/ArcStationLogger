`This README is still under developement.
More information about this software will be added.`

---

# ArcStationLogger
This tool can be used to keep track of what listeners hear on your radio station. It runs on a Windows Workstation or Server as a service application.

## The reason behind this project
When you run an online or AM/FM/DAB+ radio station, you probably use some kind of radio automation software.
At this moment there are more than one good quality software packages that you can use.
Some programs also keeps track of all the played songs and all the relevant data that comes with it. Like for example the played Artist/Title,
what time it played and how often you heard this song on your station.\

This info can be useful to some people. For example, it can be used to find errors in your music clocks/rotations or to create music charts which you
can broadcast at your own station.

But in some cases, the automation program that you run does not give you much information besides the played artists and songs
or even worse... only the current playing song.
Some programs does not track this at all or you must pay for this feature to get it as a plugin or addon for the software you're using.

That's why I created this Service application. It helps me monitor all the played songs and much more info about it.
Although the software I use does keep track of which songs have been played, this Windows service gives me a little more overview and it is also easy to use.

## How it works
The way this Windows Service works is when you run it, the application will keep track of one TXT file where the current song will be saved by the radio automation software (like mAirlist, RadioDJ or PlayIt). When the TXT file is changed (a new song started playing), it will save the artist and title of the song and the timestamp when te song started playing.

If it's a new song that doesn't exist yet, it will create a new record in the preferred database.

When the song is already in the database, then it will update the records in various tables. For example, the total play count will be incremented and a new timestamp will be added with the date and time when the song is last played.

## Install Guide
_No info yet_

## Arc Station View
_No info yet_
