import { loadStoredTheme } from './ToggleColorScheme.js';

$(document).ready(function () {

    function loadContent(url) {
        console.log('Loading content from:', url);
        $('#tutorial_content').load(url, function () {
            // Dynamically loading content requires the theme to be dynamically applied to the new page.
            loadStoredTheme();

        });
    }

    // Attach click event listener to the links
    $('.content-link').on('click', function (e) {
        e.preventDefault(); // Prevent the default link behavior

        // Get the URL of the page to be loaded from the data-url attribute
        var url = $(this).data('url');

        // Load the content
        loadContent(url);
    });

    // Load the startup content on page load
    if ($('#tutorialLayout').length > 0) {
        // Call the function you want to run when _tutorialLayout is used
        var startupUrl = $('#tutorial_content').data('startup-url');
        
        loadContent(startupUrl);

    }
});