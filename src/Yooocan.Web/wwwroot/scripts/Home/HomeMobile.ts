import "../../../node_modules/jquery.counterup/jquery.counterup.min.js";
import "../../../node_modules/waypoints/lib/jquery.waypoints.min.js";
import { onScrollUp, onScrollDown, addProductCardsAnalytics } from "../Shared/Layout";
import { rotateItems } from "../Utils/Utils";

($('#mainBody .social-numbers .counter') as any).counterUp({ delay: 25, time: 2000, offset: 95 });

var isAddStoryButtonTitleShowing = true;
function toggleAddStoryButtonTitleVisibility(toShow: boolean) {
    if (toShow !== isAddStoryButtonTitleShowing) {
        $("#add-story-button-container h6").animate({ width: "toggle" }, 350);
        isAddStoryButtonTitleShowing = !isAddStoryButtonTitleShowing;
    }
}
onScrollUp(() => toggleAddStoryButtonTitleVisibility(true));
onScrollDown(() => toggleAddStoryButtonTitleVisibility(false));

addProductCardsAnalytics("mobile homepage");

var shopCategories = $("#shop-categories-tile a");
rotateItems(shopCategories);