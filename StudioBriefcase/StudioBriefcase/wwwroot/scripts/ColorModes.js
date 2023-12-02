const LocalStorage_Mode_Name = 'StudioBriefCase_Color_Mode';

document.addEventListener("DOMContentLoaded", () => {
    let mode = localStorage.getItem(LocalStorage_Mode_Name);

    //If a Local Storage Exists, load colors.    
    if (mode != null) {
        let button = document.getElementById('color_mode_toggle');
        toggleColorScheme(button);
    }
})

const lightmodeSet = [
    { variable: '--background', value: '#eeeeee' },
    { variable: '--foreground', value: '#cccccc' },
    { variable: '--text-color1', value: 'black' },
    { variable: '--text-color2', value: 'black' },
    { variable: '--shadow1', value: 'black' },
    { variable: '--shadow2', value: '#cccc' },
    { variable: '--offset', value: '#ffffff'},
    { variable: '--menu-background', value: '#111111' }
];

const darkmodeSet = [
    { variable: '--background', value: '#1a1e22' },
    { variable: '--foreground', value: '#393939' },
    { variable: '--text-color1', value: '#f6f6f6' },
    { variable: '--text-color2', value: 'white' },
    { variable: '--shadow1', value: '#888888' },
    { variable: '--shadow2', value: '#444444' },
    { variable: '--offset', value: '#cccccc' },
    { variable: '--menu-background', value: '#111111' }
]

//
function toggleColorScheme(buttonelement) {

    let modestring = buttonelement.textContent;
    
    if (modestring == "Light") {
        buttonelement.textContent = "Dark";
        ApplyColors(darkmodeSet);
        localStorage.setItem(LocalStorage_Mode_Name, 'Dark');
    }
    else if (modestring == "Dark") {
        buttonelement.textContent = "Light";
        
        ApplyColors(lightmodeSet);
        localStorage.setItem(LocalStorage_Mode_Name, 'Light');
        //Remove the Local Storage to use Default ColorScheme to save load times
        // Using Default Colors defined in Shared/_colors.cshtml
        localStorage.removeItem(LocalStorage_Mode_Name);
    }
}

function ApplyColors(ColorSet) {
    let styleelement = document.getElementById('colorscheme');

    for (let item of ColorSet) {
        styleelement.sheet.cssRules[0].style.setProperty(item.variable, item.value);
    }
}