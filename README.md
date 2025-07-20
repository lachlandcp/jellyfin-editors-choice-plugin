## About
Editor's Choice is a plugin for the Jellyfin web UI that adds a full-width slider to the main page to feature selected content, similar to the main Netflix home page.

The featured content list is drawn from a specified user's favourited items, or a totally random selection of shows and films. The selection can also be filtered by minimum community or critic rating.

![Screenshot of Jellyfin with Editor's Choice banner slider](https://github.com/lachlandcp/jellyfin-editors-choice-plugin/blob/main/example.png?raw=true)

Note that the plugin only works for the web UI (and therefore also the mobile app), but does not and can not work for the Android TV app or other apps due to limitations of those platforms.

## Installation
There are three ways to install this plugin.

The first step is to install this plugin by adding the repository:

1. Add `https://github.com/lachlandcp/jellyfin-editors-choice-plugin/raw/main/manifest.json` as a Jellyfin plugin repository
2. Install **Editor's Choice** from the repository

The next step is to follow one of the following three options.

### Option 1: Install the File Transformation plugin
The easiest way is to install the plugin is to use the [File Transformation plugin](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation):

3. Add `https://www.iamparadox.dev/jellyfin/plugins/manifest.json` as a plugin source repository on your Jellyfin server.
4. Find "File Transformation" in the list and install it.
5. In the **Editor's Choice** plugin settings, enable 'File Transformation' in the Technical Settings section, and put your server URL in the field below (e.g. `https://www.mymediaserver.com`).
6. Restart server

### Option 2: Script injection
The next best way is to have **Editor's Choice** installed is to enable the plugin to inject the necessary script into the main web file. This requires correct permissions.

3. Make sure the user executing the Jellyfin server application has permissions to write to the `jellyfin-web/index.html` file.
4. In the **Editor's Choice** plugin settings, enable 'Client script injection' in the Technical Settings section.
5. Restart server

**The client script will fail to inject automatically into the jellyfin-web server if there is a difference in permission between the owner of the web files (root, or www-data, etc.) and the executor of the main jellyfin-server. This often happens because...**
* **Docker** - the container is being run as a non-root user while having been built as a root user, causing the web files to be owned by root. To solve this, you can remove any lines like `User: 1000:1000`, `GUID:`, `PID:`, etc. from the jellyfin docker compose file.
* **Install from distro repositories** - the jellyfin-server will execute as the `jellyfin` user while the web files will be owned by `root`, `www-data`, etc. This can *likely* be fixed by adding the `jellyfin` (or whichever user your main jellyfin server runs as) user to the same group the jellyfin-web folders are owned by. You should only do this if they are owned by a group other than root, and will have to lookup how to manage permissions on your specific distro.

### Option 3: Manually insert script tag
The final way is to manually amend the `jellyfin-web/index.html` file yourself.

If you manually insert the script tag, you will have to manually insert it on every Jellyfin update, as the index.html file will get overwritten.

3. In Jellyfin's program files, open `jellyfin-web/index.html`.
4. Before the `</body>` tag, insert the following: `<script plugin="EditorsChoice" defer="defer" src="/editorschoice/script"></script>`. If you have a base path set, change `src="/editorschoice/script"` to `src="/YOUR_BASE_PATH/editorschoice/script"`.
5. Clear your site cookies / local storage to get rid of the cached index file and receive a new one from the server.
