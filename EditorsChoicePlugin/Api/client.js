const container = `<div class="verticalSection section-1 emby-scroller-container">
<div is="emby-scroller" class="padded-top-focusscale padded-bottom-focusscale emby-scroller" data-centerfocus="true" data-scroll-mode-x="custom" style="overflow: hidden;">
    <div is="emby-itemscontainer" class="itemsContainer scrollSlider editorsChoiceItemsContainer focuscontainer-x animatedScrollX" style="white-space: nowrap; will-change: transform; transition: transform 50ms ease-out 0s; transform: translateX(0px);">
    </div>
</div>
</div>

<style>
    .homeSectionsContainer.editorsChoiceAdded {
        padding-top: 0 !important;
    }

    .homeSectionsContainer.editorsChoiceAdded .emby-scroller {
        padding: 0 !important;
        margin-left: 3.3%;
        margin-left: max(env(safe-area-inset-left), 3.3%);
        margin-right: 3.3%;
        margin-right: max(env(safe-area-inset-right), 3.3%);
    }

    .homeSectionsContainer.editorsChoiceAdded .emby-scrollbuttons {
        mix-blend-mode: difference;
    }

    .editorsChoiceItemsContainer {
        margin-bottom: 0.75em;
    }

    .editorsChoiceItemBanner {
        width: 100%;
        height: 360px;
        border-radius: 0.2em;
        flex: none;
        background-size: cover;
        background-position-x: center;
        background-position-y: 15%;
        cursor: pointer;
    }

    .editorsChoiceItemBanner > div {
        width: 100%;
        height: 100%;
        padding: 30px;
        box-sizing: border-box;
        background: linear-gradient(90deg, rgba(0, 0, 0, 1) 0%, rgba(0, 0, 0, 0) 60%, rgba(0, 0, 0, 0) 100%);
    }

    .editorsChoiceItemLogo {
        max-width: 300px;
        max-height: calc(50% - 45px);
    }

    .editorsChoiceItemTitle {
        max-width: 100%;
        margin: 0 60px 0 0;
        font-weight: 600;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .editorsChoiceItemOverview {
        white-space: normal;
        width: 650px;
        min-width: 40%;
        max-width: 100%;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 4;
        overflow: hidden;
    }

    .editorsChoiceItemButton {
        width: auto !important;
        display: inline-block !important;
        position: absolute;
        margin: 0 !important;
        bottom: 30px;
    }

    .editorsChoiceItemRating {
        display: block !important;
        margin-top: 1em;
    }

    .starIcon {
        color: #f2b01e;
        font-size: 1.4em;
        margin-right: 0.125em;
        vertical-align: bottom;
    }

    @media screen and (max-width: 500px) {
        .editorsChoiceItemLogo {
            max-width: 100%;
            max-height: 100px;
            height: auto;
        }
    }

    @media screen and (max-width: 1600px) {
        .homeSectionsContainer.editorsChoiceAdded {
            margin-top: 30px;
        }
    }
</style>

`;

// https://dev.to/codebubb/how-to-shuffle-an-array-in-javascript-2ikj#:~:text=the%20fisher-yates%20algorith
function shuffle(old_array) {
    var array = old_array
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        const temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
    return array;
}

const GUID = "70bb2ec1-f19e-46b5-b49a-942e6b96ebae";

// Detect if container is ready to setup slider
var target = $( "#reactRoot" )[0];

// Create an observer instance
var observer = new MutationObserver(function( mutations ) {
  mutations.forEach(function( mutation ) {
    var newNodes = mutation.addedNodes; // DOM NodeList
    if( newNodes !== null ) { // If there are new nodes added
    	var $nodes = $( newNodes ); // jQuery set
    	$nodes.each(function() {
    		var $node = $( this );
    		if( $node.hasClass('section0') && !$node.hasClass('editorsChoiceAdded')) {
    			setup();
    		}
    	});
    }
  });    
});
observer.observe(target, {attributes: true, childList: true, characterData: true, subtree: true});


// Setup slider
function setup() {
    console.log("Attempting creation of editors choice slider.");
    $('.homeSectionsContainer').each((index, elem) => {
        if (!$(elem).hasClass('editorsChoiceAdded')) {
            console.log("Fetching favourites data from API...");
            ApiClient.fetch({url: ApiClient.getUrl('/EditorsChoice/favourites'), type: 'GET'}).then(function(response) {
                response.json().then(function(data) {
                    var favourites = shuffle(data.favourites);
                        
                    $(elem).prepend(container);

                    favourites.forEach((favourite, i) => {
                        let communityRating = favourite.community_rating.toFixed(1);
                        
                        let editorsChoiceItemLogo = `<img class='editorsChoiceItemLogo' src='/Items/${favourite.id}/Images/Logo/0' alt='${favourite.name}'/>`;

                        if (!favourite.hasLogo) {
                            editorsChoiceItemLogo = `<h1 class="editorsChoiceItemTitle">${favourite.name}</h1>`;
                        }

                        let editorsChoiceItemRating = `<div class='editorsChoiceItemRating starRatingContainer'><span class='material-icons starIcon star'></span>${communityRating}</div>`;
                        
                        // Skip rating if it is 0 - a perfect zero means the metadata provider has no score so is misrepresentative
                        if (favourite.communityRating == 0) {
                            editorsChoiceItemRating = "";
                        }

                        let editorsChoiceItemOverview = `<p class='editorsChoiceItemOverview'>${favourite.overview}</p>`;
                        let editorsChoiceItemButton = `<button is='emby-button' class='editorsChoiceItemButton raised button-submit block emby-button'> <span>Watch</span> </button>`;

                        // Sometimes the path will be /web/index.html#/home.html, other times it will be /web/#/home.html
                        var baseUrl = Emby.Page.baseUrl() + '/';
                        if (window.location.href.includes('/index.html')) {
                            baseUrl += 'index.html';
                        }

                        let editorsChoiceItemBanner = `<div class='editorsChoiceItemBanner' data-index='${i}' style="background-image:url('/Items/${favourite.id}/Images/Backdrop/0');" onclick="window.location.href='${baseUrl}#/details?id=${favourite.id}';"><div> ${editorsChoiceItemLogo} ${editorsChoiceItemRating} ${editorsChoiceItemOverview} ${editorsChoiceItemButton}</div></div>`;
                        $(".editorsChoiceItemsContainer").append(editorsChoiceItemBanner);
                    });

                    $(elem).addClass('editorsChoiceAdded');     
                });
            });
        }
    });
}

$(document).ready(function () {
    // Remind user that their favourites will be public when they add a new favourite.
    $('body').on('click', '[is="emby-ratingbutton"]', function () {
        if (!$(this).hasClass('ratingbutton-withrating')) {
            ApiClient.getPluginConfiguration(GUID).then(function (data) {
                if (ApiClient.getCurrentUserId() == data.EditorUserId) {
                    Dashboard.confirm("You are the featured items editor! Your favourites will be displayed on the home page for all users, if enabled.");
                }
            });
        }
    })
});