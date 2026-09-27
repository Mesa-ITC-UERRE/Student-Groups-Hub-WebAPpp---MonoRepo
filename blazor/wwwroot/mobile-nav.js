document.addEventListener("click", (event) => {
    if (!(event.target instanceof Element) || !event.target.closest(".mobile-menu-tile")) return;

    const menu = document.getElementById("navbarMain");
    if (!menu || !menu.classList.contains("show")) return;

    bootstrap.Collapse.getOrCreateInstance(menu).hide();
});
