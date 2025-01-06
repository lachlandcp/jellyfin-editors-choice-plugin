using System.Net.Mime;
using System.Reflection;
using EditorsChoicePlugin.Configuration;
using Jellyfin.Data.Enums;
using Jellyfin.Extensions;
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
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public ActionResult<Dictionary<string, object>> GetFavourites() {
        _logger.LogInformation("Received request to /editorschoice/favourites");
        try {

            Dictionary<string, object> response;
            List<object> items;
            InternalItemsQuery query;
            List<BaseItem> initialResult = [];
            List<BaseItem> result = [];
            bool resultsEmpty = false;  

            // Get active user - haven't found a better way than this
            string name = "";
            if (User.Identity != null) {
                if (User.Identity.Name != null) {
                    name = User.Identity.Name;
                }
            }

            Jellyfin.Data.Entities.User? activeUser = _userManager.GetUserByName(name);
            if (activeUser == null) return NotFound();

            // If not showing random media, collect the editor user's favourited items
            if (_config.Mode == "FAVOURITES") {

                // Use random fallback if no editor ID set
                if (_config.EditorUserId == null || _config.EditorUserId == "" || _config.EditorUserId.Length < 16 ) {
                    resultsEmpty = true;
                } else {
                    Jellyfin.Data.Entities.User? editorUser = _userManager.GetUserById(Guid.Parse(_config.EditorUserId));

                    // Get the favourites list
                    query = new InternalItemsQuery(editorUser) {
                        IsFavorite = true,
                        IncludeItemsByName = true,
                        IncludeItemTypes = [BaseItemKind.Series, BaseItemKind.Movie, BaseItemKind.Episode, BaseItemKind.Season], // Editor may have favourited individual episodes or seasons - we will handle this later
                        MinCommunityRating = _config.MinimumRating,
                        MinCriticRating = _config.MinimumCriticRating
                    };
                    initialResult = _libraryManager.GetItemList(query);
                    
                    // Get ids of items in the favourites list
                    List<Guid> itemIds = new List<Guid>();
                    foreach (var item in initialResult) {
                        if (!itemIds.Contains(item.Id)){
                            // Only include if active user has parental access to this item
                            if (item.IsVisible(activeUser)){
                                itemIds.Add(item.Id);
                            }
                        }
                    }

                    // Query items from the active user to ensure access
                    query = new InternalItemsQuery(activeUser) {
                        ItemIds = [.. itemIds],
                        IncludeItemTypes = [BaseItemKind.Series, BaseItemKind.Movie, BaseItemKind.Episode, BaseItemKind.Season] // Editor may have favourited individual episodes or seasons - we will handle this later
                    };
                    result = PrepareResult(query, activeUser);

                    // If the result is empty (i.e. the active user doesn't have access to any of the items), fallback to random mode.
                    resultsEmpty = result.Count == 0;
                } 

            }

            if (_config.Mode == "COLLECTIONS") {
                List<String> remainingCollections = _config.SelectedCollections.ToList();

                while (result.Count == 0 && remainingCollections.Count > 0) { // if a collection is totally inaccessible due to user visibility or excessive filters configured, we need to try another collection
                    int collectionR = new Random().Next(remainingCollections.Count);
                    string collectionId = remainingCollections[collectionR]; 
                    remainingCollections.RemoveAt(collectionR);
                    Guid collectionGuid = Guid.Parse(collectionId);

                    BaseItem collection = _libraryManager.GetParentItem(collectionGuid, activeUser.Id);
                    if (collection is Folder) {
                        Folder f = (Folder)collection;
                        initialResult = f.GetChildren(activeUser, true);                    

                        // Get ids of items in the collection
                        List<Guid> itemIds = new List<Guid>();
                        foreach (var item in initialResult) {
                            if (!itemIds.Contains(item.Id)){
                                    itemIds.Add(item.Id);
                            }
                        }

                        query = new InternalItemsQuery(activeUser) {
                            ItemIds = [.. itemIds],
                            IncludeItemTypes = [BaseItemKind.Series, BaseItemKind.Movie],
                            MinCommunityRating = _config.MinimumRating,
                            MinCriticRating = _config.MinimumCriticRating
                        };

                        result = PrepareResult(query, activeUser);
                    }

                    // If the result is empty (i.e. the active user doesn't have access to any of the items), fallback to random mode.
                    resultsEmpty = result.Count == 0;
                }
            }

            // If showing random media is enabled OR the results list is currently empty, collect a random selection from the entire library
            if (_config.Mode == "RANDOM" || resultsEmpty) {
                                               
                // Get all shows and movies
                query = new InternalItemsQuery(activeUser) {
                    IncludeItemTypes = [BaseItemKind.Series, BaseItemKind.Movie],
                    MinCommunityRating = _config.MinimumRating,
                    MinCriticRating = _config.MinimumCriticRating
                };
                result = PrepareResult(query, activeUser);
            }

            // Build response
            response = new Dictionary<string, object>();
            items = new List<object>();

            foreach (BaseItem i in result) {
                BaseItem item = i;

                // Skip this item if it lacks a backdrop image
                if (!item.HasImage(MediaBrowser.Model.Entities.ImageType.Backdrop)) break;

                // Narrow down properties that are strictly necessary to pass through to frontend
                Dictionary<string, object> itemObject = new Dictionary<string, object>
                {
                    { "id", item.Id.ToString() },
                    { "name", item.Name },
                    { "tagline", item.Tagline },
                    { "overview", item.Overview },
                    { "official_rating", item.OfficialRating },
                    { "hasLogo", item.HasImage(MediaBrowser.Model.Entities.ImageType.Logo) }
                };

                
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

        } catch (Exception e) {
            _logger.LogError(e.ToString());
            return StatusCode(503);
        }

    }

    private List<BaseItem> PrepareResult(InternalItemsQuery query, Jellyfin.Data.Entities.User? activeUser) {
        List<BaseItem> initialResult = _libraryManager.GetItemList(query);
        List<BaseItem> result = [];

        // Randomly add items until we run out or reach the admin-set cap
        var random = new Random();
        int max = initialResult.Count;
        
        for (int i = 0; i < _config.RandomMediaCount && i < max; i++) {
            BaseItem initItem = initialResult[random.Next(initialResult.Count)];
            var shiftItem = initItem;

            // Deal with episodes or seasons
            if (shiftItem.GetBaseItemKind() == BaseItemKind.Episode || shiftItem.GetBaseItemKind() == BaseItemKind.Season) {
                shiftItem = shiftItem.GetParent();

                // If the parent is a season (i.e. the favourited item was an episode) then we need to get the season's parent show
                if (shiftItem.GetBaseItemKind() == BaseItemKind.Season) {
                    shiftItem = shiftItem.GetParent();
                }
            }

            // Check is in an allowed library
            bool inFilteredLibrary = false;
            if (_config.FilteredLibraries.Length == 0) {
                inFilteredLibrary = true; // If no libraries are selected, then we default to all libraries
            } else {
                foreach (String filteredLibraryId in _config.FilteredLibraries) {
                    if (shiftItem.GetAncestorIds().Contains(Guid.Parse(filteredLibraryId))) {
                        inFilteredLibrary = true;
                    }
                }
            }

            // Only include if active user has parental access to this item, not already in the results
            if (shiftItem.IsVisible(activeUser) && !result.Contains(shiftItem) && inFilteredLibrary){
                result.Add(shiftItem);
            } else {
                i--; // reset increment so we make up for non-inclusion
                max--;
            }
            initialResult.Remove(initItem);
        }

        return result;
    }

}