const toggleDarkMode = document.getElementById('toggleColorMode');
const body = document.body;

function updateElementTheme(element, oldClass, newClass) {
    element.classList.remove(oldClass);
    element.classList.add(newClass);
}

function setTheme(theme) {
    if (theme === 'dark') {
        updateElementTheme(body, 'BG', 'dark_BG');
        const lightElements = document.querySelectorAll('.FG, .BG');
        lightElements.forEach(element => {
            if (element.classList.contains('FG')) {
                updateElementTheme(element, 'FG', 'dark_FG');
            } else if (element.classList.contains('BG')) {
                updateElementTheme(element, 'BG', 'dark_BG');
            }
        });
        toggleDarkMode.textContent = 'LM';
    } else {
        updateElementTheme(body, 'dark_BG', 'BG');
        const darkElements = document.querySelectorAll('.dark_FG, .dark_BG');
        darkElements.forEach(element => {
            if (element.classList.contains('dark_FG')) {
                updateElementTheme(element, 'dark_FG', 'FG');
            } else if (element.classList.contains('dark_BG')) {
                updateElementTheme(element, 'dark_BG', 'BG');
            }
        });
        toggleDarkMode.textContent = 'DM';
    }
}

function loadStoredTheme() {
    const storedTheme = localStorage.getItem('theme');
    if (storedTheme) {
        setTheme(storedTheme);
    }
}

toggleDarkMode.addEventListener('click', () => {
    const currentTheme = body.classList.contains('BG') ? 'light' : 'dark';
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
    localStorage.setItem('theme', newTheme);
});

loadStoredTheme();
export { loadStoredTheme };