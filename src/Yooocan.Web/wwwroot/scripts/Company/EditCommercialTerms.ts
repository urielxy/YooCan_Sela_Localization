$("#add-shipping-rule-button").click(function () {
    var newIndex = $("#shipping-rules .shipping-rule").length;
    var newRule = $(".shipping-rule-template tr").clone();
    newRule.find("input").each((_, input) => {
        input.id = input.id.replace("_0__", `_${newIndex}__`);
        $(input).attr("name", (_, name) =>
            name.replace("[0]", `[${newIndex}]`));
    });
    newRule.appendTo($("#shipping-rules"));
    return false;
});

$("#delete-last-shipping-rule-button").click(function () {
    $("#shipping-rules .shipping-rule:last-child").remove();
    return false;
});