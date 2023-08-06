document.addEventListener("DOMContentLoaded", () => {
    let links = document.querySelectorAll('vk');
    for (let i = 0; i < links.length; i++) {
        links[i].addEventListener('click', (e) => {
            
            let method = e.target.textContent;
            let url = "https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/" + e.target.textContent + ".html";
            window.open(url, '_blank');
        });
    }
});