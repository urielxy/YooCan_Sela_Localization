function patchUserFromInputs(...inputs: HTMLElement[]) {
    var inputsCasted = inputs as HTMLInputElement[];
    var properties = new Map<string, string>(inputsCasted.map((input): [string, string] => [input.name, input.value]));
    return patchUser(properties);
}

export function patchUserFromInputsJquery(inputs: JQuery) {
    return patchUserFromInputs(...inputs.toArray());
}

export function clearUserProperty(name: string) {
    return patchUser(new Map([[name, ""]]));
}

function patchUser(properties: Map<string, string>) {
    var userId: string = $("#user-id").val();
    var patchObject = Array.from(properties).map(([key, value]) => ({
        "op": "replace",
        "path": "/" + key,
        "value": value
    }));
    return $.ajax("/User/" + (userId || ""), {
        method: "PATCH",
        contentType: "application/json",
        data: JSON.stringify(patchObject)
    });
}