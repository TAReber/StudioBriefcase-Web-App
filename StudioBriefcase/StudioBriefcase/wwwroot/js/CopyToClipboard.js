

function copyToClipboard(identifier, buttonElement) {
    // Get the target data
    const targetElement = document.querySelector('#' + identifier);

    // Get the value to be copied
    const textToCopy = targetElement.getAttribute('data-value');

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