Array.from(document.getElementsByClassName("publish-button")).forEach(x => initToggleButton(x, "PUBLISH", "UNPUBLISH"));
Array.from(document.getElementsByClassName("delete-button")).forEach(x => initToggleButton(x, "UNDELETE", "DELETE"));

function initToggleButton(button: Element, positive: string, negative: string): void {
    button.addEventListener("click", e => {
        var button = e.currentTarget as HTMLButtonElement;
        var isNegative = button.innerText == positive;
        if (!isNegative && !confirm(`Are you sure that you want to ${negative.toLowerCase()} the service provider?`))
            return;

        fetch(button.attributes["data-route"].value, { method: "POST", credentials: 'same-origin' })
            .then(response => {
                if (response.status != 200)
                    return;
                
                button.innerText = isNegative ? negative : positive;
                button.classList.remove("red", "green");
                button.classList.add(isNegative ? "red" : "green");
            });
    })
}