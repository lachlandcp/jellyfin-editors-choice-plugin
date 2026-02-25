const container = `
<div class="verticalSection section-1 editorsChoiceContainer">
  <div class="splide cardScalable">
    <div class="editorsChoiceScrollButtonsContainer">
      <div class="emby-scrollbuttons splide__arrows">
        <button type="button" is="paper-icon-button-light" data-ripple="false" data-direction="left" title="Previous"
          class="emby-scrollbuttons-button paper-icon-button-light splide__arrow splide__arrow--prev">
          <span class="material-icons chevron_left" aria-hidden="true"></span>
        </button>

        <button class="splide__toggle emby-scrollbuttons-button paper-icon-button-light" type="button" id="editorsChoicePlayPause">
          <span class="splide__toggle__play material-icons play_arrow" aria-hidden="true"></span>
          <span class="splide__toggle__pause material-icons pause" aria-hidden="true"></span>
        </button>

        <button type="button" is="paper-icon-button-light" data-ripple="false" data-direction="right" title="Next"
          class="emby-scrollbuttons-button paper-icon-button-light splide__arrow splide__arrow--next">
          <span class="material-icons chevron_right" aria-hidden="true"></span>
        </button>
      </div>
    </div>

    <div class="splide__track">
      <div is="emby-itemscontainer" class="editorsChoiceItemsContainer splide__list animatedScrollX"></div>
    </div>
  </div>
</div>

<style>
  /* ===== Layout / spacing ===== */
  .homeSectionsContainer.editorsChoiceAdded { padding-top: 0 !important; }

  .homeSectionsContainer.editorsChoiceAdded .editorsChoiceContainer {
    padding-left: max(env(safe-area-inset-left), 3.3%);
    padding-right: max(env(safe-area-inset-right), 3.3%);
    margin-bottom: 1.8em;
  }

  .editorsChoiceContainer .sectionTitle-cards { padding-bottom: 0.35em; }
  .editorsChoiceItemsContainer { column-gap: normal !important; }

  @media screen and (max-width: 1600px) {
    .homeSectionsContainer.editorsChoiceAdded { margin-top: 30px; }
  }

  /* ===== Controls ===== */
  .editorsChoiceScrollButtonsContainer,
  .editorsChoicePlayPauseContainer {
    position: absolute;
    z-index: 99;
    right: 0.15em;
    mix-blend-mode: difference;
  }

  .editorsChoiceScrollButtonsContainer { width: 7em; }
  .editorsChoicePlayPauseContainer { width: 4em; display: none; bottom: 0.83em; }

  .editorsChoiceScrollButtonsContainer > .splide__arrows,
  .editorsChoicePlayPauseContainer > .splide__toggle {
    float: right;
  }

  @media screen and (max-width: 500px) {
    .editorsChoiceScrollButtonsContainer,
    .editorsChoicePlayPauseContainer { display: none; }
  }

  .splide__track { border-radius: 0.2em; }

  .splide__arrow,
  .splide__toggle {
    position: relative;
    display: inline-block;
    opacity: 1;
    top: auto;
    transform: none;
    width: auto;
    height: auto;
    padding: .556em;
    background: transparent;
  }

  .splide__arrow--next { right: auto; }
  .splide__arrow--prev { left: auto; }

  .splide__toggle.is-active .splide__toggle__play,
  .emby-scrollbuttons-button > .splide__toggle__pause {
    display: none;
  }

  /* ===== Banner ===== */
  .editorsChoiceItemBanner {
    width: 100%;
    height: 360px;
    flex: none;
    background-size: cover;
    background-position-x: center;
    cursor: pointer;
    color: rgba(255, 255, 255, 0.8);
    text-decoration: none;
    background-position-y: 52%;
  }

  .editorsChoiceItemBanner:nth-child(odd) { background-position-y: 48%; }

  @keyframes banner {
    0% { background-position-y: 52%; }
    100% { background-position-y: 48%; }
  }

  .editorsChoiceItemBanner.is-visible {
    animation: banner 10s infinite alternate both;
  }

  .editorsChoiceItemBanner:nth-child(odd).is-visible {
    animation-direction: alternate-reverse;
  }

  .editorsChoiceItemBanner > div {
    width: 100%;
    height: 100%;
    padding: 30px;
    box-sizing: border-box;
    background: linear-gradient(90deg, rgba(0,0,0,1) 0%, rgba(0,0,0,0) 60%, rgba(0,0,0,0) 100%);
  }

  /* ===== Content ===== */
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
    width: fit-content !important;
    display: inline-block !important;
    position: absolute;
    margin: 0 !important;
    bottom: 30px;
  }

  .editorsChoiceItemRating { display: block !important; margin-top: 1em; }

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
      filter: drop-shadow(3px 3px 15px black);
    }
  }

  /* ===== Hero mode ===== */
  .editorsChoiceHeroMode .editorsChoiceContainer { padding: 0 !important; }
  .editorsChoiceHeroMode #homeTab { transform: translateY(-120px); }

  .editorsChoiceHeroMode .splide.cardScalable {
    border-radius: unset !important;
    border: 0 !important;
    background: transparent;
    box-shadow: none !important;
    margin-bottom: -170px;
  }

  .editorsChoiceHeroMode .editorsChoiceHeight-360 .splide.cardScalable {
    margin-bottom: -75px;
  }

  .editorsChoiceHeroMode .editorsChoiceHeight-400 .splide.cardScalable {
    margin-bottom: -105px;
  }

  .editorsChoiceHeroMode .editorsChoiceHeight-500 .splide.cardScalable {
    margin-bottom: -150px;
  }


  .editorsChoiceHeroMode .editorsChoiceItemBanner { background-position-y: 15% !important; }
  .editorsChoiceHeroMode .editorsChoiceScrollButtonsContainer { margin-right: max(env(safe-area-inset-left), 3.3%); }
  .editorsChoiceHeroMode .editorsChoiceScrollButtonsContainer .emby-scrollbuttons { padding-top: 120px; }

  .editorsChoiceHeroMode  .editorsChoiceBackdropCenter {
      background-position: center;
  }

  .editorsChoiceHeroMode .editorsChoiceBackdropTop {
      background-position: top;
  }

  .editorsChoiceHeroMode .editorsChoiceBackdropBottom {
      background-position: bottom;
  }

  .editorsChoiceHeroMode .editorsChoiceItemBanner .editorsChoiceBackdrop {
    position: absolute;
    inset: 0;
    z-index: 0;
    background-size: cover;

    background-repeat: no-repeat;
    mask-image: linear-gradient(
      to bottom,
      rgba(0,0,0,1) 40%,
      rgba(0,0,0,0.9) 55%,
      rgba(0,0,0,0.4) 70%,
      rgba(0,0,0,0) 100%
    );
  }

  .editorsChoiceHeroMode .editorsChoiceItemButton {
    width: fit-content !important;
    display: inline-flex !important;
    position: relative;
    margin: 0 !important;
    bottom: unset !important;
  }

  .editorsChoiceHeroMode .editorsChoiceItemBanner .editorsChoiceBackdrop::after {
    content: "";
    position: absolute;
    inset: 0;
    z-index: 1;
    background: linear-gradient(
      135deg,
      rgba(0,0,0,0.95) 0%,
      rgba(0,0,0,0.85) 15%,
      rgba(0,0,0,0.55) 30%,
      rgba(0,0,0,0.25) 50%,
      rgba(0,0,0,0.08) 65%,
      rgba(0,0,0,0) 80%
    );
  }

  .editorsChoiceHeroMode .editorsChoiceItemBanner .editorsChoiceContent {
    position: relative;
    z-index: 2;
    height: 100%;
    padding: 90px max(env(safe-area-inset-right), 3.3%) 0;
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
    justify-content: center;
    background: none !important;
  }
</style>
`;

