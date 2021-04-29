isUndefined = function (obj) { return typeof obj === "undefined"; }

showLoading = showLoader = function () {
    var $loader = $('.landing-container');
    if (!$loader.is(':visible')) {
        $loader.fadeIn();
    }
}

hideLoading = hideLoader = function () {
    $(".landing-container").fadeOut();
}

function showErrors(id, html) {
    $('#' + id).html(html);
    $('#' + id).parent().slideDown();
}
function hideErrors(id) {
    $('#' + id).html();
    $('#' + id).parent().slideUp();
}

function showNotification(html, title) {
    showModal(html, title, 'modal-xl', 'Remove')
}
utilities = function () {

    //   Prevent Users from submitting form by hitting enter
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            if ($(event.target).hasClass('allow-newline')) {
                return true
            }
            else {
                event.preventDefault();
                return false;
            }
        }
    });
    // Toggle classes in body for syncing sliding animation with other elements
    $('#navbarNav')
        .on('show.bs.collapse', function (e) {
            $(".menu-slider").addClass('in');
        })
        .on('hide.bs.collapse', function (e) {
            $(".menu-slider").removeClass('in');
        });

    $(window).on("load", function () {
        $('body').addClass('pageloading');
        setTimeout(function () {
            $(".landing-container").fadeOut(1000);
        }, 1000)

    });

}


//alert = function (message, callback) {
//    //if ($.isFunction(bootbox) && !isUndefined(bootbox))
//    bootbox.alert({ title: 'تنبيه', message: message, callback: callback });
//}

//alertWithTitle = function (message, callback, title) {
//    bootbox.alert({ title: title, message: message, callback: callback });
//}

//confirm = function (message, callback) {
//    bootbox.confirm({ title: 'تأكيد', message: message, callback: callback });
//}




//initializing main application module
$(document).ready(function () {
    utilities(); 
});



