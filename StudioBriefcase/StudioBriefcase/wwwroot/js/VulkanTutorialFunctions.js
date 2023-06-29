$(document).ready(function () {
    $(document).on("click", "vk", function () {
        console.log("Testing Click");
        var functionName = $(this).text();
        var url = "https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/" + functionName + ".html";
        window.open(url, '_blank');
    });
});