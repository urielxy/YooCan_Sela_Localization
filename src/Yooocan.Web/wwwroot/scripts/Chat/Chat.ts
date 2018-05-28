declare const iflychat: any;
import { registerModal, isAuthenticated, getQueryParams } from "../Shared/Layout";
import { openNameFormModal } from "../User/NamePrompt";

var $chatButton = $("#chat-button");
var $chatRegisterButton = $(".chat-register-button");
var tokenServiceUrl = $chatButton.data("token-service-url");
$.get(tokenServiceUrl)
    .done(data => handleTokenResponse(data as ChatModel))
    .fail();

function handleTokenResponse(model: ChatModel) {
    if (model.userToken) {
        initIFlyChat(model);
    }
    else {
        if (!isAuthenticated()) {
            $chatRegisterButton.click(function () {
                registerModal($chatButton[0].id, new Map([["openNameForm", "1"]]));
            });
        }
        else {
            $chatRegisterButton.click(() => openNameFormModal());
            if (getQueryParams()["openNameForm"]) {
                openNameFormModal();
            }
        }
        $chatRegisterButton.show();
    }
}

function initIFlyChat(model: ChatModel) {
    (window as any).iflychat_auth_token = model.userToken;
    var iflychat_app_id = model.appId;
    var iflychat_external_cdn_host = "cdn.iflychat.com";

    //Boot iFlyChat App
    var iflychat_bundle = document.createElement("SCRIPT") as any;
    iflychat_bundle.src = "//" + iflychat_external_cdn_host + "/js/iflychat-v2.min.js?app_id=" + iflychat_app_id;
    iflychat_bundle.async = "async";
    document.body.appendChild(iflychat_bundle);

    //Load Popup Chat
    var iflychat_popup = document.createElement("DIV");
    iflychat_popup.className = "iflychat-popup";
    document.body.appendChild(iflychat_popup);
}

class ChatModel {
    userToken: string;
    appId: string;
}

var defaultChatroomId = parseInt($("#default-chatroom-id").text());
if (defaultChatroomId) {
    (window as any).iflychatAsyncInit = () => {
        iflychat.on('ready', () => {
            iflychat.startChat({
                id: `c-${defaultChatroomId}`,
                state: "open"
            });
        });
    };
}