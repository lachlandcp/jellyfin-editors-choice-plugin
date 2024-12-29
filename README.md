## About
Editor's Choice is a plugin for Jellyfin that adds a full-width slider to the main page to feature selected content, similar to the main Netflix home page.

The featured content list is drawn from a specified user's favourited items, or a totally random selection of shows and films.

![Screenshot of Jellyfin with Editor's Choice banner slider](https://github.com/lachlandcp/jellyfin-editors-choice-plugin/blob/main/example.png?raw=true)

## Installation
**NOTE: The client script will fail to inject automatically into the jellyfin-web server if there is a difference in permission between the owner of the web files (root, or www-data, etc.) and the executor of the main jellyfin-server. This often happens because...**
* **Docker** - the container is being run as a non-root user while having been built as a root user, causing the web files to be owned by root. To solve this, you can remove any lines like `User: 1000:1000`, `GUID:`, `PID:`, etc. from the jellyfin docker compose file.
* **Install from distro repositories** - the jellyfin-server will execute as the `jellyfin` user while the web files will be owned by `root`, `www-data`, etc. This can *likely* be fixed by adding the `jellyfin` (or whichever user your main jellyfin server runs as) user to the same group the jellyfin-web folders are owned by. You should only do this if they are owned by a group other than root, and will have to lookup how to manage permissions on your specific distro.
* **Alternatively, the script can manually be added to the index.html as described below.**

**NOTE: If you manually injected the script tag, you will have to manually inject it on every jellyfin-web update, as the index.html file will get overwritten. However, for normal Jellyscrub updates the script tag will not need to be changed as the plugin will return the latest script from /EditorsChoice/script**

1. Add https://github.com/lachlandcp/jellyfin-editors-choice-plugin/raw/main/manifest.json as a Jellyfin plugin repository
2. Install **Editor's Choice** from the repository
3. Restart the Jellyfin server
4. If your Jellyfin's web path is set, the plugin should automatically inject the companion client script into the "index.html" file of the web server directory. Otherwise, the line `<script plugin="EditorsChoice" defer="defer" version="1.0.0.0" src="/EditorsChoice/script"></script>` will have to be added at the end of the body tag manually right before `</body>`. If you have a base path set, change `src="/EditorsChoice/script"` to `src="/YOUR_BASE_PATH/EditorsChoice/script"`.
5. Clear your site cookies / local storage to get rid of the cached index file and receive a new one from the server.