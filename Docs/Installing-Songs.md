The Band BoomBox Official Song Collection, as the name suggests, is a collection of free playable Band BoomBox songs, available as a free download from the official website. This is made possible thanks to the generous contribution of a number of song authors, and chart authors.

While it is not strictly necessary to install the Official Song Collection to play Band BoomBox, it is highly recommended, as it greatly expands the number of songs to play with, and is a great way to get started with the game.

## Downloading the Official Song Collection
The collection is provided as a .zip archive, which contains a "Songs" folder with all of the songs included in the collection. To download it, go to the latest release on the Releases Github page, available here: [https://github.com/thomeval/BandBoomBox/releases](https://github.com/thomeval/BandBoomBox/releases). The Song Collection will be one of the entries listed under the "Assets" heading.

## Installing the Song Collections

*NOTE: The instructions below apply both to installing the Official Song Collection, as well as any other custom song collections from other sources.*

To install additional songs to Band BoomBox, first extract the archive using any compatible application, then copy the contained "Songs" folder to one of the supported locations. Band BoomBox supports loading songs from three different locations:

- The game's Save Data folder (recommended)
- The game's installed location
- A custom location, as defined in the `Settings.json` file.

See the sections below for detailed instructions on how to install to these locations. Any number of subfolders can be created within the above "Songs" folders.

Note that the Official Song Collection will typically receive updates from time to time. To update an existing song installation with a newer version, simply extract and copy the "Songs" folder over the existing one, overwriting any existing files.

*NOTE: Although having songs in more than one of the above locations will work as expected, be sure to avoid having duplicate copies of the same song installed in different folders. Doing this will result in the game only loading one copy of each duplicate song, which may result in inconsistent and unexpected behaviour.*

*NOTE: The game scans the above song folders when starting up. When adding new songs, make sure that you close the game and restart it for any changes to take effect.*

## Method 1: Installing to the Save Data folder
First, make sure that you have run Band BoomBox at least once, as the Save Data folder is created when the game starts up.

Next, navigate to Band BoomBox's Save Data folder, which on Windows is located at:
`C:\Users\[Your Username]\Documents\My Games\Band BoomBox`

On Linux, this folder is located at:
`~/My Games/Band BoomBox`

Alternatively, to access this folder easily, go to the Options screen while the game is running, and select Advanced -> Open Save Data Folder.

Copy the Songs folder in the .zip archive to this folder. If a Songs folder already exists here, overwrite any existing files.

*NOTE: On Windows 11, this folder is synced to Microsoft Onedrive's cloud service by default unless you have manually opted out. If you're currently using Onedrive, it is not recommended to install a large number of custom songs to this location, as it will count towards your Onedrive usage quota.*

## Method 2: Installing to the game install folder
The default Songs folder for the install folder is located at:
`[Installed Folder]\Band BoomBox_Data\StreamingAssets\Songs`

Where `[Installed Folder]` refers to the folder that the game itself was installed to (the one that contains Band BoomBox.exe).

Copy the Songs folder in the .zip archive to this folder. There should already be a Songs folder in this location, as this is where the songs bundled with the game are stored. Overwrite any existing files.

## Method 3: Installing to a custom folder
*NOTE: This method is not recommended for novice users, as it requires editing a configuration file. If you are not comfortable with this, please use one of the other two methods above.*

First, make sure that you have run Band BoomBox at least once, as the Save Data folder is created when the game starts up.

Next, navigate to Band BoomBox's Save Data folder, which on Windows is located at:
`C:\Users\[Your Username]\Documents\My Games\Band BoomBox`

On Linux, this folder is located at:
`~/My Games/Band BoomBox`

Alternatively, to access this folder easily, go to the Options screen while the game is running, and select Advanced -> Open Save Data Folder.

Open the `Settings.json` file contained in this folder with any text editor. Look for the `SongFolders` section, and add your desired custom folder to that section. For example:

    "SongFolders": [
        "%StreamingAssetsFolder%/Songs",
        "%AppSaveFolder%/Songs",
        "C:\\Games\\My Custom Songs"
    ],

Note that each entry in this list must be surrounded with "", and separated with commas. Also note the behaviour of backslash (\\) characters. Any backslash in the path must appear *twice* to be interpreted correctly. For example, to add `C:\Games\My Custom Songs`, it should be written as `C:\\Games\\My Custom Songs`. This does not apply to forward slashes (/).

Once you're finished editing the file, save your changes, and then copy the Songs folder in the .zip archive to the folder you specified.

There is no limit to the number of custom song folder that can be added to this list - add as many as you like. The game will scan all of them for songs when starting up.

