//$('.item-nav').click(function (event) {
//    // Avoid the link click from loading a new page
//    event.preventDefault();

//    // Load the content from the link's href attribute
//    $('.content').load($(this).attr('href'));
//});

function refreshSection(href) {
    event.preventDefault();

    $('.content').load($(this).attr(href))
}