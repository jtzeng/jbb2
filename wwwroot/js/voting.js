$(document).ready(function() {
    /*
    $('#posts').find('a').each(function() {
        var url = $(this).attr('lolurl');
        console.log(url);
    });
    */

    // Send AJAX voting events to the server.
    $('#posts').find('a').each(function() {
        $(this).click(function(event) {
            var url = $(this).attr('voteurl');
            console.log(url);

            var post_id = $(this).attr('post-id');

            $('#score' + post_id).load(url);

            event.preventDefault();
        });
    });

    // Resize images dynamically via click.
    $('img').each(function() {
        $(this).click(function(event) {
            var orig_src = $(this).attr('orig-src');
            var thumb_src = $(this).attr('thumb-src');

            // If it's a thumbnail preview, set the image source to the
            // original source. Otherwise, set it back to the thumbnail.
            if ($(this).attr('thumb') === '1') {
                $(this).attr('src', orig_src);
                $(this).attr('thumb', '0');
            } else {
                $(this).attr('src', thumb_src);
                $(this).attr('thumb', '1');
            }
        });
    });
});