const GUID = "70bb2ec1-f19e-46b5-b49a-942e6b96ebae";

/* ===== Utils ===== */
function shuffle(input) {
    const array = input.slice(); // don't mutate original
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

function getLocalizedString(key) {
    const localization = {
        watchButton: {
            en: "Watch",
            fr: "Regarder",
            es: "Ver",
            de: "Ansehen",
            it: "Guarda",
            pt: "Assistir",
            zh: "观看",
            ja: "見る",
            ru: "Смотреть",
        },
    };

    const lang = (navigator.language || "en").slice(0, 2);
    return (localization[key] && (localization[key][lang] || localization[key].en)) || "";
}

function getBaseUrl() {
    let baseUrl = Emby.Page.baseUrl() + "/";
    if (window.location.href.includes("/index.html")) baseUrl += "index.html";
    return baseUrl;
}

function buildRating(item) {
    const rating = typeof item.community_rating === "number" ? Number(item.community_rating.toFixed(1)) : 0;
    if (rating === 0) return "";
    return `<div class="editorsChoiceItemRating starRatingContainer">
            <span class="material-icons starIcon star"></span>${rating}
          </div>`;
}

function buildLogoOrTitle(item, reduceImageSizes) {
    if (!item.hasLogo) return `<h1 class="editorsChoiceItemTitle">${item.name}</h1>`;
    const logoSize = reduceImageSizes ? "?width=300" : "";
    return `<img class="editorsChoiceItemLogo" src="../Items/${item.id}/Images/Logo/0${logoSize}" alt="${item.name}"/>`;
}

function buildOverview(item) {
    const overview = (item && typeof item.overview === "string") ? item.overview : "";
    return overview ? `<p class="editorsChoiceItemOverview">${overview}</p>` : "";
}

function buildBannerSizeParam(reduceImageSizes) {
    if (!reduceImageSizes) return "";
    const w = Math.max(window.screen.width, window.screen.height);
    return `?width=${w}`;
}

function ensureSplideLoaded() {
    return new Promise((resolve, reject) => {
        if (window.Splide) return resolve();

        const existing = document.querySelector('script[data-editorschoice-splide="1"]');
        if (existing) {
            existing.addEventListener("load", resolve, { once: true });
            existing.addEventListener("error", reject, { once: true });
            return;
        }

        const s = document.createElement("script");
        s.type = "text/javascript";
        s.src = "https://cdn.jsdelivr.net/npm/@splidejs/splide@4.1.4/dist/js/splide.min.js";
        s.setAttribute("data-editorschoice-splide", "1");
        s.addEventListener("load", resolve, { once: true });
        s.addEventListener("error", reject, { once: true });
        document.head.appendChild(s);

        if (!document.querySelector('link[data-editorschoice-splide="1"]')) {
            const l = document.createElement("link");
            l.rel = "stylesheet";
            l.href = "https://cdn.jsdelivr.net/npm/@splidejs/splide@4.1.4/dist/css/splide.min.css";
            l.setAttribute("data-editorschoice-splide", "1");
            document.head.appendChild(l);
        }
    });
}

/* ===== Render ===== */
function renderHeroSlide(item, data, baseUrl) {
    const rating = buildRating(item);
    const logoOrTitle = buildLogoOrTitle(item, data.reduceImageSizes);
    const overview = buildOverview(item);

    let button = "";

    if(data.showPlayButton) {
        let buttonString = getLocalizedString("watchButton");
        // Check if Custom Text is set.
        if (data.playButtonText) {
            buttonString = data.playButtonText;
        }
        button = `<button is="emby-button" class="editorsChoiceItemButton raised button-submit emby-button">
                    <span>${buttonString}</span>
                  </button>`;
    }

    const backdropSize = buildBannerSizeParam(data.reduceImageSizes);
    const backdropUrl = `../Items/${item.id}/Images/Backdrop/0${backdropSize}`;
    const extraClass = data.heroBackdropPosition === "center" ? "editorsChoiceBackdropCenter" :
        data.heroBackdropPosition === "top" ? "editorsChoiceBackdropTop" :
        data.heroBackdropPosition === "bottom" ? "editorsChoiceBackdropBottom" : "";

    return `
  <a href="${baseUrl}#/details?id=${item.id}"
     onclick="Emby.Page.showItem('${item.id}'); return false;"
     class="editorsChoiceItemBanner splide__slide">
    <div class="editorsChoiceBackdrop ${extraClass}" style="background-image:url('${backdropUrl}');"></div>
    <div class="editorsChoiceContent">
      ${logoOrTitle}
      ${rating}
      ${overview}
      ${button}
    </div>
  </a>`;
}

function renderNormalSlide(item, data, baseUrl) {
    const rating = buildRating(item);
    const logoOrTitle = buildLogoOrTitle(item, data.reduceImageSizes);
    const overview = buildOverview(item);

    const bannerSize = buildBannerSizeParam(data.reduceImageSizes);
    let button = "";
    if(data.showPlayButton) {
        let buttonString = getLocalizedString("watchButton");
        // Check if Custom Text is set.
        if (data.playButtonText) {
            buttonString = data.playButtonText;
        }
        button = `<button is="emby-button" class="editorsChoiceItemButton raised button-submit emby-button">
                    <span>${buttonString}</span>
                  </button>`;
    }

    return `<a href="${baseUrl}#/details?id=${item.id}"
                 onclick="Emby.Page.showItem('${item.id}'); return false;"
                 class="editorsChoiceItemBanner splide__slide"
                 style="background-image:url('../Items/${item.id}/Images/Backdrop/0${bannerSize}');">
                <div>
                  ${logoOrTitle}
                  ${rating}
                  ${overview}
                  ${button}
                </div>
              </a>`;
}

/* ===== Main setup ===== */
async function setup() {
    console.log("Attempting creation of editors choice slider.");

    const $containers = $(".homeSectionsContainer").filter((_, el) => !$(el).hasClass("editorsChoiceAdded"));
    if (!$containers.length) return;

    try {
        await ensureSplideLoaded();
    } catch (e) {
        console.warn("Editors Choice: Splide failed to load.", e);
        return;
    }

    $containers.each((_, elem) => {
        console.log("Fetching favourites data from API...");

        ApiClient.fetch({ url: ApiClient.getUrl("/EditorsChoice/favourites"), type: "GET" })
            .then((response) => response.json())
            .then((data) => {
                if (data.hideOnTvLayout && document.documentElement.classList.contains("layout-tv")) {
                    console.log("Editors Choice: hidden on TV layout by configuration.");
                    return;
                }

                const favourites = shuffle(data.favourites || []);
                const $containerElem = $(container);
                const containerId = `editorsChoice-${Date.now()}-${Math.floor(Math.random() * 10000)}`;

                $containerElem.first().attr("id", containerId);
                $containerElem.first().addClass(`editorsChoiceHeight-${data.bannerHeight}`);
                $(elem).prepend($containerElem);

                // TV focus workaround
                let focusResolved = false;
                $(`.layout-tv #${containerId} .emby-scrollbuttons button`).on("focus", function () {
                    if (focusResolved) return;
                    $('[is="emby-tabs"] .emby-button').first().trigger("focus");
                    focusResolved = true;
                });

                if (data.useHeroLayout) document.body.classList.add("editorsChoiceHeroMode");

                if ("heading" in data && data.heading && !data.useHeroLayout) {
                    $($containerElem).prepend(`<h2 class="sectionTitle sectionTitle-cards">${data.heading}</h2>`);
                }

                const baseUrl = getBaseUrl();
                const $list = $(`#${containerId} .editorsChoiceItemsContainer`);

                for (const item of favourites) {
                    const html = data.useHeroLayout
                        ? renderHeroSlide(item, data, baseUrl)
                        : renderNormalSlide(item, data, baseUrl);

                    $list.append(html);
                }

                $(elem).addClass("editorsChoiceAdded");

                // Toggle autoplay control visibility (button exists: #editorsChoicePlayPause)
                const playPauseBtn = document.getElementById("editorsChoicePlayPause");
                if (playPauseBtn) playPauseBtn.style.display = data.autoplay ? "" : "none";

                new Splide(`#${containerId} .splide`, {
                    type: data.transitionEffect ?? "loop",
                    autoplay: !!data.autoplay,
                    rewind: true,
                    interval: data.autoplayInterval,
                    pagination: false,
                    keyboard: true,
                    height: `${data.bannerHeight + (data.useHeroLayout ? 180 : 0)}px`, // Add 80px to the banner image height in hero mode to compensate for navbar overlay
                }).mount();
            })
            .catch((e) => console.warn("Editors Choice: failed to fetch/render.", e));
    });
}

window.onload = function () {
    // Detect if container is ready to setup slider
    const target = document.getElementById("reactRoot");
    if (!target) {
        console.warn("Editors Choice: reactRoot not found.");
        return;
    }

    const observer = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            const newNodes = mutation.addedNodes;
            if (!newNodes || !newNodes.length) continue;

            $(newNodes).each(function () {
                const $node = $(this);
                if ($node.hasClass("section0") && !$node.hasClass("editorsChoiceAdded")) {
                    setup();
                }
            });
        }
    });

    observer.observe(target, { attributes: true, childList: true, characterData: true, subtree: true });

    // Remind user that their favourites will be public when they add a new favourite.
    $("body").on("click", '[is="emby-ratingbutton"]', function () {
        if ($(this).hasClass("ratingbutton-withrating")) return;

        ApiClient.getPluginConfiguration(GUID).then((data) => {
            if (ApiClient.getCurrentUserId() === data.EditorUserId) {
                Dashboard.confirm("You are the featured items editor! Your favourites will be displayed on the home page for all users, if enabled.");
            }
        });
    });
};
