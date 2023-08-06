

/* This Function for Unique Target toggles */
document.addEventListener('DOMContentLoaded', () => {
    let toggles = document.querySelectorAll('.event-toggle');

    for (let i = 0; i < toggles.length; i++) {
        
        toggles[i].addEventListener('click', (e) => {
            let targetid = e.target.dataset.target;

            if (!targetid) {
                targetid = e.target.parentElement.dataset.target;
            }

            ToggleExpanded(targetid);
        });
    }
});
function ToggleExpanded(elementId) {
    let container = document.getElementById(elementId);
    container.classList.toggle('expanded');
};


//Discord Link Copy
function copyToClipboard(discord_id, buttonElement) {
    // search for the target target by it's id
    const targetElement = document.querySelector('#' + discord_id);
    // Get the DataValue can be Copied.
    //This is because I wanted to the discord url to be defined at the top of the page.
    const textToCopy = targetElement.getAttribute('data-value');
    //console.log(discordid);
    // Copy the text to the clipboard
    navigator.clipboard.writeText(textToCopy).then(function () {
        // Update the button text
        buttonElement.textContent = 'Copied!';
        buttonElement.style.opacity = 1;
        // Restore the original button text after a delay
        setTimeout(() => {
            buttonElement.textContent = textToCopy;
        }, 3000);
    }).catch(function (err) {
        console.error('Failed to copy text: ', err);
    });
}