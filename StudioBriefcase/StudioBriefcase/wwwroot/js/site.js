function setMainHeight() {
    const headerHeight = $('header').outerHeight(true);
    const footerHeight = $('footer').outerHeight(true);
    const windowHeight = window.innerHeight;
    const mainHeight = windowHeight - headerHeight - footerHeight;
    
    $('#mainHeight').css('height', mainHeight + 'px');
    $('.finescroll').css('height', mainHeight + 'px');
}

function positionStickyPanel() {
    const headerHeight = $('header').outerHeight(true);
    $('.sticky').css('top', headerHeight + 'px');
}

function TogglerIconClick() {
    setTimeout(function () {
        setMainHeight();
        //positionStickyPanel();
    }, 0);

}

// Set mainHeight on page load
$(document).ready(function () {
  
    setTimeout(function () {
        setMainHeight();
        positionStickyPanel();
    }, 50);
    
});

// Set mainHeight on window resize
$(window).on('resize', function () {
    setMainHeight();
    positionStickyPanel();
});

