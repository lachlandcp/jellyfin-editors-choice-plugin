using System.Net;
using System.Net.Mime;
using System.Reflection;
using EditorsChoicePlugin.Configuration;
using Jellyfin.Data.Enums;
using MediaBrowser.Common.Api;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EditorsChoicePlugin.Api;

[ApiController]
[Route("editorschoice")]
public class EditorsChoiceActivityController : ControllerBase {

    private readonly PluginConfiguration _config;
    private readonly IUserManager _userManager;
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger<EditorsChoiceActivityController> _logger;
    private readonly string _scriptPath;

    public EditorsChoiceActivityController (IUserManager userManager, ILibraryManager libraryManager, ILogger<EditorsChoiceActivityController> logger) {
        _userManager = userManager;
        _libraryManager = libraryManager;
        _logger = logger;

        _config = Plugin.Instance!.Configuration;

        _scriptPath = GetType().Namespace + ".client.js";

        _logger.LogInformation("EditorsChoiceActivityController loaded.");
    }

    [HttpGet("script")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/javascript")]
    public ActionResult GetClientScript(){
        var scriptStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_scriptPath);

        if (scriptStream != null) {
            return File(scriptStream, "application/javascript");
        }

        return NotFound();
    }

    [HttpGet("favourites")]
    [Authorize(Policy = Policies.RequiresElevation)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public ActionResult<Dictionary<string, object>> GetFavourites() {
        _logger.LogInformation("Received request to /editorschoice/favourites");
        try {

            Dictionary<string, object> response;
            List<object> items;
            InternalItemsQuery query;
            List<BaseItem> result = [];
            bool editorsFavouritesEmpty = false;

            // If not showing random media, collect the editor user's favourited items
            if (!_config.ShowRandomMedia) {
                // Use random fallback if no editor ID set
                editorsFavouritesEmpty = _config.EditorUserId == "" || _config.EditorUserId.Length < 16;

                Jellyfin.Data.Entities.User? editorUser = _userManager.GetUserById(Guid.Parse(_config.EditorUserId));

                // TODO: exclude items from libraries that active user does not have access to. This assumes users have access to ALL libraries.
                query = new InternalItemsQuery(editorUser) {
                    IsFavorite = true,
                    IncludeItemsByName = true,
                    IncludeItemTypes = [BaseItemKind.Series, BaseItemKind.Movie, BaseItemKind.Episode, BaseItemKind.Season] // Editor may have favourited individual episodes or seasons - we will handle this later
                };

                result = _libraryManager.GetItemList(query);

                editorsFavouritesEmpty = result.Count == 0;
            }

            // If showing random media is enabled OR the editor's favourites list is currently empty, collect a random selection from the entire library
            if (_config.ShowRandomMedia || editorsFavouritesEmpty) {
                // Get all shows and movies
                // TODO: exclude items from libraries that active user does not have access to. This assumes users have access to ALL libraries.
                query = new InternalItemsQuery() {
                    IncludeItemTypes = [BaseItemKind.Series, BaseItemKind.Movie]
                };

                List<BaseItem> initialResult = _libraryManager.GetItemList(query);

                var random = new Random();

                for (int i = 0; i < 5; i++) {
                    if (i < initialResult.Count) { // cover edge case if there are less than 5 items in the library
                        BaseItem shiftItem = initialResult[random.Next(initialResult.Count)];
                        result.Add(shiftItem);
                        initialResult.Remove(shiftItem);
                    }
                }
            }

            response = new Dictionary<string, object>();
            items = new List<object>();

            foreach (BaseItem i in result) {
                BaseItem item = i;

                // If it's an episode or a season, then we'll get the parent season or show
                if (item.GetBaseItemKind() == BaseItemKind.Episode || item.GetBaseItemKind() == BaseItemKind.Season) {
                    item = item.GetParent();

                    // If the parent is a season (i.e. the favourited item was an episode) then we need to get the season's parent show
                    if (item.GetBaseItemKind() == BaseItemKind.Season) {
                        item = item.GetParent();
                    }
                }

                // Narrow down properties that are strictly necessary to pass through to frontend
                Dictionary<string, object> itemObject = new Dictionary<string, object>();
                itemObject.Add("id", item.Id.ToString());
                itemObject.Add("name", item.Name);
                itemObject.Add("tagline", item.Tagline);
                itemObject.Add("overview", item.Overview);
                itemObject.Add("official_rating", item.OfficialRating);

                if (item.CriticRating.HasValue) {
                    itemObject.Add("critic_rating", item.CriticRating);
                }
                if (item.CommunityRating.HasValue) {
                    itemObject.Add("community_rating", Math.Round(Convert.ToDecimal(item.CommunityRating), 2));
                }

                items.Add(itemObject);
            }

            response.Add("favourites", items);

            return Ok(response);

        } catch (Exception) {
            return StatusCode(503);
        }

    }


}