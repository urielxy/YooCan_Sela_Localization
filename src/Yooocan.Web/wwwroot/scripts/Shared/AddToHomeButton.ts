import "../../lib/add-to-homescreen/style/addtohomescreen.css";
import "../../lib/add-to-homescreen/src/addtohomescreen.js";
import { isMobile } from "./Layout";

var lastVisitKey = "last-homescreen-button-visit";
if (isStandAlone()) {
    if (!localStorage.getItem(lastVisitKey)) {
        ga("send", "event", "homescreen button", "first visit", undefined, { nonInteraction: true });
    }
    localStorage.setItem(lastVisitKey, new Date().toJSON());
    ga("send", "event", "homescreen button", "visit", undefined, { nonInteraction: true });
}
//have to do extra checks because addToHomeScreen's check isn't good for android and chrome on ios and google search on GSA
else if (isMobile() && !localStorage.getItem(lastVisitKey) && window.navigator.userAgent.indexOf("CriOS") < 0 && window.navigator.userAgent.indexOf("GSA") < 0) {
    (window as any).addToHomescreen({
                                        skipFirstVisit: true,
                                        maxDisplayCount: 1
                                    });
    improveIosForwardCompatibility();
}

function isStandAlone() {
    return isMobile() && ((window.navigator as any).standalone === true || window.matchMedia("(display-mode: standalone)").matches);
}

function improveIosForwardCompatibility() {
    new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            var newNodes = mutation.addedNodes;
            if (newNodes !== null) {
                const $nodes = $(newNodes);
                $nodes.each(function () {
                    const $node = $(this);
                    $node.find(".ath-ios:not(.ath-ios6,.ath-ios7,.ath-ios8)").addClass("ath-ios8");
                });
            }
        });
    }).observe($("html")[0], {
        childList: true
    });

}