import * as LoadMore from "../Utils/LoadMore";
import { addProductCardsAnalytics } from "../Shared/Layout";

LoadMore.initLoadMoreButton("shop category", 24, undefined, $('.feed').find(".card").length);

addProductCardsAnalytics('shop category');