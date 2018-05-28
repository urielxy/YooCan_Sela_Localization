export function rotateItems(items: JQuery, interval: number = 2000) {
    setInterval(() => {
        items.each((i, c) => {
            if (!c.classList.contains("hide")) {
                c.classList.add("hide");
                items[(i + 1) % items.length].classList.remove("hide");
                return false;
            }
        });
    }, interval)
}