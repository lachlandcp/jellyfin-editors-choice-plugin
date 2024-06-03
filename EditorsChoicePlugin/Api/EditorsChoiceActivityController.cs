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

            if (_config.EditorUserId == "" || _config.EditorUserId.Length < 16) return StatusCode(503);

            Jellyfin.Data.Entities.User? user = _userManager.GetUserById(Guid.Parse(_config.EditorUserId));

            InternalItemsQuery query = new InternalItemsQuery(user) {
                IsFavorite = true,
                IncludeItemsByName = true,
                IncludeItemTypes = new[] {BaseItemKind.Series, BaseItemKind.Movie, BaseItemKind.Episode, BaseItemKind.Season}
            };
            List<BaseItem> result = _libraryManager.GetItemList(query);

            response = new Dictionary<string, object>();
            items = new List<object>();

            foreach (BaseItem i in result) {
                BaseItem item = i;

                if (item.GetBaseItemKind() == BaseItemKind.Episode || item.GetBaseItemKind() == BaseItemKind.Season) {
                    item = item.GetParent();

                    if (item.GetBaseItemKind() == BaseItemKind.Season) {
                        item = item.GetParent();
                    }
                }

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