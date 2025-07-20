const container = `<div class="verticalSection section-1 editorsChoiceContainer">
    <div class="splide cardScalable">
        <div class="editorsChoiceScrollButtonsContainer">
            <div class="emby-scrollbuttons splide__arrows">
                <button type="button" is="paper-icon-button-light" data-ripple="false" data-direction="left" title="Previous" class="emby-scrollbuttons-button paper-icon-button-light splide__arrow splide__arrow--prev">
                    <span class="material-icons chevron_left" aria-hidden="true"></span>
                </button>
                <button type="button" is="paper-icon-button-light" data-ripple="false" data-direction="right" title="Next" class="emby-scrollbuttons-button paper-icon-button-light splide__arrow splide__arrow--next">
                    <span class="material-icons chevron_right" aria-hidden="true"></span>
                </button>
            </div>
        </div>
        <div class="editorsChoicePlayPauseContainer">
            <button class="splide__toggle emby-scrollbuttons-button paper-icon-button-light" type="button">
                <span class="splide__toggle__play material-icons play_arrow" aria-hidden="true"></span>
                <span class="splide__toggle__pause material-icons pause" aria-hidden="true"></span>
            </button>
        </div>
        <div class="splide__track">
            <div is="emby-itemscontainer" class="editorsChoiceItemsContainer splide__list animatedScrollX">
            </div>
        </div>
    </div>
</div>

<style>
    .homeSectionsContainer.editorsChoiceAdded {
        padding-top: 0 !important;
    }

    .homeSectionsContainer.editorsChoiceAdded .editorsChoiceContainer {
        padding-left: 3.3%;
        padding-left: max(env(safe-area-inset-left), 3.3%);
        padding-right: 3.3%;
        padding-right: max(env(safe-area-inset-right), 3.3%);
        margin-bottom: 1.8em;
    }

    .editorsChoiceItemsContainer {
        margin-bottom: 0.75em !important;
    }

    .editorsChoiceScrollButtonsContainer, .editorsChoicePlayPauseContainer {
        position: absolute;
        z-index: 99;
        right: 0.15em;
        mix-blend-mode: difference;
    }

    .editorsChoiceScrollButtonsContainer
        width: 7em;
    }

    .editorsChoicePlayPauseContainer {
        width: 4em;
    }

    .editorsChoiceScrollButtonsContainer > .splide__arrows, .editorsChoicePlayPauseContainer > .splide__toggle {
        float: right;
    }

    @media screen and (max-width: 500px) {
        .editorsChoiceScrollButtonsContainer, .editorsChoicePlayPauseContainer {
            display: none;
        }
    }

    .splide__track {
        border-radius: 0.2em;
    }

    .splide__arrow, .splide__toggle {
        position: relative;
        display: inline-block;
        opacity: 1;
        top: auto;
        transform: none;
        width: auto;
        height: auto;
        padding: .556em;
        margin-top: .85em;
        background: transparent;
    }

    .splide__arrow--next {
        right: auto;
    }

    .splide__arrow--prev {
        left: auto;
    }

    .editorsChoicePlayPauseContainer {
        display: none;
        bottom: 0.83em;
    }

    .splide__toggle.is-active .splide__toggle__play, .emby-scrollbuttons-button>.splide__toggle__pause {
        display: none;
    }

    .editorsChoiceItemBanner {
        width: 100%;
        height: 360px;
        flex: none;
        background-size: cover;
        background-position-x: center;
        background-position-y: 15%;
        cursor: pointer;
        color: #ddd;
        color: rgba(255, 255, 255, 0.8);
        text-decoration: none;
    }

    .editorsChoiceItemBanner > div {
        width: 100%;
        height: 100%;
        padding: 30px;
        box-sizing: border-box;
        background: linear-gradient(90deg, rgba(0, 0, 0, 1) 0%, rgba(0, 0, 0, 0) 60%, rgba(0, 0, 0, 0) 100%);
    }

    .editorsChoiceItemLogo {
        display: block;
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

    .layout-tv .editorsChoiceItemOverview {
        min-width: 55%;
        -webkit-line-clamp: 2;
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

    editorsChoiceContainer .sectionTitle-cards {
        padding-bottom: 0.35em;
    }

    @media screen and (max-width: 500px) {
        .editorsChoiceItemLogo {
            max-width: 100%;
            max-height: 100px;
            height: auto;
            filter: drop-shadow(3px 3px 15px black);
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

// Function to get localized string based on user's language
function getLocalizedString(key) {
    const localization = {
        'watchButton': {
            'en': 'Watch',
            'fr': 'Regarder',
            'es': 'Ver',
            'de': 'Ansehen',
            'it': 'Guarda',
            'pt': 'Assistir',
            'zh': '观看',
            'ja': '見る',
            'ru': 'Смотреть'
        }
    };

    const userLanguage = navigator.language.slice(0, 2); // Retrieve the first two characters of the user's language
    return localization[key]?.[userLanguage] || localization[key]?.['en']; // Fallback to English if the user's language is not available
}

// Setup slider
function setup() {
    console.log("Attempting creation of editors choice slider.");
    $('.homeSectionsContainer').each((index, elem) => {
        if (!$(elem).hasClass('editorsChoiceAdded')) {
            console.log("Fetching favourites data from API...");
            ApiClient.fetch({url: ApiClient.getUrl('/EditorsChoice/favourites'), type: 'GET'}).then(function(response) {
                response.json().then(function(data) {
                    var favourites = shuffle(data.favourites);
                        
                    var containerElem = $(container);
                    var containerId = 'editorsChoice-' + Date.now();
                    containerElem.first().attr('id', containerId); // add unique id to container element to handle duplicate indexPages
                    $(elem).prepend(containerElem);

                    var focusResolved = false;
                    $('.layout-tv #' + containerId + ' .emby-scrollbuttons button').on('focus', function () {
                        if (!focusResolved) {
                            $('[is="emby-tabs"] .emby-button').first().trigger('focus');
                            focusResolved = true;
                        }
                    });

                    // Add heading if exists
                    if ('heading' in data) {
                        $(containerElem).prepend(`<h2 class="sectionTitle sectionTitle-cards">${data.heading}</h2>`);
                    }

                    favourites.forEach((favourite, i) => {
                        
                        // Process star rating
                        var communityRating = 0;
                        
                        if ('community_rating' in favourite) {
                            communityRating = favourite.community_rating.toFixed(1);
                        }

                        var editorsChoiceItemRating = `<div class='editorsChoiceItemRating starRatingContainer'><span class='material-icons starIcon star'></span>${communityRating}</div>`;

                        // skip rating if it is 0 - a perfect zero means the metadata provider has no score so is misrepresentative
                        if (communityRating == 0) {
                            editorsChoiceItemRating = "";
                        }
            
                        // Process logo
                        var logoImageSize = "";
                        if (data.reduceImageSizes) {
                            logoImageSize = "?width=300";
                        }
                        var editorsChoiceItemLogo = `<img class='editorsChoiceItemLogo' src='../Items/${favourite.id}/Images/Logo/0${logoImageSize}' alt='${favourite.name}'/>`;
                        if (!favourite.hasLogo) editorsChoiceItemLogo = `<h1 class="editorsChoiceItemTitle">${favourite.name}</h1>`;

                        // Process item description
                        if (!('overview' in favourite)) {
                            favourite.overview = "";
                        }

                        var editorsChoiceItemOverview = `<p class='editorsChoiceItemOverview'>${favourite.overview}</p>`;

                        if (favourite.overview == "") {
                            editorsChoiceItemOverview = "";
                        }

                        // Process button
                        let editorsChoiceItemButton = `<button is='emby-button' class='editorsChoiceItemButton raised button-submit block emby-button'> <span>${getLocalizedString('watchButton')}</span> </button>`;
                        
                        // Process banner 
                        var bannerImageWidth = Math.max(window.screen.width, window.screen.height);
                        var bannerImageSize = "";
                        if (data.reduceImageSizes) {
                            bannerImageSize = "?width=" + bannerImageWidth;
                        }
                        let editorsChoiceItemBanner = `<a href='' onclick="Emby.Page.showItem('${favourite.id}'); return false;" class='editorsChoiceItemBanner splide__slide' style="background-image:url('../Items/${favourite.id}/Images/Backdrop/0${bannerImageSize}');"><div> ${editorsChoiceItemLogo} ${editorsChoiceItemRating} ${editorsChoiceItemOverview} ${editorsChoiceItemButton}</div></a>`;
                        $('#' + containerId + ' .editorsChoiceItemsContainer').append(editorsChoiceItemBanner);
                        
                    });
                    
                    $(elem).addClass('editorsChoiceAdded');
                    
                    if (data.autoplay) {
                        $('.editorsChoicePlayPauseContainer').show();
                    }
                    
                    new Splide( '#' + containerId + ' .splide', {
                        type: 'loop',
                        autoplay: data.autoplay,
                        interval: data.autoplayInterval,
                        pagination: false,
                        keyboard: true,
                        height: `${data.bannerHeight}px`
                      }).mount();

                });
            });
        }
    });
}

window.onload = function() {
    var sliderScript = document.createElement('script');
    sliderScript.type = 'text/javascript';
    sliderScript.src = 'https://cdn.jsdelivr.net/npm/@splidejs/splide@4.1.4/dist/js/splide.min.js';

    var sliderStyle = document.createElement('link');
    sliderStyle.rel = 'stylesheet';
    sliderStyle.href = 'https://cdn.jsdelivr.net/npm/@splidejs/splide@4.1.4/dist/css/splide.min.css';

    document.head.appendChild(sliderScript);
    document.head.appendChild(sliderStyle);

    // Detect if container is ready to setup slider
    var target = document.getElementById('reactRoot');

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

    // Remind user that their favourites will be public when they add a new favourite.
    $('body').on('click', '[is="emby-ratingbutton"]', function () {
        if (!$(this).hasClass('ratingbutton-withrating')) {
            ApiClient.getPluginConfiguration(GUID).then(function (data) {
                if (ApiClient.getCurrentUserId() == data.EditorUserId) {
                    Dashboard.confirm("You are the featured items editor! Your favourites will be displayed on the home page for all users, if enabled.");
                }
            });
        }
    });
};